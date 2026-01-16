using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "WaveSystem/WaveData")]
public class WaveData : ScriptableObject
{
    [Header("Wave settings")]
    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private int enemyCount;
    [SerializeField] private float spawnRate;

    public IReadOnlyList<GameObject> EnemyPrefabs => enemyPrefabs;
    public int EnemyCount => enemyCount;
    public float SpawnRate => spawnRate;    
}
