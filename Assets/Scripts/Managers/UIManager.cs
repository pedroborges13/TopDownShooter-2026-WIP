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
    private Coroutine currentReloadCoroutine; //Stores reference to the active coroutine


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
    
    // ----- WEAPON HUD -----
    void UpdateWeaponName(string name)
    {
        weaponNameText.text = name;

        //Stops the old coroutine when switching weapons
        if (currentReloadCoroutine != null)
        {
            StopCoroutine(currentReloadCoroutine);  
            currentReloadCoroutine = null;
        }

        //Reset reload visuals when switching weapons
        if (reloadGroup != null) reloadGroup.SetActive(false);
        reloadFillImage.fillAmount = 0;
    }

    void UpdateAmmoText(int current, int max)
    {
        ammoText.text = $"{current}/{max}";

        if (current == 0) ammoText.color = Color.red; //Visual feedback: change colour when bullets are running out 
        else ammoText.color = Color.white;
    }

    void StartReloadVisual(float reloadTime)
    {
        if (currentReloadCoroutine != null) StopCoroutine(currentReloadCoroutine); //Stops the previous coroutine before starting new one

        //Reset reload visuals
        reloadFillImage.fillAmount = 0;
        if (reloadGroup != null) reloadGroup.SetActive(true);

        currentReloadCoroutine = StartCoroutine(ReloadAnimationRoutine(reloadTime)); //Starts new coroutine and saves the reference
    }

    IEnumerator ReloadAnimationRoutine(float reloadTime)
    {
        if(reloadGroup != null) reloadGroup.SetActive(true);
        reloadFillImage.fillAmount = 0;

        float timer = 0;

        while (timer < reloadTime)
        {
            timer += Time.deltaTime;

            //Fills from 0 to 1 based on time percentage
            reloadFillImage.fillAmount = timer / reloadTime;
            yield return null;
        }

        reloadFillImage.fillAmount = 1;

        yield return new WaitForSeconds(0.2f);
        if (reloadGroup != null) reloadGroup.SetActive(false);

        currentReloadCoroutine = null; //Clears the reference
    }

    //--------------------------

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
 