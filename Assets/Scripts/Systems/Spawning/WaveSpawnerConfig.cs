using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Spawning/Wave Spawner Config", fileName = "WaveSpawnerConfig")]
public class WaveSpawnerConfig : ScriptableObject
{
    [Header("Enemy Definitions")]
    [SerializeField] private List<EnemyDefinition> enemies = new List<EnemyDefinition>();
    [SerializeField] private WaveProgressionCurve progressionCurve;

    [Header("Spawn Timing")]
    [SerializeField] private float delayBeforeFirstWave = 1f;
    [SerializeField] private float delayBetweenWaves = 2f;
    [SerializeField] private float delayBetweenEnemySpawns = 0.2f;

    [Header("Spawn Positioning")]
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
    [SerializeField] private float spawnRadiusAroundPoint = 6f;
    [SerializeField] private float navMeshSampleRadius = 8f;
    [SerializeField] private int navMeshSampleAttempts = 10;

    public IReadOnlyList<EnemyDefinition> Enemies => enemies;
    public WaveProgressionCurve ProgressionCurve => progressionCurve;
    public float DelayBeforeFirstWave => Mathf.Max(0f, delayBeforeFirstWave);
    public float DelayBetweenWaves => Mathf.Max(0f, delayBetweenWaves);
    public float DelayBetweenEnemySpawns => Mathf.Max(0f, delayBetweenEnemySpawns);
    public IReadOnlyList<Transform> SpawnPoints => spawnPoints;
    public float SpawnRadiusAroundPoint => Mathf.Max(0f, spawnRadiusAroundPoint);
    public float NavMeshSampleRadius => Mathf.Max(0.1f, navMeshSampleRadius);
    public int NavMeshSampleAttempts => Mathf.Max(1, navMeshSampleAttempts);
}
