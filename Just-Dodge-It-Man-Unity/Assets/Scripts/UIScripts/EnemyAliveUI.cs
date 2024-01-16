using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyAliveUI : MonoBehaviour
{

    private TextMeshProUGUI EnemyAliveText;

    void Start()
    {
        if(WaveManager.Instance != null)
        {
            WaveManager.Instance.OnEnemyAmountChanged += WaveManager_OnEnemyAmountChanged;
        }
        else
        {
            Debug.Log("This should not happen");
        }
        
        EnemyAliveText = GetComponent<TextMeshProUGUI>();
    }

    private void WaveManager_OnEnemyAmountChanged(object sender, WaveManager.OnEnemyAmountChangedEventArgs e)
    {
        EnemyAliveText.text = "Enemies " + e.newAmount;
    }

    private void OnDestroy()
    {
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.OnEnemyAmountChanged -= WaveManager_OnEnemyAmountChanged;
        }
    }
}
