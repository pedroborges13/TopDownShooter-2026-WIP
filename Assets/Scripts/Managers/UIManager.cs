using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("UI elements")]
    [SerializeField] private TextMeshProUGUI waveText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnWaveChanged += UpdateWaveText;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateWaveText(int currentWave)
    {
        waveText.text = "Wave " + currentWave.ToString();
    }

    void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnWaveChanged -= UpdateWaveText;
        }
    }
}
 