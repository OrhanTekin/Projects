using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaserHolder : IEnemy
{

    [SerializeField] private Transform[] allFirePoints;
    [SerializeField] private GameObject laserPrefab;
    GameObject laserLeft;
    GameObject laserRight;

    private Vector2 screenBounds;
    private Vector2 target1;
    private Vector2 target2;
    private Vector2 current;

    private void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        target1 = new Vector2(0, screenBounds.y -1);
        target2 = new Vector2(0, -screenBounds.y + 1);
        current = target1;

        laserLeft = ObjectPoolManager.SpawnObject(laserPrefab, allFirePoints[0].position, allFirePoints[0].rotation, ObjectPoolManager.PoolType.Laser);
        laserRight = ObjectPoolManager.SpawnObject(laserPrefab, allFirePoints[1].position, allFirePoints[1].rotation, ObjectPoolManager.PoolType.Laser);
    }


    private void Update()
    {
        MoveTo(current);
        if (ReachedTarget(target1))
        {
            current = target2;
        }
        else if(ReachedTarget(target2))
        {
            current = target1;
        }
    }

    private void LateUpdate()
    {
        laserLeft.transform.position = allFirePoints[0].position;
        laserRight.transform.position = allFirePoints[1].position;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if(laserLeft != null && laserRight != null)
        {         
            ObjectPoolManager.ReturnObjectToPool(laserLeft);
            ObjectPoolManager.ReturnObjectToPool(laserRight);

        }       
    }

}
