using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurret : IEnemy
{

    private enum State
    {
        MoveToInitialPosition,
        RotateAndShoot
    }

    private State state;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float rotationSpeed;

    private Vector2 screenBounds;
    private Vector2 target;


    [SerializeField] private float shootDelay = 0.1f;
    private float shootTimer;

    private float direction = 1f;

    void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        state = State.MoveToInitialPosition;


        shootTimer = shootDelay;
    }

    void Update()
    {
        switch (state)
        {
            case State.MoveToInitialPosition:
                transform.eulerAngles = new Vector3(0, 0, 180);
                target = new Vector2(transform.position.x, screenBounds.y - 2);
                MoveTo(target);

                if (ReachedTarget(target))
                {
                    state = State.RotateAndShoot;
                }
                break;
            case State.RotateAndShoot:

                transform.Rotate(0, 0, rotationSpeed * Time.deltaTime * direction);

                if(transform.eulerAngles.z <= 110f)
                {
                    direction = 1;
                }else if(transform.eulerAngles.z >= 250f)
                {
                    direction = -1;
                }


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
