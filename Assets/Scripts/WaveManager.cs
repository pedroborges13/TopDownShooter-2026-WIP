using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Wave List")]
    [SerializeField] private List<WaveData> waves;

    [SerializeField] private float timeBetweenWaves; //45 segundos ou apertar o botao de pular

    private int currentWave;
    private int enemiesAlive;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
