using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShip : IEnemy
{

    private enum State
    {
        MoveToInitialPosition,
        StayAndShoot
    }

    private State state;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform firePoint;

    private Vector2 screenBounds;
    private Vector2 target;

    [SerializeField] private float shootDelay = 0.4f;
    private float shootTimer;


    void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        state = State.MoveToInitialPosition;
        transform.Rotate(0, 0, 180);
        shootTimer = shootDelay;
    }

    
    void Update()
    {
        switch (state)
        {
            case State.MoveToInitialPosition:
                target = new Vector2(transform.position.x, screenBounds.y - 4);
                MoveTo(target);

                if (ReachedTarget(target))
                {
                    state = State.StayAndShoot;
                }
                break;
            case State.StayAndShoot:
                if(shootTimer > 0f)
                {
                    shootTimer -= Time.deltaTime;
                }
                else
                {
                    ObjectPoolManager.SpawnObject(bullet, firePoint.position, firePoint.rotation, ObjectPoolManager.PoolType.EnemyBullet);
                    SoundManager.PlaySound(SoundManager.Sound.ShootingSound);
                    shootTimer = shootDelay;
                }

                break;
        }
    }
}
