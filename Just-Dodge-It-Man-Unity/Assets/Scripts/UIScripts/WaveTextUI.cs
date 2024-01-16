using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;


public class WaveTextUI : MonoBehaviour
{
    private TextMeshProUGUI WaveText;

    void Start()
    {
        WaveManager.Instance.OnWaveChanged += WaveManager_OnWaveChanged;


        WaveText = GetComponent<TextMeshProUGUI>();
    }

    private void WaveManager_OnWaveChanged(object sender, WaveManager.OnWaveChangedEventArgs e)
    {
        WaveText.text = "Wave " + e.nextWave;
    }

    private void OnDestroy()
    {
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.OnWaveChanged -= WaveManager_OnWaveChanged;
        }
    }
}
