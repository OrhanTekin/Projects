using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContainer : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private PlayerStats stats;
    [SerializeField] private Shield shield;
    


    void Start()
    {
        if (CanvasUI.Instance != null)
        {
            CanvasUI.Instance.OnPowerupSelected += CanvasUI_OnPowerupSelected;
        }
    }

    private void CanvasUI_OnPowerupSelected(object sender, EventArgs e)
    {

        shield.ActivateShield(stats.ShieldAmount);
        
        if (stats.RedBullet == true)
        {
            player.RedBullet();
        }
        player.transform.localScale = new Vector3(stats.PlayerSize, stats.PlayerSize, 0);
    }

    void OnDisable()
    {
        if (CanvasUI.Instance != null)
        {
            CanvasUI.Instance.OnPowerupSelected -= CanvasUI_OnPowerupSelected;
        }
    }
}
