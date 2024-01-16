using System;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    public static Player Instance { get; private set; }

    [SerializeField] private GameObject playerSprite;
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private GameObject[] allFirePoints;
    [SerializeField] private BulletTypeSO[] allBulletTypesSO;
    private BulletTypeSO currentProjectile;

    [SerializeField] private PlayerStats stats;


    //front gun
    private float shootTimer;


    private float rocketShootTimer;
    //rockets

    //wider bullets?, piercing shots?

  
    //screen boundary
    private Vector2 screenBounds;
    private float playerWidth;
    private float playerHeight;
    private float posX;
    private float posY;
    //Player rotates with mouse
    private Vector3 mousePosition;

    //enemy hit radius
    private float enemyHitRadius = 0.25f;
    //dmg tick rate when enemy touches player
    private float nextActionTime = 0.0f;


    void Awake(){
        //set singleton;
        Instance = this;

        //screen bound
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        playerWidth = playerSprite.GetComponent<SpriteRenderer>().bounds.size.x / 2;
        playerHeight = playerSprite.GetComponent<SpriteRenderer>().bounds.size.y / 2;

        //start with normal bullet
        currentProjectile = allBulletTypesSO[0];
        shootTimer = stats.ShootDelayTime;
        rocketShootTimer = stats.RocketShootDelayTime;
    }

    void Update()
    {
        
        HandlePlayerMovement();

        HandlePlayerDirection();

        HandlePlayerCollision();

        if(shootTimer > 0f)
        {
            shootTimer -= Time.deltaTime;
        }
        else
        {
            //debug shooting: comment out two lines below
            if (GameInput.Instance.GetTestFire() == 1)
            {
                //SpawnBullets();
                //SpawnRockets();
                //shootTimer = stats.shootDelayTime;
                //rocketShootTimer = stats.rocketShootDelayTime;
            }
            //Shoot Gun continously

            SpawnBullets();
            shootTimer = stats.ShootDelayTime;
        }

        if (stats.Rockets)
        {
            if (rocketShootTimer > 0f)
            {
                rocketShootTimer -= Time.deltaTime;
            }
            else
            {
                SpawnRockets();
                rocketShootTimer = stats.RocketShootDelayTime;
            }
        }
        CheckHealth();
    }

    //spawn bullet
    void SpawnBullets(){
        ObjectPoolManager.SpawnObject(currentProjectile.prefab.gameObject, allFirePoints[0].transform.position, allFirePoints[0].transform.rotation, ObjectPoolManager.PoolType.Bullet);


        //unlocked guns
        if (stats.ExtraGuns)
        {
            ObjectPoolManager.SpawnObject(currentProjectile.prefab.gameObject, allFirePoints[1].transform.position, allFirePoints[1].transform.rotation, ObjectPoolManager.PoolType.Bullet);
            ObjectPoolManager.SpawnObject(currentProjectile.prefab.gameObject, allFirePoints[2].transform.position, allFirePoints[2].transform.rotation, ObjectPoolManager.PoolType.Bullet);
        } 
    }

    void SpawnRockets()
    {
        ObjectPoolManager.SpawnObject(allBulletTypesSO[2].prefab.gameObject, allFirePoints[3].transform.position, allFirePoints[3].transform.rotation, ObjectPoolManager.PoolType.Rocket);
        ObjectPoolManager.SpawnObject(allBulletTypesSO[2].prefab.gameObject, allFirePoints[4].transform.position, allFirePoints[4].transform.rotation, ObjectPoolManager.PoolType.Rocket);
    }

    void HandlePlayerMovement(){
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
        //convert to vector3
        Vector3 moveDir = new Vector3(inputVector.x, inputVector.y, transform.position.z); 

        transform.position += moveDir * stats.Movespeed * Time.deltaTime;

        
        //dont run out of the screen
        posX = Mathf.Clamp(transform.position.x, screenBounds.x * -1 + playerWidth, screenBounds.x - playerWidth);
        posY = Mathf.Clamp(transform.position.y, screenBounds.y * -1 + playerHeight, screenBounds.y - playerHeight);
        transform.position = new Vector3(posX,posY, transform.position.z);
    }

    void HandlePlayerDirection(){
        //turn to mouse direction
        mousePosition = GameInput.Instance.GetMousePosition();
        Vector2 mousePosInWorld = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector2 direction = new Vector2(
            mousePosInWorld.x - transform.position.x,
            mousePosInWorld.y - transform.position.y
        );

        transform.up = direction;
    }

    void HandlePlayerCollision()
    {
        //check if enemy is touching player or an enemy projectile hit player
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, enemyHitRadius, transform.up, 0, enemyLayerMask);


        if (Time.time > nextActionTime)
        {
            nextActionTime += 0.01f;
            if (hit)
            {
                stats.CurrentHealth -= 5;
            }
        }
    }

    void CheckHealth()
    {
        if(stats.CurrentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        stats.CurrentHealth -= (int) damage;  
    }

    public void RedBullet()
    {
        currentProjectile = allBulletTypesSO[1];
    }
}
