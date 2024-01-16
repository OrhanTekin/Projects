using UnityEngine;


public class EnemyFinalBoss : IEnemy
{

    private enum State
    {
        MoveToInitialPosition,   //move to top edge of screen
        RightArmRockets,         //rotate right connector and spawn normal rockets from the firepoints of the right arm
        LeftArmLaser,            //spawn one or many laser from left arm and rotate left connector and stay there with laser on
        Triangle,                //Move boss out of screen and activate Triangles Transform, one moves one stands still, create line between them and shoot or smth and then the other moves and so on
                                 //triangle phase is either on timer or number of shots
        HomingMissiles,          //Reset rotation and Spawn homing missiles from left and right connector              
    }
    //Once, when final boss is low, heal his health bar up again to nearly full
    //health up world space bottom of the screen

    private State state;

    [SerializeField] private GameObject leftConnector;
    [SerializeField] private GameObject rightConnector;

    private Vector2 screenBounds;
    private Vector2 target;
    private Vector3 direction;
    Quaternion rotationStart;
    private PolygonCollider2D polygonCollider;

    [SerializeField] private float rotationSpeed;
    private float rotationProgress;

    [SerializeField] private GameObject[] allFirePoints;
    [SerializeField] private GameObject rocketPrefab;
    [SerializeField] private float rocketsPhaseDuration;
    private float rocketPhaseTimer;
    [SerializeField] private float rocketShootDelay;
    private float rocketShootTimer;

    [SerializeField] private GameObject laserFirePoint;
    [SerializeField] private GameObject laserPrefab;
    GameObject laser;
    private bool activateLaser;

    private Quaternion laserPointInitRotation;
    [SerializeField] private Vector2 offsetLaserPos;


    private int totalDirectionChanges = 4;
    private int currDirectionChangesLeft;
    private int index = 0;
    private Vector3[] directions = new Vector3[4] { new Vector3(0,0,140f), new Vector3(0,0,10f), new Vector3(0,0,30f), new Vector3(0,0,-13f) };


    [SerializeField] private GameObject Triangles;
    [SerializeField] private float TrianglePhaseDuration;
    private float trianglePhaseTimer;
    private Vector2[] triangleTargets = new Vector2[4];

    [SerializeField] private GameObject slowBullet;
    [SerializeField] private float slowBulletShootDelay;
    private float slowBulletTimer;

    private bool homingMissiles = false;
    [SerializeField] private GameObject homingMissilePrefab;
    [SerializeField] private float missileShootDelay;
    private float missileShootTimer;

    private bool fillHpUp;
    private bool trigger;
    [SerializeField] private float fillDelay;
    private float fillTimer;
    private int fillAmountPerTick;
    private int totalAmountToHeal;


    void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        state = State.MoveToInitialPosition;
        polygonCollider = transform.GetComponent<PolygonCollider2D>();

        rocketPhaseTimer = rocketsPhaseDuration;
        rocketShootTimer = 0f;

        laser = ObjectPoolManager.SpawnObject(laserPrefab, laserFirePoint.transform.position, laserFirePoint.transform.rotation, ObjectPoolManager.PoolType.Laser);
        laser.SetActive(false);

        activateLaser = true;
        currDirectionChangesLeft = totalDirectionChanges;

        laserPointInitRotation = laserFirePoint.transform.rotation;

        trianglePhaseTimer = TrianglePhaseDuration;

        slowBulletTimer = slowBulletShootDelay;
        missileShootTimer = missileShootDelay;

        fillHpUp = true;
        trigger = false;
        fillAmountPerTick = maxHealth / 100;

        //Set camera of canvas for health bar for post processing after ui
        transform.GetChild(3).GetComponent<Canvas>().worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

        SoundManager.PlaySound(SoundManager.Sound.FinalBossEnterSound);

    }


    void Update()
    {
        switch (state)
        {
            case State.MoveToInitialPosition:
                target = new Vector2(0, screenBounds.y);
                MoveTo(target);

                if (ReachedTarget(target))
                {
                    state = State.RightArmRockets;
                }

                break;
            case State.RightArmRockets:
                if(rightConnector.transform.eulerAngles.z > 300f || rightConnector.transform.eulerAngles.z == 0f)
                {
                    rotationProgress -= rotationSpeed * Time.deltaTime;
                    rightConnector.transform.rotation = Quaternion.Euler(0, 0, rotationProgress);
                }
                else if(rightConnector.transform.eulerAngles.z <= 300f)
                {
                    if(rocketPhaseTimer > 0f)
                    {
                        rocketPhaseTimer -= Time.deltaTime;
                        ShootRockets();

                    }
                    else
                    {
                        state = State.LeftArmLaser;
                        rocketPhaseTimer = rocketsPhaseDuration;
                        target = new Vector2(1f, 20f);
                        rotationProgress = 0f;
                    }
                }
                
                break;
            case State.LeftArmLaser:
                if (!ReachedTarget(target))
                {
                    MoveTo(target);
                    ShootRockets();
                }
                else if(transform.eulerAngles.z < 90f || transform.eulerAngles.z > 95f)
                {
                    rotationProgress += rotationSpeed * Time.deltaTime;
                    transform.rotation = Quaternion.Euler(0, 0, rotationProgress);
                    ShootRockets();
                }
                else
                {
                    //start laser
                    if (activateLaser)
                    {
                        leftConnector.transform.eulerAngles = new Vector3(0,0,90f);   //set it to 90clean so interpolation work without problems
                        activateLaser = false;
                        laser.SetActive(true);
                        direction = directions[index % directions.Length];
                        rotationStart = leftConnector.transform.rotation;
                        rotationProgress = 0f;

                    }

                    if(currDirectionChangesLeft > 0)
                    {
                        ShootRockets();
                        rotationProgress += (rotationSpeed/ 50f) * Time.deltaTime;
                        leftConnector.transform.rotation = Quaternion.Lerp(rotationStart, Quaternion.Euler(direction), rotationProgress);

                        //check if interpolation is done
                        if(rotationProgress >= 1f)
                        {
                            leftConnector.transform.rotation = Quaternion.Euler(direction); //set position to goal to make interpolation clean
                            rotationProgress = 0f; 
                            rotationStart = Quaternion.Euler(direction);
                            index++;
                            direction = directions[index % directions.Length];
                            currDirectionChangesLeft--;
                        }                       
                    }
                    else
                    {
                        laser.SetActive(false);

                        target = new Vector2(0, 30);
                        MoveTo(target);

                        if (ReachedTarget(target))
                        {
                            currDirectionChangesLeft = totalDirectionChanges;
                            activateLaser = true;                           
                            transform.rotation = Quaternion.Euler(0, 0, 0);
                            rightConnector.transform.rotation = Quaternion.Euler(0, 0, 0);
                            leftConnector.transform.rotation = Quaternion.Euler(0, 0, 0);
                            state = State.Triangle;
                            polygonCollider.enabled = false;
                            Triangles.SetActive(true);

                            float x, y, targetX, targetY;
                            for (int i = 0; i < Triangles.transform.childCount; i++)
                            {
                                x = i >= 2 ? screenBounds.x : -screenBounds.x;
                                y = i > 0 && i < 3 ? -screenBounds.y : screenBounds.y;

                                Triangles.transform.GetChild(i).position = new Vector3(x, y, 0);

                                targetX = (i + 1) % 4 >= 2 ? screenBounds.x : -screenBounds.x;
                                targetY = (i + 1) % 4 > 0 && (i + 1) % 4 < 3 ? -screenBounds.y : screenBounds.y;

                                triangleTargets[i] = new Vector2(targetX, targetY);
                            }
                        }                        
                    }
                }

                break;
            case State.Triangle:
                if(trianglePhaseTimer > 0f)
                {
                    trianglePhaseTimer -= Time.deltaTime;

                    for(int i = 0; i<Triangles.transform.childCount; i++)
                    {
                        Triangles.transform.GetChild(i).position = Vector2.MoveTowards(Triangles.transform.GetChild(i).position, triangleTargets[i], speed * Time.deltaTime);
                    }                    
                    //flip targets
                    if(Triangles.transform.GetChild(1).position == new Vector3(triangleTargets[1].x, triangleTargets[1].y, 0))
                    {
                        for(int i = 0; i < Triangles.transform.childCount; i++)
                        {
                            triangleTargets[i] = i % 2 == 0 ? new Vector2(triangleTargets[i].x, triangleTargets[i].y * -1f) : new Vector2(triangleTargets[i].x * -1f, triangleTargets[i].y);
                        }                       
                    }
                    //Shoot slow bullets
                    if(slowBulletTimer > 0f)
                    {
                        slowBulletTimer -= Time.deltaTime;
                    }
                    else
                    {
                        //Shoot
                        for (int i = 0; i < Triangles.transform.childCount; i++)
                        {
                            ObjectPoolManager.SpawnObject(slowBullet, Triangles.transform.GetChild(i).GetChild(1).position, Triangles.transform.GetChild(i).GetChild(1).rotation, ObjectPoolManager.PoolType.EnemyBullet);
                        }                           
                        slowBulletTimer = slowBulletShootDelay;
                        SoundManager.PlaySound(SoundManager.Sound.ShootingSound);
                    }
                }
                else
                {
                    Triangles.SetActive(false);
                    state = State.HomingMissiles;
                    trianglePhaseTimer = TrianglePhaseDuration;
                }
                break;
            case State.HomingMissiles:
                homingMissiles = true;
                polygonCollider.enabled = true;
                state = State.MoveToInitialPosition;
                break;
        }

        UpdateLaserPositions();
        ShootHomingMissiles();
        CheckHp();
    }



    void ShootRockets()
    {
        for(int i = 0; i<allFirePoints.Length; i++)
        {
            allFirePoints[i].transform.position = rightConnector.transform.GetChild(1).GetChild(i).transform.position;
        }


        if (rocketShootTimer > 0f)
        {
            rocketShootTimer -= Time.deltaTime;
        }
        else
        {
            //Shoot rockets here

            for (int i = 0; i < allFirePoints.Length; i++)
            {
                ObjectPoolManager.SpawnObject(rocketPrefab, allFirePoints[i].transform.position, allFirePoints[i].transform.rotation, ObjectPoolManager.PoolType.EnemyRocket);
            }
            rocketShootTimer = rocketShootDelay;
            SoundManager.PlaySound(SoundManager.Sound.RocketShootSound);
        }
    }

    void ShootHomingMissiles()
    {
        if (homingMissiles)
        {
            if(missileShootTimer > 0f)
            {
                missileShootTimer -= Time.deltaTime;
            }
            else
            {
                //Shoot
                for(int i = 0; i<3; i++)
                {
                    int firepointIndex = Random.Range(0, allFirePoints.Length);
                    ObjectPoolManager.SpawnObject(homingMissilePrefab, allFirePoints[firepointIndex].transform.position, allFirePoints[firepointIndex].transform.rotation, ObjectPoolManager.PoolType.EnemyRocket);

                }
                missileShootTimer = missileShootDelay;
                SoundManager.PlaySound(SoundManager.Sound.RocketShootSound);
            }
        }
    }

    void CheckHp()
    {
        if (!trigger)
        {
            if (currHealth <= (maxHealth / 4))
            {
                trigger = true;
                totalAmountToHeal = 3 * (maxHealth / 4);
                SoundManager.PlaySound(SoundManager.Sound.FinalBossStartHealSound);
            }
        }
        else
        {
            if (fillHpUp)
            {
                if (fillTimer > 0f)
                {
                    fillTimer -= Time.deltaTime;
                }
                else
                {
                    currHealth += fillAmountPerTick;
                    totalAmountToHeal -= fillAmountPerTick;

                    //Notice to healtbar
                    EnemyHealed();

                    fillTimer = fillDelay;
                    if (totalAmountToHeal <= 0)
                    {
                        fillHpUp = false;
                    }
                    SoundManager.PlaySound(SoundManager.Sound.FinalBossHealSound);
                }

            }
        }
    }

    void UpdateLaserPositions()
    {
        laserFirePoint.transform.position = leftConnector.transform.GetChild(2).position + (leftConnector.transform.right * offsetLaserPos.x) + (leftConnector.transform.up * offsetLaserPos.y);
        laserFirePoint.transform.rotation = leftConnector.transform.rotation * laserPointInitRotation;

        laser.transform.position = laserFirePoint.transform.position;
        laser.transform.rotation = laserFirePoint.transform.rotation;

    }


    public override void OnDestroy()
    {
        base.OnDestroy();
        if (laser != null)
        {
            ObjectPoolManager.ReturnObjectToPool(laser);
        }
    }
}
