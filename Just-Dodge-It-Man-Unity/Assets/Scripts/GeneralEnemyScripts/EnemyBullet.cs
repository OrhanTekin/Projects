using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
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
    [SerializeField] private SoundManager.Sound sound;

    private void OnEnable()
    {
        gameObject.GetComponentInChildren<TrailRenderer>().Clear();
        returnToPoolTimerCoroutine = StartCoroutine(ReturnToPoolAfterTime());      
    }

    private void Update()
    {
        transform.position += transform.up * speed * Time.deltaTime;

        RaycastHit2D hit = Physics2D.CapsuleCast(transform.position, new Vector2(sizeX, sizeY), CapsuleDirection2D.Vertical, 0, transform.up, 0f, bulletHitLayerMask);
        if (hit)
        {         
            ObjectPoolManager.SpawnObject(impactEffectPrefab.gameObject, transform.position, transform.rotation * impactEffectPrefab.rotation, ObjectPoolManager.PoolType.ImpactEffect);
            GameManager.Instance.BulletHit(damage, hit.transform);
            SoundManager.PlaySound(sound);
            SoundManager.PlaySound(SoundManager.Sound.EnemyBulletImpactSound);
            ObjectPoolManager.ReturnObjectToPool(gameObject);
        }

        RaycastHit2D hitShield = Physics2D.CapsuleCast(transform.position, new Vector2(sizeX, sizeY), CapsuleDirection2D.Vertical, 0, transform.up, 0f, bulletShieldLayerMask);
        if (hitShield)
        {      
            ObjectPoolManager.SpawnObject(impactEffectPrefab.gameObject, transform.position, transform.rotation * impactEffectPrefab.rotation, ObjectPoolManager.PoolType.ImpactEffect);
            ObjectPoolManager.ReturnObjectToPool(gameObject);
        }
    }

    IEnumerator ReturnToPoolAfterTime()
    {
        float timer = 0f;
        while (timer < timeToLive)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }
}
