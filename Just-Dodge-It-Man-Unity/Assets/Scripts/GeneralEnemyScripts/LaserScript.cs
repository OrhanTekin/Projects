using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LaserScript : MonoBehaviour
{
    [SerializeField] public float defaultDistance = 100;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LayerMask layersToHit;
    [SerializeField] private int damage;
    [SerializeField] private Transform impactEffectPrefab;

    private void OnEnable()
    {
        lineRenderer.enabled = false;
        StartCoroutine(SetLineVisible());
    }

    IEnumerator SetLineVisible()
    {
        //On enabled is called before deactivating the line in objectpoolManager -> so Wait one frame with coroutine to fix both issues
        yield return null;
        lineRenderer.enabled = true;
    }


    private void Update()
    {
        ShootLaser();
        SoundManager.PlaySound(SoundManager.Sound.LaserSound);
    }


    void ShootLaser()
    {
        if(Physics2D.Raycast(transform.position, transform.up, defaultDistance, layersToHit))
        {
            //player hit
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, defaultDistance, layersToHit);
            Draw2DRay(transform.position, hit.point);
            hit.transform.GetComponent<IDamageable>().TakeDamage(damage);
            ObjectPoolManager.SpawnObject(impactEffectPrefab.gameObject, hit.point, transform.rotation * impactEffectPrefab.rotation, ObjectPoolManager.PoolType.ImpactEffect);
        }
        else
        {
            Draw2DRay(transform.position, transform.position + (transform.up * defaultDistance));
        }
    }


    void Draw2DRay(Vector2 startPos, Vector2 endPos)
    {
        lineRenderer.SetPositions(new Vector3[] { startPos, endPos });
    }
}
