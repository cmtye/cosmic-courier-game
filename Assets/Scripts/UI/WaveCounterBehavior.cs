using System;
using System.Collections;
using System.Collections.Generic;
using Enemy_Scripts.Spawning_Scripts;
using TMPro;
using UnityEngine;

public class WaveCounterBehavior : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private void Awake() 
    { 
        _text = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        SpawnManager.OnWaveOver += UpdateText;
    }

    private void OnDisable()
    {
        SpawnManager.OnWaveOver -= UpdateText;
    }

    private void UpdateText(int wave)
    {
        _text.SetText("Wave: " + (wave + 1));
    }
    
}
