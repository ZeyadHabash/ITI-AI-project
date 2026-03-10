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

        if (!TryGetSpawnPosition(selected.NavMeshAreaName, out Vector3 spawnPosition))
        {
            Debug.LogWarning($"No valid NavMesh position found for area '{selected.NavMeshAreaName}'.", this);
            return;
        }

        GameObject spawned = Instantiate(selected.Prefab, spawnPosition, Quaternion.identity);
        HealthComponent health = spawned != null ? spawned.GetComponent<HealthComponent>() : null;
        if (health != null && health.Type == DamagableType.Enemy)
        {
            RegisterEnemy(health);
        }
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

    private bool TryGetSpawnPosition(string areaName, out Vector3 spawnPosition)
    {
        IReadOnlyList<Transform> points = config.SpawnPoints;
        if (points == null || points.Count == 0)
        {
            spawnPosition = transform.position;
            return NavMesh.SamplePosition(transform.position, out NavMeshHit fallbackHit, config.NavMeshSampleRadius, NavMesh.AllAreas)
                && AssignSpawnFromHit(fallbackHit, out spawnPosition);
        }

        int areaMask = BuildAreaMask(areaName);

        for (int i = 0; i < config.NavMeshSampleAttempts; i++)
        {
            Transform point = points[UnityEngine.Random.Range(0, points.Count)];
            if (point == null)
            {
                continue;
            }

            Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * config.SpawnRadiusAroundPoint;
            Vector3 candidate = point.position + new Vector3(randomOffset.x, 0f, randomOffset.y);
            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, config.NavMeshSampleRadius, areaMask))
            {
                spawnPosition = hit.position;
                return true;
            }
        }

        spawnPosition = Vector3.zero;
        return false;
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

    private static bool AssignSpawnFromHit(NavMeshHit hit, out Vector3 spawnPosition)
    {
        spawnPosition = hit.position;
        return true;
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
