using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.EventSystems.EventTrigger;
using Random = UnityEngine.Random;


public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Header("Wave List")]
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private float spawnRate;
    private float hpMod = 1f;
    private float speedMod = 1f;
    private int enemiesKilled;
    private int totalEnemies;
    private int extraEnemies;
    private int extraGroup;
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
        GlobalEvents.OnEnemyKilled += CheckWaveEnded;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartWave(WaveData data, int waveNumber)
    {
        //A cada X waves, aumenta a dificuldade dos inimigos
        if (waveNumber > 1 && waveNumber % 5 == 0)
        {
            hpMod += 0.4f;
            speedMod += 0.2f;
        }

        enemiesKilled = 0;
        totalEnemies = 0;
        extraGroup = 0;
        extraEnemies = (waveNumber - 1) * 2;
        foreach (var group in data.Groups) totalEnemies += (group.Count + extraEnemies);
        Debug.Log($"Total enemies: {totalEnemies} ExtraEnemies: {extraEnemies}");

        StartCoroutine(SpawnRoutine(data));
    }

    //Coroutine responsável pelo FLUXO e TEMPO da Wave
    IEnumerator SpawnRoutine(WaveData data)
    {
        //Quantos groups tem em WaveData
        foreach (var group in data.Groups)
        {
            //Sorteia um spawn aleatorio para o grupo nascer
            int randomSpawnIndex = Random.Range(0, spawnPoints.Count);

            for (int i = 0; i < group.Count + extraEnemies; i++)
            {
                SpawnEnemy(group.EnemyPrefab, randomSpawnIndex);
                yield return new WaitForSeconds(spawnRate); //Intervalo de spawns entre inimigos do mesmo grupo
            }

            //Intervalo entre os grupos
            yield return new WaitForSeconds(data.TimeBetweenGroups);
        }
    }

    //Responsavel por instanciar e configurar os inimigos
    void SpawnEnemy(GameObject prefab, int pointIndex)
    {
        Transform selectedPoint = spawnPoints[pointIndex];
        GameObject enemy = Instantiate(prefab, selectedPoint.position, Quaternion.identity);

        //Aplica buffs aos inimigos
        if(enemy.TryGetComponent<EntityStats>(out EntityStats stats)) //TryGetComponent é melhor para performance, faz tudo em uma operacao só comparado ao GetComponent.
        {
            stats.SetupEnemyStats(hpMod, speedMod); //Pega o metodo publico do EntityStats
            Debug.Log($"HP {stats.MaxHp}, Velocidade {stats.MoveSpeed}");
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
