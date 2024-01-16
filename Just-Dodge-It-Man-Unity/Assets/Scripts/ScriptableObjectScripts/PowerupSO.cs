using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class PowerupSO : ScriptableObject
{
    public string description;
    public Transform imageTransform; //maybe change later?
    public string varName;
    [SerializeField] public bool initInfinitely;
    [SerializeField] private int initAmount;

    public int CurrAmount { get; set; }

    public void InitDefault()
    {
        CurrAmount = initAmount;
    }
}
