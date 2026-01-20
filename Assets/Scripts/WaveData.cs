using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "WaveSystem/WaveData")]
public class WaveData : ScriptableObject
{
    [Header("Wave settings")]
    [SerializeField] private List<EnemyGroup> groups;
    [SerializeField] private float timeBetweeenGroups;
    //[SerializeField] private int enemyCount;
    //[SerializeField] private float spawnRate;

    public IReadOnlyList<EnemyGroup> Groups => groups;
    public float TimeBetweenGroups => timeBetweeenGroups;   
    //public int EnemyCount => enemyCount;
    //public float SpawnRate => spawnRate;
}

[System.Serializable]
public class EnemyGroup
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int count;
    //[SerializeField] private int spawnPoint;

    public GameObject EnemyPrefab => enemyPrefab;
    public int Count => count;
    //public int SpawnPoint => spawnPoint;
}
