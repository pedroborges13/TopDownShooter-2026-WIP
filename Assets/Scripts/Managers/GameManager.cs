using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public enum GameState { Prepatation, Combat, Paused, GameOver}
    public GameState currentState;

    [SerializeField] private float defaultPrepTime = 60f;
    private float prepTime;
    private int currentWave;


    //Events
    public event Action <GameState> OnGameStateChanged;
    public event Action<int> OnWaveChanged;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        prepTime = defaultPrepTime;
        currentState = GameState.Prepatation;
        Debug.Log("GameState: " + currentState);

        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.OnWaveEnded += EnterPreparationMode;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == GameState.Prepatation)
        {
            prepTime -= Time.deltaTime;
            if (prepTime <= 0) StartWave();
        }
    }

    public void StartWave() //UI button or timer
    {
        if (currentState == GameState.Combat) return;

        currentState = GameState.Combat;

        WaveManager.Instance.StartWave(currentWave);
        currentWave++;
        OnWaveChanged?.Invoke(currentWave);
    }

    void EnterPreparationMode()
    {
        Debug.Log("GameState: " + currentState);
        currentState = GameState.Prepatation;
        prepTime = defaultPrepTime;
        //Avisar UIManager mostrar botao e o timer
    }


    private void OnDisable()
    {
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.OnWaveEnded -= EnterPreparationMode;
        } 
    }
}
