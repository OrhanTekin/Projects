using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class EnemyHomingMissile : MonoBehaviour, IDamageable
{
    [SerializeField] private float health = 20f;
    [SerializeField] private float speed = 80f;
    [SerializeField] private float damage = 80f;

    [SerializeField] private float timeToLive = 2f;
    private Coroutine returnToPoolTimerCoroutine;
    //define what the bullets should be able to hit/damage;
    [SerializeField] private LayerMask bulletHitLayerMask;
    [SerializeField] private LayerMask bulletShieldLayerMask;

    [SerializeField] private float sizeX = 0.3f;
    [SerializeField] private float sizeY = 0.7f;

    [SerializeField] private Transform impactEffectPrefab;
    Player player;
    Vector3 direction;

    private void Start()
    {
        if (Player.Instance != null)
        {
            player = Player.Instance;
        }
        else
        {
            Debug.Log("Player instance is null");
        }
    }

    private void OnEnable()
    {
        gameObject.GetComponentInChildren<TrailRenderer>().Clear();
        returnToPoolTimerCoroutine = StartCoroutine(ReturnToPoolAfterTime());
    }

    private void Update()
    {
        if(player != null)
        {
            direction = new Vector3(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y, 0);

            transform.up = direction;
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        }
        else
        {
            transform.position += transform.up * speed * Time.deltaTime;
        }
        
        RaycastHit2D hit = Physics2D.CapsuleCast(transform.position, new Vector2(sizeX, sizeY), CapsuleDirection2D.Vertical, 0, transform.up, 0f, bulletHitLayerMask);
        if (hit)
        {
            ObjectPoolManager.SpawnObject(impactEffectPrefab.gameObject, transform.position, transform.rotation * impactEffectPrefab.rotation, ObjectPoolManager.PoolType.ImpactEffect);
            GameManager.Instance.BulletHit(damage, hit.transform);
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

    public void TakeDamage(float damage)
    {
        health -= (int)damage;
        if(health <= 0)
        {
            ObjectPoolManager.ReturnObjectToPool(gameObject);
        }
    }
}
