using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaser : IEnemy
{
    private GameObject player;

    void Start()
    {
        if(Player.Instance != null)
        {
            player = Player.Instance.gameObject;
        }
        
    }



    void Update()
    {
        if(player != null)
        {
            transform.up = player.transform.position - transform.position;
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        }
    }

}
