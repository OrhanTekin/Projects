using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    
    [SerializeField] private float speed = 35f;
    [SerializeField] private float damage = 20;

    [SerializeField] private float timeToLive = 2f;
    private Coroutine returnToPoolTimerCoroutine;
    //define what the bullets should be able to hit/damage;
    [SerializeField] private LayerMask bulletHitLayerMask;
    [SerializeField] private LayerMask bulletShieldLayerMask;

    [SerializeField] private float sizeX = 0.2f;
    [SerializeField] private float sizeY = 0.2f;

    [SerializeField] private Transform impactEffectPrefab;

    private static bool Piercing=false;
    private float piercingShotsHitTimer = 0.2f;
    private float hitProcTimer;

    void Start()
    {
       
    }

    private void OnEnable()
    {
        gameObject.GetComponentInChildren<TrailRenderer>().Clear();
        returnToPoolTimerCoroutine = StartCoroutine(ReturnToPoolAfterTime());
        hitProcTimer = 0f;
    }

    void Update()
    {
        transform.position += transform.up * speed * Time.deltaTime;

        RaycastHit2D hitShield = Physics2D.CapsuleCast(transform.position, new Vector2(sizeX, sizeY), CapsuleDirection2D.Vertical, 0, transform.up, 0f, bulletShieldLayerMask);
        if (hitShield)
        {
            ObjectPoolManager.SpawnObject(impactEffectPrefab.gameObject, transform.position, transform.rotation * impactEffectPrefab.rotation, ObjectPoolManager.PoolType.ImpactEffect);
            ObjectPoolManager.ReturnObjectToPool(gameObject);
            SoundManager.PlaySound(SoundManager.Sound.PlayerBulletImpactSound);
            return;
        }


        if (Piercing)
        {
            
            RaycastHit2D[] hit = Physics2D.CapsuleCastAll(transform.position, new Vector2(sizeX, sizeY), CapsuleDirection2D.Vertical, 0, transform.up, 0f, bulletHitLayerMask);
            if (hit.Length != 0)
            {            
                if (hitProcTimer > 0f)
                {
                    hitProcTimer -= Time.deltaTime;
                }
                else
                {
                    foreach(RaycastHit2D single in hit)
                    {
                        GameManager.Instance.BulletHit(damage, single.transform);                      
                    }
                    ObjectPoolManager.SpawnObject(impactEffectPrefab.gameObject, transform.position, transform.rotation * impactEffectPrefab.rotation, ObjectPoolManager.PoolType.ImpactEffect);
                    SoundManager.PlaySound(SoundManager.Sound.PlayerBulletImpactSound);
                    hitProcTimer = piercingShotsHitTimer;
                }               
            }          
        }
        else
        {
            RaycastHit2D hit = Physics2D.CapsuleCast(transform.position, new Vector2(sizeX, sizeY), CapsuleDirection2D.Vertical, 0, transform.up, 0f, bulletHitLayerMask);

            if (hit)
            {
               
                ObjectPoolManager.SpawnObject(impactEffectPrefab.gameObject, transform.position, transform.rotation * impactEffectPrefab.rotation, ObjectPoolManager.PoolType.ImpactEffect);
                GameManager.Instance.BulletHit(damage, hit.transform);
                ObjectPoolManager.ReturnObjectToPool(gameObject);
                SoundManager.PlaySound(SoundManager.Sound.PlayerBulletImpactSound);
            }        
        }
    }

    IEnumerator ReturnToPoolAfterTime()
    {
        float timer = 0f;
        while(timer < timeToLive)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }

    public static void SetPiercing(bool value)
    {
        Piercing = value;
    }

}
