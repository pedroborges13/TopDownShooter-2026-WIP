using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI elements")]
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private GameObject preparationUI;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject gameOverScreen;

    [Header("Weapon HUD")]
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private Image reloadFillImage;
    [SerializeField] private GameObject reloadGroup;


    void OnEnable()
    {
        PlayerWallet.OnMoneyChanged += UpdateMoneyText; //Must subscribe in OnEnable to get PlayerWallet Start() value

        Weapon.OnWeaponEquipped += UpdateWeaponName;
        Weapon.OnAmmoChanged += UpdateAmmoText;
        Weapon.OnReloadStart += StartReloadVisual;

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

    void UpdateWeaponName(string name)
    {
        weaponNameText.text = name;

        if (reloadGroup != null) reloadGroup.SetActive(false);
        reloadFillImage.fillAmount = 0;
    }

    void UpdateAmmoText(int current, int max)
    {
        ammoText.text = $"{current}/{max}";

        if (current == 0) ammoText.color = Color.red;
        else ammoText.color = Color.white;
    }

    void StartReloadVisual(float duration)
    {
        StopCoroutine(nameof(ReloadAnimationRoutine));
        StartCoroutine(ReloadAnimationRoutine(duration));
    }

    IEnumerator ReloadAnimationRoutine(float duration)
    {
        if(reloadGroup != null) reloadGroup.SetActive(true);
        reloadFillImage.fillAmount = 0;

        float timer = 0;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            reloadFillImage.fillAmount = timer / duration;
            yield return null;
        }

        reloadFillImage.fillAmount = 1;

        yield return new WaitForSeconds(0.2f);
        if (reloadGroup != null) reloadGroup.SetActive(false);
    }

    void OnDisable()
    {
        Weapon.OnWeaponEquipped -= UpdateWeaponName;
        Weapon.OnAmmoChanged -= UpdateAmmoText;
        Weapon.OnReloadStart -= StartReloadVisual;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnWaveChanged -= UpdateWaveText;
            GameManager.Instance.OnGamePhaseChanged -= UpdateGamePhaseUI;
            GameManager.Instance.OnGameStatusChanged -= UpdateGameStatusUI;
        }
        PlayerWallet.OnMoneyChanged -= UpdateMoneyText;
    }
}
 