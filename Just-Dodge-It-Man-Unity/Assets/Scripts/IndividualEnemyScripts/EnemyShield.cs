using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShield : IEnemy
{

    private enum State
    {
        MoveIntoScreen, //Upper center of screen
        ShootRevolver, // Shoot really quick bullet -> new bullet script
        DelayPhase,  //Add a delay 
        SaberPhaseMoveOut,
        SaberPhaseDash,
        ProtectionPhase, // Wield shield and occasionally fire rockets that follow player(destructible) until player comes into range and then move onto next phase
        LaserPhase, // Go to top of screen (protected by shield) and spawn 3-4 enemyturrets (player needs to destroy all) -> moveintoscreenphase      
    }

    private State state;

    [SerializeField] private Transform[] childObjects;
    [SerializeField] private GameObject[] weaponPrefabs;
    Player player;

    private Vector2 screenBounds;
    private Vector3 target;
    private Vector3 direction;
    private Vector2[] targetPos;
    private int rand;

    [SerializeField] private Transform firePoint;

    [SerializeField] private float revolverPhaseDuration = 0.6f;
    private float revolverPhaseTimer;
    [SerializeField] private int revolverShots;
    private int currentShotsLeft;
    [SerializeField] private float revolverShotsInBetweenDelay;
    private float revolverShotsInBetweenTimer;

    [SerializeField] private float totalMissiles = 5f;
    private float missilesLeft;
    [SerializeField] private float missileDelay = 1f;
    private float missileTimer;


    [SerializeField] private float laserPhaseDuration = 3f;
    private float laserPhaseTimer;
    GameObject[] laser = new GameObject[2];
    private bool laserActive;
    [SerializeField] private float laserRotationSpeed = 30;
    float rotationProgress;

    [SerializeField] private Transform bulletFirepoints;
    [SerializeField] private float bulletsShootDelay = 1f;
    private float bulletsShootTimer;

    [SerializeField] private int totalDashes;
    private int currentDashesLeft;
    [SerializeField] private float dashDuration;
    private float dashTimer;

    float x;
    float y;

    void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        targetPos = new Vector2[4] { new(0, screenBounds.y - 3), new(0, -screenBounds.y + 3), new(-screenBounds.x +3, 0), new(screenBounds.x -3, 0) };
        rand = Random.Range(0, targetPos.Length);

        state = State.MoveIntoScreen;
        if (Player.Instance != null)
        {
            player = Player.Instance;
        }
        else
        {
            Debug.Log("Player instance is null");
        }

        revolverPhaseTimer = revolverPhaseDuration;
        missileTimer = missileDelay;
        revolverShotsInBetweenTimer = revolverShotsInBetweenDelay;
        currentShotsLeft = revolverShots;

        missilesLeft = totalMissiles;
        for(int i = 0; i<2; i++)
        {
            laser[i] = ObjectPoolManager.SpawnObject(weaponPrefabs[2], transform.position, transform.rotation, ObjectPoolManager.PoolType.Laser);
            laser[i].SetActive(false);
        }
        laser[1].GetComponentInChildren<LaserScript>().defaultDistance = 10f;
        laserActive = false;

        laserPhaseTimer = laserPhaseDuration;
        bulletsShootTimer = bulletsShootDelay;
        dashTimer = dashDuration;
        currentDashesLeft = totalDashes;

    }

    void Update()
    {
        switch (state)
        {
            case State.MoveIntoScreen:
                ShieldAndGunToPlayer();
                target = targetPos[rand];
                MoveTo(target);
                ShowShield();

                
                if (ReachedTarget(target))
                {
                    state = State.ShootRevolver;                
                }
                break;
            case State.ShootRevolver:
                ShieldAndGunToPlayer();
                ShowRevolver();

                if (revolverPhaseTimer > 0f)
                {
                    revolverPhaseTimer -= Time.deltaTime;
                }
                else
                {
                    if(currentShotsLeft > 0)
                    {
                        if(revolverShotsInBetweenTimer > 0f)
                        {
                            revolverShotsInBetweenTimer -= Time.deltaTime;
                        }
                        else
                        {
                            //Shoot
                            ObjectPoolManager.SpawnObject(weaponPrefabs[0], firePoint.position, firePoint.rotation, ObjectPoolManager.PoolType.EnemyBullet);
                            revolverShotsInBetweenTimer = revolverShotsInBetweenDelay;
                            currentShotsLeft--;
                            SoundManager.PlaySound(SoundManager.Sound.RevolverSound);
                        }
                    }
                    else
                    {
                        state = State.DelayPhase;
                        revolverPhaseTimer = revolverPhaseDuration * 1.5f;
                        currentShotsLeft = revolverShots;
                    }                                      
                }

                break;
            case State.DelayPhase:
                ShieldAndGunToPlayer();
                if (revolverPhaseTimer > 0f)
                {
                    revolverPhaseTimer -= Time.deltaTime;
                }
                else
                {
                    state = State.SaberPhaseMoveOut;
                    revolverPhaseTimer = revolverPhaseDuration;

                    x = targetPos[rand].x * 2f;
                    y = targetPos[rand].y * 2f;

                    target = new Vector3(x, y, 0);
                }

                break;
            case State.SaberPhaseMoveOut:
                MoveTo(target);
                ShowShield();
                if (ReachedTarget(target))
                {
                    laser[1].SetActive(true);                  
                    state = State.SaberPhaseDash;
                    transform.position = RandomPositionOutsideScreen();
                    ShieldAndGunToPlayer();
                    if(player != null)
                    {
                        target = Vector3.Normalize(new Vector3(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y, 0));
                    }
                    ShowShield();
                    transform.GetComponentInChildren<TrailRenderer>().Clear();
                }
                break;
            case State.SaberPhaseDash:
                laser[1].transform.position = childObjects[1].transform.position;
                laser[1].transform.rotation = childObjects[1].transform.rotation;

                if(currentDashesLeft > 0)
                {
                    if (dashTimer > 0f)
                    {
                        dashTimer -= Time.deltaTime;
                        transform.position = transform.position + (target * speed * Time.deltaTime);
                    }
                    else
                    {
                        transform.position = RandomPositionOutsideScreen();
                        ShieldAndGunToPlayer();
                        if(player != null)
                        {
                            target = Vector3.Normalize(new Vector3(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y, 0));
                        }
                        dashTimer = dashDuration;
                        currentDashesLeft--;
                        transform.GetComponentInChildren<TrailRenderer>().Clear();
                    }

                }
                else
                {
                    state = State.ProtectionPhase;
                    currentDashesLeft = totalDashes;
                    laser[1].SetActive(false);
                }

                
                    break;
            case State.ProtectionPhase:
                target = targetPos[rand];
                MoveTo(target);
                ShieldAndGunToPlayer();

                if(missilesLeft > 0)
                {
                    if (missileTimer > 0f)
                    {
                        missileTimer -= Time.deltaTime;
                    }
                    else
                    {
                        //Shoot homing missiles
                        ObjectPoolManager.SpawnObject(weaponPrefabs[1], firePoint.position, firePoint.rotation, ObjectPoolManager.PoolType.EnemyRocket);
                        missilesLeft--;
                        missileTimer = missileDelay;
                        SoundManager.PlaySound(SoundManager.Sound.RocketShootSound);
                    }
                }
                else
                {
                    state = State.LaserPhase;

                    missilesLeft = totalMissiles;

                    laser[0].transform.position = new Vector3(0, 0, 0);
                    laser[0].SetActive(true);
                    laserActive = true;
                }
                break;
            case State.LaserPhase:
                ShieldAndGunToPlayer();
                x = targetPos[rand].x * 2f;
                y = targetPos[rand].y * 2f;

                target = new Vector3(x, y, 0);

                if (!ReachedTarget(target))
                {
                    MoveTo(target);
                    
                }
                else if (ReachedTarget(target))
                {
                    if(laserPhaseTimer > 0f)
                    {
                        laserPhaseTimer -= Time.deltaTime;

                        if (bulletsShootTimer > 0f)
                        {
                            bulletsShootTimer -= Time.deltaTime;
                        }
                        else
                        {
                            //Shoot
                            for(int i = 0; i<bulletFirepoints.childCount; i++)
                            {
                                ObjectPoolManager.SpawnObject(weaponPrefabs[3], bulletFirepoints.GetChild(i).localPosition, bulletFirepoints.GetChild(i).localRotation, ObjectPoolManager.PoolType.EnemyBullet);
                            }                           
                            bulletsShootTimer = bulletsShootDelay;
                            SoundManager.PlaySound(SoundManager.Sound.ShootingSound);
                        }
                    }
                    else
                    {
                        laserPhaseTimer = laserPhaseDuration;
                        rand = Random.Range(0, targetPos.Length);
                        state = State.MoveIntoScreen;
                    }                    
                }
                break;
        }

        if (laserActive)
        {     
            rotationProgress += laserRotationSpeed * Time.deltaTime;
            laser[0].transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotationProgress));                    
        }

    }


    void ShowShield()
    {
        childObjects[1].gameObject.SetActive(false);
        childObjects[2].gameObject.SetActive(true);
    }

    void ShowRevolver()
    {
        childObjects[1].gameObject.SetActive(true);
        childObjects[2].gameObject.SetActive(false);
    }

    void ShieldAndGunToPlayer()
    {
        //Make shield and gun always rotate to player
        if (player != null)
        {
            direction = new Vector3(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y, transform.position.z);
            
        }
        childObjects[0].transform.up = direction;
    }

    Vector2 RandomPositionOutsideScreen()
    {
        int XOrY = Random.Range(0,2);
        float xAxis;
        float yAxis;
        if(XOrY == 0)
        {
            //enemy dashes from the top or bottom
            xAxis = Random.Range(-screenBounds.x, screenBounds.x);
            yAxis = Random.Range(0, 2) == 0 ? screenBounds.y + 5 : -screenBounds.y - 5;

        }else
        {
            //left or right
            yAxis = Random.Range(-screenBounds.y, screenBounds.y);
            xAxis = Random.Range(0,2) == 0 ? screenBounds.x +5 : -screenBounds.x -5;          
        }

        return new Vector2(xAxis, yAxis);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        foreach(GameObject curr in laser)
        {
            if (curr != null)
            {
                curr.GetComponentInChildren<LaserScript>().defaultDistance = 50f;
                ObjectPoolManager.ReturnObjectToPool(curr);
            }
        }
             
    }

}
