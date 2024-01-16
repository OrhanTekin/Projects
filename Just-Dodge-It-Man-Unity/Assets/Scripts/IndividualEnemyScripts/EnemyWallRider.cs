using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWallRider : IEnemy
{

    private enum State
    {
        MoveToEdge,
        MoveAroundAndShoot
    }

    private State state;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform firePoint;

    private Vector2 screenBounds;
    private Vector2 target;

    [SerializeField] private float shootDelay = 0.8f;
    private float shootTimer;

    private Vector2[] allEdges;
    private int index;

    void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        state = State.MoveToEdge;
        allEdges = new Vector2[] {
            new Vector2(screenBounds.x - 1, screenBounds.y - 1),
            new Vector2(-screenBounds.x + 1, screenBounds.y - 1),
            new Vector2(-screenBounds.x + 1, -screenBounds.y + 1),
            new Vector2(screenBounds.x - 1, -screenBounds.y + 1)
        };
        index = 0;
        shootTimer = shootDelay;
    }

    
    void Update()
    {
        switch (state)
        {
            case State.MoveToEdge:
                target = allEdges[index];
                transform.eulerAngles = new Vector3(0,0,90);
                MoveTo(target);
                if (ReachedTarget(target))
                {
                    state = State.MoveAroundAndShoot;
                }
                break;
            case State.MoveAroundAndShoot:
                if (shootTimer > 0f)
                {
                    shootTimer -= Time.deltaTime;
                }
                else
                {
                    ObjectPoolManager.SpawnObject(bullet, firePoint.position, firePoint.rotation, ObjectPoolManager.PoolType.EnemyBullet);
                    SoundManager.PlaySound(SoundManager.Sound.ShootingSound);
                    shootTimer = shootDelay;
                }


                MoveTo(target);

                if (ReachedTarget(target))
                {
                    index++;
                    target = allEdges[index % 4];
                    transform.eulerAngles += new Vector3(0, 0, 90);
                }

                break;
        }
    }
}
