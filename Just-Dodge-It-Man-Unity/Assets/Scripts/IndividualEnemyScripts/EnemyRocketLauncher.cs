using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRocketLauncher : IEnemy
{

    private enum State
    {
        MoveToInitialPosition,
        RotateAndShoot
    }

    private State state;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private GameObject homingMissile;
    [SerializeField] private Transform firePoint;

    private Vector2 target;

    [SerializeField] private float rocketDelay = 0.8f;
    private float rocketTimer;


    void Start()
    {
        state = State.MoveToInitialPosition;
        rocketTimer = rocketDelay;
    }

    
    void Update()
    {
        switch (state)
        {
            case State.MoveToInitialPosition:
                target = new Vector2(0, 0);
                MoveTo(target);

                if (ReachedTarget(target))
                {
                    state = State.RotateAndShoot;
                }
                break;
            case State.RotateAndShoot:
                transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

                if (rocketTimer > 0f)
                {
                    rocketTimer -= Time.deltaTime;
                }
                else
                {
                    ObjectPoolManager.SpawnObject(homingMissile, firePoint.position, firePoint.rotation, ObjectPoolManager.PoolType.EnemyRocket);
                    rocketTimer = rocketDelay;
                    SoundManager.PlaySound(SoundManager.Sound.RocketShootSound);
                }
                break;
        }
    }
}
