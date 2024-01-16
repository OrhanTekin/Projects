using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private float rotationSpeed;

    void Update()
    {
        if(player != null)
        {
            transform.position = player.transform.position;
        }       
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    public void ActivateShield(int number)
    {
        if (number == 0) return;
        for(int i = 1; i<=number; i++)
        {
            gameObject.transform.GetChild(i % 8).gameObject.SetActive(true);
        }
    }

}
