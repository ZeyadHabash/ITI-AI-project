using UnityEngine;

[CreateAssetMenu(menuName = "Game/Spawning/Enemy Definition", fileName = "EnemyDefinition")]
public class EnemyDefinition : ScriptableObject
{
    [SerializeField] private string enemyId = "Enemy";
    [SerializeField] private GameObject prefab;
    [SerializeField] private int spawnCost = 1;
    [SerializeField] private float selectionWeight = 1f;
    [SerializeField] private string navMeshAreaName = "Walkable";

    public string EnemyId => enemyId;
    public GameObject Prefab => prefab;
    public int SpawnCost => Mathf.Max(1, spawnCost);
    public float SelectionWeight => Mathf.Max(0f, selectionWeight);
    public string NavMeshAreaName => navMeshAreaName;
}
