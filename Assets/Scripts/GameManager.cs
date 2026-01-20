using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    public enum GameState { Prepatation, Combat, Paused, GameOver}
    public GameState currentState;

    [SerializeField] private List<WaveData> waveTemplates;
    [SerializeField] private float defaultPrepTime = 60f;
    private float prepTime;
    private int currentWave = 1;

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

        WaveData nextWave = waveTemplates[Random.Range(0, waveTemplates.Count)];

        WaveManager.Instance.StartWave(nextWave, currentWave);
        currentWave++;
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
