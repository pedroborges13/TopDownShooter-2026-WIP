using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState { Prepatation, Combat, Paused, GameOver}
    public GameState currentState;

    private float prepTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
            prepTime -= Time.time;
            if (prepTime <= 0) StartWave();
        }
    }

    public void StartWave() //UI button or timer
    {
        if (currentState == GameState.Combat) return;

        currentState = GameState.Combat;

        
    }

    void EnterPreparationMode()
    {
        currentState = GameState.Prepatation;
        prepTime = 60f;
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
