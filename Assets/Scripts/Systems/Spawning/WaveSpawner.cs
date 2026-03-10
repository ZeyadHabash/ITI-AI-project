using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private WaveSpawnerConfig config;
    [SerializeField] private bool autoStart = true;

    public event Action<int> OnWaveStarted;
    public event Action<int> OnWaveCleared;

    public int CurrentWave => currentWave;
    public int AliveEnemyCount => aliveEnemies.Count;
    public bool IsRunning => isRunning;

    private readonly HashSet<HealthComponent> aliveEnemies = new HashSet<HealthComponent>();
    private Coroutine waveLoopRoutine;
    private int currentWave;
    private bool isRunning;

    private void Start()
    {
        if (autoStart)
        {
            StartSpawning();
        }
    }

    private void OnDisable()
    {
        UnsubscribeAllEnemies();
    }

    public void StartSpawning()
    {
        if (isRunning)
        {
            return;
        }

        if (!IsConfigValid())
        {
            Debug.LogWarning("WaveSpawner cannot start due to missing configuration.", this);
            return;
        }

        isRunning = true;
        waveLoopRoutine = StartCoroutine(WaveLoop());
    }

    public void StopSpawning()
    {
        if (!isRunning)
        {
            return;
        }

        isRunning = false;
        if (waveLoopRoutine != null)
        {
            StopCoroutine(waveLoopRoutine);
            waveLoopRoutine = null;
        }
    }

    private IEnumerator WaveLoop()
    {
        if (config.DelayBeforeFirstWave > 0f)
        {
            yield return new WaitForSeconds(config.DelayBeforeFirstWave);
        }

        while (isRunning)
        {
            currentWave++;
            OnWaveStarted?.Invoke(currentWave);

            List<EnemyDefinition> purchasePlan = BuildWavePurchasePlan(currentWave);
            for (int i = 0; i < purchasePlan.Count && isRunning; i++)
            {
                SpawnEnemy(purchasePlan[i]);
                if (config.DelayBetweenEnemySpawns > 0f)
                {
                    yield return new WaitForSeconds(config.DelayBetweenEnemySpawns);
                }
            }

            yield return new WaitUntil(() => !isRunning || AliveEnemyCount == 0);
            if (!isRunning)
            {
                yield break;
            }

            OnWaveCleared?.Invoke(currentWave);
            if (config.DelayBetweenWaves > 0f)
            {
                yield return new WaitForSeconds(config.DelayBetweenWaves);
            }
        }
    }

    private List<EnemyDefinition> BuildWavePurchasePlan(int waveNumber)
    {
        List<EnemyDefinition> plan = new List<EnemyDefinition>();
        int credits = config.ProgressionCurve.GetWaveCredits(waveNumber);

        if (credits <= 0)
        {
            return plan;
        }

        int lowestCost = GetLowestAffordableCost();
        if (lowestCost <= 0)
        {
            return plan;
        }

        int maxPurchases = 512;
        for (int i = 0; i < maxPurchases && credits >= lowestCost; i++)
        {
            EnemyDefinition selected = SelectEnemyForCredits(waveNumber, credits);
            if (selected == null)
            {
                break;
            }

            plan.Add(selected);
            credits -= selected.SpawnCost;
        }

        return plan;
    }

    private void SpawnEnemy(EnemyDefinition selected)
    {
        if (selected == null || selected.Prefab == null)
        {
            return;
        }

        if (!TryGetSpawnPositionForDefinition(selected, out Vector3 spawnPosition))
        {
            Debug.LogWarning($"[WaveSpawner] No valid NavMesh position found for '{selected.EnemyId}'. Skipping spawn.", this);
            return;
        }

        GameObject spawned = Instantiate(selected.Prefab, spawnPosition, Quaternion.identity);
        if (spawned == null)
        {
            return;
        }

        // Place the agent using its own agentTypeID so the sample targets the correct
        // baked NavMesh surface. Pre-instantiation sampling with only an area mask
        // ignores agent type and can return a position on an incompatible surface,
        // causing silent Warp failures and "not placed on NavMesh" errors at runtime.
        NavMeshAgent agent = spawned.GetComponent<NavMeshAgent>();
        if (agent != null && !PlaceAgentOnNavMesh(agent, spawnPosition))
        {
            Debug.LogWarning($"[WaveSpawner] Could not place '{selected.EnemyId}' on NavMesh near {spawnPosition}. Skipping spawn.", this);
            Destroy(spawned);
            return;
        }

        HealthComponent health = spawned.GetComponent<HealthComponent>();
        if (health != null && health.Type == DamagableType.Enemy)
        {
            RegisterEnemy(health);
        }
    }

    private bool TryGetSpawnPositionForDefinition(EnemyDefinition selected, out Vector3 spawnPosition)
    {
        NavMeshAgent prefabAgent = selected != null ? selected.Prefab.GetComponent<NavMeshAgent>() : null;
        if (prefabAgent == null)
        {
            // Non-agent enemies can still be placed by a broad NavMesh sample.
            return TrySampleAnyNavMesh(GetSpawnOriginCandidate(), config.NavMeshSampleRadius, out spawnPosition)
                || TrySampleAnyNavMesh(GetSpawnOriginCandidate(), config.NavMeshSampleRadius * 3f, out spawnPosition);
        }

        int definitionAreaMask = BuildAreaMask(selected.NavMeshAreaName);
        int effectiveAreaMask = prefabAgent.areaMask & definitionAreaMask;
        if (effectiveAreaMask == 0)
        {
            effectiveAreaMask = prefabAgent.areaMask;
        }

        NavMeshQueryFilter filter = new NavMeshQueryFilter
        {
            agentTypeID = prefabAgent.agentTypeID,
            areaMask = effectiveAreaMask
        };

        int attempts = Mathf.Max(1, config.NavMeshSampleAttempts);
        for (int i = 0; i < attempts; i++)
        {
            Vector3 candidate = GetSpawnOriginCandidate();
            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, config.NavMeshSampleRadius, filter))
            {
                spawnPosition = hit.position;
                return true;
            }

            if (NavMesh.SamplePosition(candidate, out NavMeshHit wideHit, config.NavMeshSampleRadius * 3f, filter))
            {
                spawnPosition = wideHit.position;
                return true;
            }
        }

        spawnPosition = Vector3.zero;
        return false;
    }

    private bool PlaceAgentOnNavMesh(NavMeshAgent agent, Vector3 origin)
    {
        if (agent.isOnNavMesh)
        {
            return true;
        }

        // Use the agent's own type and walkable mask so we only sample surfaces
        // that this agent type can actually navigate (Humanoid vs Flying, etc.).
        NavMeshQueryFilter filter = new NavMeshQueryFilter
        {
            agentTypeID = agent.agentTypeID,
            areaMask = agent.areaMask
        };

        if (NavMesh.SamplePosition(origin, out NavMeshHit hit, config.NavMeshSampleRadius, filter))
        {
            return agent.Warp(hit.position);
        }

        // Widen radius as a last resort before giving up.
        if (NavMesh.SamplePosition(origin, out NavMeshHit wideHit, config.NavMeshSampleRadius * 3f, filter))
        {
            return agent.Warp(wideHit.position);
        }

        return false;
    }

    private Vector3 GetSpawnOriginCandidate()
    {
        IReadOnlyList<Transform> points = config.SpawnPoints;
        if (points == null || points.Count == 0)
        {
            return transform.position;
        }

        for (int i = 0; i < config.NavMeshSampleAttempts; i++)
        {
            Transform point = points[UnityEngine.Random.Range(0, points.Count)];
            if (point != null)
            {
                Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * config.SpawnRadiusAroundPoint;
                return point.position + new Vector3(randomOffset.x, 0f, randomOffset.y);
            }
        }

        return transform.position;
    }

    private static int BuildAreaMask(string areaName)
    {
        if (!string.IsNullOrWhiteSpace(areaName))
        {
            int area = NavMesh.GetAreaFromName(areaName);
            if (area >= 0)
            {
                return 1 << area;
            }
        }

        int walkable = NavMesh.GetAreaFromName("Walkable");
        if (walkable >= 0)
        {
            return 1 << walkable;
        }

        return NavMesh.AllAreas;
    }

    private static bool TrySampleAnyNavMesh(Vector3 candidate, float radius, out Vector3 spawnPosition)
    {
        if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, radius, NavMesh.AllAreas))
        {
            spawnPosition = hit.position;
            return true;
        }

        spawnPosition = Vector3.zero;
        return false;
    }

    private EnemyDefinition SelectEnemyForCredits(int waveNumber, int availableCredits)
    {
        float totalWeight = 0f;
        int maxAffordableCost = GetMaxAffordableCost(availableCredits);
        IReadOnlyList<EnemyDefinition> enemies = config.Enemies;
        for (int i = 0; i < enemies.Count; i++)
        {
            EnemyDefinition enemy = enemies[i];
            if (!CanSelectEnemy(enemy, availableCredits))
            {
                continue;
            }

            float weighted = enemy.SelectionWeight
                * config.ProgressionCurve.GetCostSelectionMultiplier(enemy.SpawnCost, maxAffordableCost, waveNumber);
            if (weighted > 0f)
            {
                totalWeight += weighted;
            }
        }

        if (totalWeight <= 0f)
        {
            return null;
        }

        float threshold = UnityEngine.Random.value * totalWeight;
        float runningWeight = 0f;

        for (int i = 0; i < enemies.Count; i++)
        {
            EnemyDefinition enemy = enemies[i];
            if (!CanSelectEnemy(enemy, availableCredits))
            {
                continue;
            }

            float weighted = enemy.SelectionWeight
                * config.ProgressionCurve.GetCostSelectionMultiplier(enemy.SpawnCost, maxAffordableCost, waveNumber);
            if (weighted <= 0f)
            {
                continue;
            }

            runningWeight += weighted;
            if (threshold <= runningWeight)
            {
                return enemy;
            }
        }

        return null;
    }

    private bool CanSelectEnemy(EnemyDefinition enemy, int availableCredits)
    {
        return enemy != null
            && enemy.Prefab != null
            && enemy.SpawnCost > 0
            && enemy.SpawnCost <= availableCredits;
    }

    private int GetLowestAffordableCost()
    {
        int lowest = int.MaxValue;
        IReadOnlyList<EnemyDefinition> enemies = config.Enemies;
        for (int i = 0; i < enemies.Count; i++)
        {
            EnemyDefinition enemy = enemies[i];
            if (enemy == null || enemy.Prefab == null || enemy.SpawnCost <= 0)
            {
                continue;
            }

            if (enemy.SpawnCost < lowest)
            {
                lowest = enemy.SpawnCost;
            }
        }

        return lowest == int.MaxValue ? 0 : lowest;
    }

    private int GetMaxAffordableCost(int availableCredits)
    {
        int maxCost = 1;
        IReadOnlyList<EnemyDefinition> enemies = config.Enemies;
        for (int i = 0; i < enemies.Count; i++)
        {
            EnemyDefinition enemy = enemies[i];
            if (enemy == null || enemy.Prefab == null || enemy.SpawnCost > availableCredits)
            {
                continue;
            }

            if (enemy.SpawnCost > maxCost)
            {
                maxCost = enemy.SpawnCost;
            }
        }

        return maxCost;
    }


    private bool IsConfigValid()
    {
        return config != null
            && config.ProgressionCurve != null
            && config.Enemies != null
            && config.Enemies.Count > 0;
    }

    private void RegisterEnemy(HealthComponent health)
    {
        if (health == null || health.IsDead)
        {
            return;
        }

        if (aliveEnemies.Add(health))
        {
            health.OnDied += HandleEnemyDied;
        }
    }

    private void HandleEnemyDied(HealthComponent health)
    {
        if (health == null)
        {
            return;
        }

        if (aliveEnemies.Remove(health))
        {
            health.OnDied -= HandleEnemyDied;
        }
    }

    private void UnsubscribeAllEnemies()
    {
        foreach (HealthComponent enemy in aliveEnemies)
        {
            if (enemy != null)
            {
                enemy.OnDied -= HandleEnemyDied;
            }
        }

        aliveEnemies.Clear();
    }
}
