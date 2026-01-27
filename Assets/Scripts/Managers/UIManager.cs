using TMPro;
using UnityEngine;
using UnityEngine.Android;

public class UIManager : MonoBehaviour
{
    [Header("UI elements")]
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private GameObject preparationUI;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject gameOverScreen;

    void OnEnable()
    {
        PlayerWallet.OnMoneyChanged += UpdateMoneyText; //Must subscribe in OnEnable to get PlayerWallet Start() value

        if (GameManager.Instance != null) SubscribeToGameManager();
    }

    void Start()
    {
        if (GameManager.Instance != null) SubscribeToGameManager(); //GameManager events trigger in Start(), causing subscription conflict in OnEnable
    }

    void SubscribeToGameManager() 
    {
        //Prevents duplication if OnEnable() subscription works
        GameManager.Instance.OnWaveChanged -= UpdateWaveText; 
        GameManager.Instance.OnGamePhaseChanged -= UpdateGamePhaseUI;
        GameManager.Instance.OnGameStatusChanged -= UpdateGameStatusUI;

        GameManager.Instance.OnWaveChanged += UpdateWaveText;
        GameManager.Instance.OnGamePhaseChanged += UpdateGamePhaseUI;
        GameManager.Instance.OnGameStatusChanged += UpdateGameStatusUI;

        UpdateGamePhaseUI(GameManager.Instance.CurrentPhase);
    }

    void UpdateWaveText(int currentWave)
    {
        waveText.text = "Wave " + currentWave.ToString();
    }

    void UpdateMoneyText(int currentMoney)
    {
       moneyText.text = "$: " + currentMoney.ToString();
    }

    void UpdateGamePhaseUI(GamePhase newPhase)
    {
        if (newPhase == GamePhase.Preparation)
        {
            preparationUI.SetActive(true);  
        }
        else if (newPhase == GamePhase.Combat)
        {
            preparationUI.SetActive(false); 
        }

    }

    void UpdateGameStatusUI(GameStatus newStatus)
    {
        if (newStatus == GameStatus.Paused)
        {
            pauseScreen.SetActive(true);
            gameOverScreen.SetActive(false);
        }
        else if (newStatus == GameStatus.GameOver)
        {
            pauseScreen.SetActive(false);
            gameOverScreen.SetActive(true);
        }
        else 
        {
            pauseScreen.SetActive(false);
            gameOverScreen.SetActive(false);
        }
    }

    void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnWaveChanged -= UpdateWaveText;
            GameManager.Instance.OnGamePhaseChanged -= UpdateGamePhaseUI;
            GameManager.Instance.OnGameStatusChanged -= UpdateGameStatusUI;
        }
        PlayerWallet.OnMoneyChanged -= UpdateMoneyText;
    }
}
 