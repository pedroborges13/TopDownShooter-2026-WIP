using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Header("Wave List")]
    [SerializeField] private List<Transform> spawnPoints;
    private float hpMod;
    private float speedMod;
    private int enemiesKilled;
    private int totalEnemies;
    //[SerializeField] private List<WaveData> waves;
    //[SerializeField] private float timeBetweenWaves; //45 segundos ou apertar o botao de pular
    //private int currentWave;
    //private int enemiesAlive;
    //private bool waveActive;

    //Events
    public event Action OnWaveStarted;
    public event Action OnWaveEnded;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartWave(WaveData data, int waveNumber)
    {
        //A cada 5 waves, aumenta a dificuldade
        if (waveNumber > 1 && waveNumber % 5 == 0)
        {
            hpMod += 0.2f;
            speedMod += 0.2f;
        }

        enemiesKilled = 0;
        totalEnemies = 0;
        foreach (var group in data.Groups) totalEnemies += group.Count;

        StartCoroutine(SpawnRoutine(data));
    }

    IEnumerator SpawnRoutine(WaveData data)
    {
        foreach (var group in data.Groups)
        {
            for (int i = 0; i < group.Count; i++)
            {
                SpawnEnemy(group.EnemyPrefab, group.SpawnPoint);
                yield return new WaitForSeconds(0.2f); //Pequeno intervalo dentro do grupo
            }

            yield return new WaitForSeconds(data.TimeBetweenGroups);
        }
    }

    void SpawnEnemy(GameObject prefab, int pointIndex)
    {
        //Transform sp = spawnPoints(
        GameObject enemy = Instantiate(prefab, spawnPoints[1].position, Quaternion.identity);
    }
}
