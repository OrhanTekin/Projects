using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyJet : IEnemy
{

    private Vector3 direction = new Vector3(1f,0,0);
    [SerializeField] private float rotationProgressX = 0f;


    private void Start()
    {
        transform.rotation = Quaternion.Euler(0, 0, -90);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        if(transform.position.x < -30f)
        {
            direction = new Vector3(1f, 0, 0);
            transform.rotation = Quaternion.Euler(0, 0, -90);
            transform.position = new Vector3(transform.position.x, RandomY(), 0);
        }
        else if(transform.position.x > 30f)
        {
            direction = new Vector3(-1f, 0, 0);
            transform.rotation = Quaternion.Euler(0, 0, 90);
            transform.position = new Vector3(transform.position.x, RandomY(), 0);
        }
       
        transform.Rotate(0, rotationProgressX * Time.deltaTime, 0); 
    }

    int RandomY()
    {
        return Random.Range(-8, 9);
    }
}
