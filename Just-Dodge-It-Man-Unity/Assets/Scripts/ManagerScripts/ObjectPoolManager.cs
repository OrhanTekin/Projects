using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static List<PooledObjectInfo> ObjectPools = new List<PooledObjectInfo>();

    private GameObject objectPoolEmptyHolder;

    private static GameObject bulletsEmpty;
    private static GameObject rocketsEmpty;
    private static GameObject enemyBulletsEmpty;
    private static GameObject enemyRocketsEmpty;
    private static GameObject impactEffectEmpty;
    private static GameObject enemyLaserEmpty;

    [SerializeField] private PlayerStats stats;

    public enum PoolType
    {
        Bullet,
        Rocket,
        EnemyRocket,
        EnemyBullet,
        ImpactEffect,
        Laser,
        None
    }

    public static PoolType PoolingType;

    private void Awake()
    {
        SetupEmpties();
    }

    void SetupEmpties()
    {
        objectPoolEmptyHolder = new GameObject("Pooled Objects");

        bulletsEmpty = new GameObject("Bullets");
        bulletsEmpty.transform.SetParent(objectPoolEmptyHolder.transform);

        rocketsEmpty = new GameObject("Rockets");
        rocketsEmpty.transform.SetParent(objectPoolEmptyHolder.transform);

        enemyBulletsEmpty = new GameObject("Enemy Bullets");
        enemyBulletsEmpty.transform.SetParent(objectPoolEmptyHolder.transform);

        enemyRocketsEmpty = new GameObject("Enemy Rockets");
        enemyRocketsEmpty.transform.SetParent(objectPoolEmptyHolder.transform);

        impactEffectEmpty = new GameObject("Impact Effects");
        impactEffectEmpty.transform.SetParent(objectPoolEmptyHolder.transform);

        enemyLaserEmpty = new GameObject("Laser");
        enemyLaserEmpty.transform.SetParent(objectPoolEmptyHolder.transform);

    }


    public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPosition, Quaternion spawnRotation, PoolType poolType = PoolType.None)
    {
        PooledObjectInfo pool = ObjectPools.Find(p => p.LookupString == objectToSpawn.name);

        if(pool == null)
        {
            pool = new PooledObjectInfo() { LookupString = objectToSpawn.name };
            ObjectPools.Add(pool);
        }

        GameObject spawnableObj = pool.InactiveObjects.FirstOrDefault();

        if(spawnableObj == null)
        {
            GameObject parentObject = SetParentObject(poolType);

            spawnableObj = Instantiate(objectToSpawn, spawnPosition, spawnRotation);

            if(parentObject != null)
            {
                spawnableObj.transform.SetParent(parentObject.transform);
            }

        }
        else
        {
            spawnableObj.transform.position = spawnPosition;
            spawnableObj.transform.rotation = spawnRotation;
            pool.InactiveObjects.Remove(spawnableObj);
            spawnableObj.SetActive(true);
        }

        return spawnableObj;
    }

    public static void ReturnObjectToPool(GameObject obj)
    {
        string goName = obj.name.Substring(0, obj.name.Length - 7);
        PooledObjectInfo pool = ObjectPools.Find(p => p.LookupString == goName);

        if(pool == null)
        {
            Debug.LogWarning("Trying to release an object that is not pooled: " + obj.name);
        }
        else
        {
            obj.SetActive(false);
            pool.InactiveObjects.Add(obj);
        }
    }

    private static GameObject SetParentObject(PoolType poolType)
    {
        switch (poolType)
        {
            case PoolType.Bullet:
                return bulletsEmpty;
            case PoolType.Rocket:
                return rocketsEmpty;
            case PoolType.EnemyBullet:
                return enemyBulletsEmpty;
            case PoolType.EnemyRocket:
                return enemyRocketsEmpty;
            case PoolType.ImpactEffect:
                return impactEffectEmpty;
            case PoolType.Laser:
                return enemyLaserEmpty;
            case PoolType.None:
                return null;
            default:
                return null;
        }
    }

}


public class PooledObjectInfo
{
    public string LookupString;
    public List<GameObject> InactiveObjects = new List<GameObject>();
}