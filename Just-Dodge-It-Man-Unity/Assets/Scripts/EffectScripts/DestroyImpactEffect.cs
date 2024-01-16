using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyImpactEffect : MonoBehaviour
{

    private Coroutine returnToPoolTimerCoroutine;

    void Start()
    {
        //Destroy(gameObject, .2f);
    }

    private void OnEnable()
    {
        returnToPoolTimerCoroutine = StartCoroutine(ReturnToPoolAfterTime());
    }

    IEnumerator ReturnToPoolAfterTime()
    {
        float timer = 0f;
        while (timer < 0.2f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }
}
