using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

[CreateAssetMenu]
public class PlayerStats : ScriptableObject
{
    [Header("Initial Values")]
    [SerializeField] private int initMaxHealth = 250;
    [SerializeField] private int initMovespeed = 15;
    [SerializeField] private float initShootDelayTime = 0.2f;
    [SerializeField] private float initRocketShootDelayTime = 0.8f;
    [SerializeField] private float initDamageMultiplier = 1f;
    [SerializeField] private float initPlayerSize = 1f;
    [SerializeField] private int initNumberOfShields = 0;


    [Header("Initial Ability Values")]
    [SerializeField] private bool initExtraGuns = false;
    [SerializeField] private bool initRockets = false;
    [SerializeField] private bool initPiercingShots = false;
    [SerializeField] private bool initRedBullet = false;

    private int _activeMaxHealth;
    private int _activeCurrentHealth;

    public int MaxHealth {
        get { return _activeMaxHealth; }
        set { 
            if(value != _activeMaxHealth)
            {
                _activeMaxHealth = Mathf.Max(value, 0);
                OnHealthChanged?.Invoke(this, new OnHealthChangedEventArgs
                {
                    health = _activeCurrentHealth,
                    maximumHealth = _activeMaxHealth
                });
            }
                    
            
            
        }
    }
    public int CurrentHealth {
        get { return _activeCurrentHealth; } 
        set
        {
            if(value != _activeCurrentHealth)
            {
                _activeCurrentHealth = Mathf.Max(value, 0);
                OnHealthChanged?.Invoke(this, new OnHealthChangedEventArgs
                {
                    health = _activeCurrentHealth,
                    maximumHealth = _activeMaxHealth
                });
            }
                 
            
            
        } 
    }
    public int Movespeed { get; set; }
    public float ShootDelayTime { get; set; }
    public float RocketShootDelayTime { get; set; }

    public float DamageMultiplier { get; set; }

    public float PlayerSize { get; set; }

    public int ShieldAmount { get; set; }

    public bool ExtraGuns { get; set; }
    public bool Rockets { get; set; }
    public bool PiercingShots { get; set; }
    public bool RedBullet { get; set; }


    //events
    public event EventHandler<OnHealthChangedEventArgs> OnHealthChanged;
    public class OnHealthChangedEventArgs : EventArgs
    {
        public int health;
        public int maximumHealth;
    }


    public void InitDefaults()
    {
        MaxHealth = initMaxHealth;
        CurrentHealth = initMaxHealth;
        Movespeed = initMovespeed;
        ShootDelayTime = initShootDelayTime;
        RocketShootDelayTime = initRocketShootDelayTime;
        DamageMultiplier = initDamageMultiplier;
        PlayerSize = initPlayerSize;
        ShieldAmount = initNumberOfShields;
        ExtraGuns = initExtraGuns;
        Rockets = initRockets;
        PiercingShots = initPiercingShots;
        RedBullet = initRedBullet;


        //Debug
        Bullet.SetPiercing(PiercingShots);
    }


    public void SetStat(PowerupSO powerup)
    {
        //handle selected powerup here
        switch (powerup.varName)
        {
            case "damageUp":
                DamageMultiplier += 0.3f;
                break;
            case "firerateUp":
                ShootDelayTime -= 0.01f;
                RocketShootDelayTime -= 0.08f;
                if(ShootDelayTime <= 0.1f)
                {
                    ShootDelayTime = 0.1f;
                }
                if(RocketShootDelayTime <= 0.1f)
                {
                    RocketShootDelayTime = 0.1f;
                }
                break;
            case "piercingShots":
                PiercingShots = true;
                Bullet.SetPiercing(PiercingShots);
                break;
            case "gunsUp":
                ExtraGuns = true;
                break;
            case "rockets":
                Rockets = true;
                break;
            case "redBullet":
                RedBullet = true;
                break;
            case "maxHealthBigUp":
                CurrentHealth = CurrentHealth + 500;
                MaxHealth = MaxHealth + 500;
                break;
            case "healPercent":
                int percentOfMaxHealth = MaxHealth / 2;
                if (CurrentHealth + percentOfMaxHealth > MaxHealth)
                {
                    CurrentHealth = MaxHealth;
                }
                else
                {
                    CurrentHealth += percentOfMaxHealth;
                }
                break;
            case "maxHealthUp":
                CurrentHealth = CurrentHealth + 250;
                MaxHealth = MaxHealth + 250;
                break;
            case "movespeedUp":
                Movespeed += 5;
                break;
            case "decreaseSize":
                PlayerSize -= 0.1f;
                break;
            case "shieldUp":
                ShieldAmount++;
                break;
            case "unlockAll":
                ShootDelayTime = 0.1f;
                RocketShootDelayTime = 0.1f;
                DamageMultiplier += 10f;
                PiercingShots = true;
                Bullet.SetPiercing(PiercingShots);
                ExtraGuns = true;
                Rockets = true;
                RedBullet = true;
                MaxHealth = MaxHealth + 5000;
                CurrentHealth = MaxHealth;
                Movespeed = Movespeed + 15;
                ShieldAmount = 8;
                PlayerSize = 0.5f;
                break;
        }
    }
}
