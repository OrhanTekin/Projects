using System;
using UnityEngine;

public abstract class IEnemy : MonoBehaviour, IDamageable
{
    public int maxHealth;
    public float speed;
    protected int currHealth;

    public event EventHandler<OnEnemyHealthChangedEventArgs> OnEnemyHealthChanged;
    public class OnEnemyHealthChangedEventArgs : EventArgs
    {
        public float healthNormalized;
    }

    void Awake()
    {
        if (WaveManager.Instance != null)
        {
            int additionalHealth = (int) (0.5f * (WaveManager.Instance.wave-1) * maxHealth); 
            maxHealth += additionalHealth;
            currHealth = maxHealth;
        }
        else
        {
            Debug.Log("This should only happen in debug"); //Awake of wave manager inits instance -> both are called on the same time when you drag an obj in the scene
            currHealth = maxHealth;
        }
    }

    public void TakeDamage(float damage)
    {
        currHealth -= (int)damage;
        if (currHealth <= 0)
        {
            Destroy(gameObject);

        }

        OnEnemyHealthChanged?.Invoke(this, new OnEnemyHealthChangedEventArgs
        {
            healthNormalized = (float) currHealth / maxHealth
        });

    }

    protected void MoveTo(Vector2 targetPosition)
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }


    protected bool ReachedTarget(Vector2 targetPosition)
    {
        return transform.position == new Vector3(targetPosition.x, targetPosition.y, transform.position.z);
    }
    public void EnemyHealed()
    {
        OnEnemyHealthChanged?.Invoke(this, new OnEnemyHealthChangedEventArgs
        {
            healthNormalized = (float) currHealth / maxHealth
        });
    }

    public virtual void OnDestroy()
    {
        WaveManager.Instance.DestroyEnemy();
    }

}
