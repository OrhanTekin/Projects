using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTank : IEnemy
{

    private enum State
    {
        InitialMove, // Move to top of screen
        MoveToEdge, //Move to left edge, do rotation of moves and move to right edge and so on
        ShootRockets, // Rotate and keep rotating to player position and shoot fast rockets
        ShootLaser, // rotate to right side of the screen on left edge and left side on right edge and fire laser to almost entire space
        MoveToCenter, // move to center and reset rotation
        ShootBulletWave, // fire bullet wave -> go back to MoveToEdge state 
    }

    private State state;
    [SerializeField] private float rotationSpeed = 1f;

    [SerializeField] private GameObject[] weaponPrefabs;
    [SerializeField] private Transform[] allFirePoints;

    Player player;
    bool moveLeft = false;

    private Vector2 screenBounds;
    private Vector2 target;
    private Vector3 direction;

    //Rocket State vars
    [SerializeField] private int rocketShots = 3;
    private int rocketShotsLeft;
    private float rocketsShotCooldownTimer = 0.2f; //change below aswell

    //Laser State vars
    //part 1
    private float shootLaserStateInitialDelayTimer; //change below aswell

    //part 2
    GameObject laser;
    bool activateLaser;
    Quaternion rotationStart;
    float rotationProgress;

    //Bullet Wave Phase
    private int rotationsBulletWavePhase=4;
    private float rotationDelayTimer;
    private float BulletWaveDelayTimer;


    void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));   
        
        state = State.InitialMove;
        if (Player.Instance != null)
        {
            player = Player.Instance;
        }
        else
        {
            Debug.Log("Player instance is null");
        }

        laser = ObjectPoolManager.SpawnObject(weaponPrefabs[2], allFirePoints[0].position, allFirePoints[0].rotation, ObjectPoolManager.PoolType.Laser);
        laser.SetActive(false);
        Reset();
    }

    void Update()
    {
        switch (state)
        {
            case State.InitialMove:
                target = new Vector2(0, screenBounds.y - 2);
                MoveTo(target);
                if (ReachedTarget(target))
                {
                    state = State.MoveToEdge;
                }
                break;
            case State.MoveToEdge:
                if (moveLeft)
                {
                    //move to left edge
                    target = new Vector2(-screenBounds.x + 5, transform.position.y);
                    MoveTo(target);
                    if (ReachedTarget(target))
                    {
                        state = State.ShootRockets;
                    }
                }
                else
                {
                    //move to right edge
                    target = new Vector2(screenBounds.x - 5, transform.position.y);
                    MoveTo(target);
                    if (ReachedTarget(target))
                    {
                        state = State.ShootRockets;
                    }
                }
                break;
            case State.ShootRockets:
                if(player != null)
                {
                    direction = new Vector3(transform.position.x - player.transform.position.x, transform.position.y - player.transform.position.y, 0);
                }     

                transform.up = direction;
                if(rocketShotsLeft > 0)
                {
                    if(rocketsShotCooldownTimer > 0f)
                    {
                        rocketsShotCooldownTimer -= Time.deltaTime;
                    }
                    else
                    {
                        rocketsShotCooldownTimer = 0.2f;
                        ObjectPoolManager.SpawnObject(weaponPrefabs[1], allFirePoints[1].transform.position, allFirePoints[1].rotation, ObjectPoolManager.PoolType.EnemyRocket);
                        ObjectPoolManager.SpawnObject(weaponPrefabs[1], allFirePoints[2].transform.position, allFirePoints[2].rotation, ObjectPoolManager.PoolType.EnemyRocket);
                        rocketShotsLeft--;
                        SoundManager.PlaySound(SoundManager.Sound.RocketShootSound);
                    }
                }
                else
                {
                    state = State.ShootLaser;
                }

                break;
            case State.ShootLaser:
                //after some delay shoot laser
                if (shootLaserStateInitialDelayTimer > 0f)
                {
                    //rotate to side of screen
                    direction = (moveLeft) ? new Vector3(-1, 0, 0) : new Vector3(1, 0, 0);
                    transform.up = Vector3.RotateTowards(transform.up, direction, rotationSpeed * Time.deltaTime, 0.0f);
                    shootLaserStateInitialDelayTimer -= Time.deltaTime;
                }
                else
                {
                    if (activateLaser)
                    {
                        //just the first time
                        activateLaser = false;
                        laser.SetActive(true);
                        rotationStart = transform.rotation;
                        direction = new Vector3(0, 1, 0);
                    }

                    rotationProgress += rotationSpeed * Time.deltaTime;
                    transform.rotation = Quaternion.Lerp(rotationStart, Quaternion.Euler(direction), rotationProgress);


                    //update laser aswell
                    laser.transform.position = allFirePoints[0].position;
                    Vector3 rotation = new Vector3(0, 0, allFirePoints[0].eulerAngles.z);
                    laser.transform.rotation = Quaternion.Euler(rotation);
                    

                    if(transform.eulerAngles == direction)
                    {
                        state = State.MoveToCenter;
                        laser.SetActive(false);
                    }
                }
                

                break;
            case State.MoveToCenter:
                target = new Vector2(0,0);
                MoveTo(target);
                if (ReachedTarget(target))
                {
                    state = State.ShootBulletWave;
                }
                break;
            case State.ShootBulletWave:
                if (rotationDelayTimer > 0f)
                {
                    rotationDelayTimer -= Time.deltaTime;

                    if(BulletWaveDelayTimer > 0f)
                    {
                        BulletWaveDelayTimer -= Time.deltaTime;
                    }
                    else
                    {
                        if (rotationsBulletWavePhase > 0)
                        {

                            for(int i=0; i<=180; i+=15)
                            {
                                //Shoot bullets
                                ObjectPoolManager.SpawnObject(weaponPrefabs[0], allFirePoints[0].position, Quaternion.Euler(0, 0, i), ObjectPoolManager.PoolType.EnemyBullet);
                                ObjectPoolManager.SpawnObject(weaponPrefabs[0], allFirePoints[0].position, Quaternion.Euler(0, 0, -i), ObjectPoolManager.PoolType.EnemyBullet);
                            }

                            BulletWaveDelayTimer = 0.5f;
                            rotationsBulletWavePhase--;
                            SoundManager.PlaySound(SoundManager.Sound.ShootingSound);
                        }
                        else
                        {
                            Reset();
                            state = State.InitialMove;
                        }
                    }
                }
                else
                {
                    //Rotate 90deg
                    transform.Rotate(0, 0, 90);
                    rotationDelayTimer = 1f;

                }
                break;
        }
       
    }


    private void Reset()
    {
        moveLeft = !moveLeft; //next time move to the other edge
        rocketShotsLeft = rocketShots;
        shootLaserStateInitialDelayTimer = 1f;
        activateLaser = true;
        rotationProgress = 0f;
        rotationsBulletWavePhase = 4;
        rotationDelayTimer = 1f;
        BulletWaveDelayTimer = 0.5f;

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }


    public override void OnDestroy()
    {
        base.OnDestroy();
        //Destroy(laser);
        if(laser != null)
        {
            ObjectPoolManager.ReturnObjectToPool(laser);
        }
        
    }
}




