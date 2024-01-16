using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class HealthTextUI : MonoBehaviour
{

    private TextMeshProUGUI HealthText;
    [SerializeField] private PlayerStats stats;

    private void Start()
    {
        stats.OnHealthChanged += Stats_OnHealthChanged; 

        HealthText = GetComponent<TextMeshProUGUI>();
    }

    private void Stats_OnHealthChanged(object sender, PlayerStats.OnHealthChangedEventArgs e)
    {
        HealthText.text = "Health " + e.health + "/" + e.maximumHealth;
    }

    

    private void OnDestroy()
    {
        stats.OnHealthChanged -= Stats_OnHealthChanged;
    }
}
