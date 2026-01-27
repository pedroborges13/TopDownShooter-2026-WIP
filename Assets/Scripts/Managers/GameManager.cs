using System;
using UnityEngine;
public enum GamePhase { Preparation, Combat }
public enum GameStatus { Playing, Paused, GameOver }
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GamePhase CurrentPhase { get; private set; }
    public GameStatus CurrentStatus { get; private set; }

    [SerializeField] private float defaultPrepTime = 60f;
    private float prepTime;
    private int currentWave;

    //Events
    public event Action <GamePhase> OnGamePhaseChanged;
    public event Action <GameStatus> OnGameStatusChanged;
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
        CurrentPhase = GamePhase.Preparation;
        OnGamePhaseChanged?.Invoke(CurrentPhase);
        Debug.Log($"GameState: {CurrentPhase}");    

        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.OnWaveEnded += EnterPreparationMode;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentStatus != GameStatus.Playing) return;

        if (CurrentPhase == GamePhase.Preparation) PreparationTimer();
    }

    void PreparationTimer()
    {
        prepTime -= Time.deltaTime;
        if (prepTime <= 0) StartWave();
    }

    public void StartWave() //UI button or timer
    {
        if (CurrentPhase == GamePhase.Combat) return;

        CurrentPhase = GamePhase.Combat;
        OnGamePhaseChanged?.Invoke(CurrentPhase);
        Debug.Log($"GameState: {CurrentPhase}");

        WaveManager.Instance.StartWave(currentWave);
        currentWave++;
        OnWaveChanged?.Invoke(currentWave);
    }

    void EnterPreparationMode()
    {
        CurrentPhase = GamePhase.Preparation;
        OnGamePhaseChanged?.Invoke(CurrentPhase);
        Debug.Log($"GameState: {CurrentPhase}");
        prepTime = defaultPrepTime;
    }

    public void TogglePause()
    {
        if (CurrentStatus == GameStatus.Paused) ResumeGame();
        else PauseGame();
    }
    void PauseGame()
    {
        CurrentStatus = GameStatus.Paused;
        Time.timeScale = 0;
        OnGameStatusChanged?.Invoke(CurrentStatus);
    }

    void ResumeGame()
    {
        CurrentStatus = GameStatus.Playing;
        Time.timeScale = 1;
        OnGameStatusChanged?.Invoke(CurrentStatus);
    }
    private void OnDisable()
    {
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.OnWaveEnded -= EnterPreparationMode;
        } 
    }
}
