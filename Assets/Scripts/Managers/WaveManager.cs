using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.EventSystems.EventTrigger;
using Random = UnityEngine.Random;

[Serializable]
public class EnemyDatabase
{
    public GameObject normalPrefab;
    public GameObject runnerPrefab;
    public GameObject bossPrefab;
}
public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private EnemyDatabase database;
    [SerializeField] private List<Transform> spawnPoints;

    [Header("Settings")]
    [SerializeField] private float spawnRate;
    [SerializeField] private float spawnBetweenGroups;

    [Header("Runner Progression")]
    [SerializeField] private float initialRunnerChance;
    [SerializeField] private float runnerChanceIncrease;
    [SerializeField] private int runnerIncreaseInterval;

    //Control
    private float hpMod = 1f;
    private float speedMod = 1f;
    private int enemiesKilled;
    private int totalEnemies;

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
        GlobalEvents.OnEnemyKilled += CheckWaveEnded;
    }

    public void StartWave(int waveNumber)
    {
        //----- DIFICULT SCALING -----
        //Every 5 waves, increase enemy health modifier
        if (waveNumber > 1 && waveNumber % 5 == 0)
        {
            hpMod += 0.4f;
        }

        //----- ENEMY COUNT LOGIC -----
        //Total groups: (Base 2 + 1 every 3 rounds)
        int totalGroups = 2 + Mathf.FloorToInt(waveNumber / 3f);

        //Enemies per group: (Base 5 + 2 every 4 rounds)
        int enemiesPerGroup = 5 + (Mathf.FloorToInt(waveNumber / 4f) * 2);

        //----- RUNNER ENEMY SPAWN LOGIC -----
        //Runner enemy spawn chance
        float currentRunnerChance = initialRunnerChance;

        if(waveNumber > 1)
        {
            //Counts how many full 2-round cycles have passed
            int increases = (waveNumber - 1) / runnerIncreaseInterval;

            //Apply the increase
            currentRunnerChance += (increases * runnerChanceIncrease);
        }

        //Ensures the chance is never less than 0 or bigger than 1 (100%)
        currentRunnerChance = Mathf.Clamp01(currentRunnerChance);
        Debug.Log($"Wave {waveNumber}: Chance de Runner é de {currentRunnerChance * 100}%");

        // ----- TOTAL CALCULATION -----
        //Calculate total enemies for the wave
        totalEnemies = (totalGroups * enemiesPerGroup);
        if (waveNumber > 1 && waveNumber % 5 == 0) totalEnemies += 1; //+1 from Boss

        enemiesKilled = 0;

        Debug.Log($"Iniciando Wave {waveNumber}: {totalEnemies} inimigos totais.");
        //OnWaveStarted?

        StartCoroutine(SpawnProceduralRoutine(totalGroups, enemiesPerGroup, currentRunnerChance, waveNumber));
    }

   
    IEnumerator SpawnProceduralRoutine(int totalGroups, int enemiesPerGroup, float runnerChance, int waveNumber)
    {
        //Loop through all groups
        for (int g = 0; g < totalGroups; g++)
        {
            int spawnIndex = Random.Range(0, spawnPoints.Count);

            //Loop through all units inside the group
            for (int i = 0; i < enemiesPerGroup; i++)
            {
                GameObject prefabToSpawn;

                //Probability logic to choose between normal or runner
                if (Random.value < runnerChance) prefabToSpawn = database.runnerPrefab;
                else prefabToSpawn = database.normalPrefab;

                SpawnEnemy(prefabToSpawn, spawnIndex);
                yield return new WaitForSeconds(spawnRate);
            }

            yield return new WaitForSeconds(spawnBetweenGroups);
        }

        //Boss spawn logic (Every 5 rounds at the end of the wave???)
        if (waveNumber > 1 && waveNumber % 5 == 0)
        {
            int bossIndex = Random.Range(0, spawnPoints.Count);
            SpawnEnemy(database.bossPrefab, bossIndex); //Ver isso depois
        }

    }

    //Responsible for instantiating and configuring enemies
    void SpawnEnemy(GameObject prefab, int pointIndex)
    {
        if (prefab == null) return; 

        Transform selectedPoint = spawnPoints[pointIndex];
        Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)); //Small position variation to prevent them from spawning too close together
        GameObject enemy = Instantiate(prefab, selectedPoint.position + randomOffset, Quaternion.identity);

        //Apply difficulty buffs via EntityStats
        if (enemy.TryGetComponent<EntityStats>(out EntityStats stats)) //TryGetComponent performs a single operation (better performance than GetComponent + null check)
        {
            stats.SetupEnemyStats(hpMod, speedMod); //Accesses EntityStats public method
            //Debug.Log($"HP {stats.MaxHp}, Speed {stats.MoveSpeed}");
        }
    }

    void CheckWaveEnded()
    {
        enemiesKilled++;
        Debug.Log($"Enemies: {enemiesKilled} / {totalEnemies}");

        if(enemiesKilled >= totalEnemies)
        {
            OnWaveEnded?.Invoke();
        }
    }

    void OnDisable()
    {
        GlobalEvents.OnEnemyKilled -= CheckWaveEnded;
    }
}
