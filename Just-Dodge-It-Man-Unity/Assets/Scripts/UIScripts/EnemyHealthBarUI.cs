using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBarUI : MonoBehaviour
{

    [SerializeField] private IEnemy enemy;
    [SerializeField] private Image barImage;

    [SerializeField] private float healthBarOffset;
    [SerializeField] private bool useAlternativeSprite;
    [SerializeField] private Transform rotatingSprite;

    private void Start()
    {
        enemy.OnEnemyHealthChanged += Enemy_OnEnemyHealthChanged;
        barImage.fillAmount = 1f;
        Hide();
    }

    private void Enemy_OnEnemyHealthChanged(object sender, IEnemy.OnEnemyHealthChangedEventArgs e)
    {
        barImage.fillAmount = e.healthNormalized;

        if(e.healthNormalized == 0f || e.healthNormalized == 1f)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }


    private void Update()
    {
        transform.eulerAngles = new Vector3(0, 0, 0);
        if (!useAlternativeSprite)
        {
            transform.position = new Vector3(enemy.transform.position.x, enemy.transform.position.y + healthBarOffset, 0);
        }else
        {
            transform.position = new Vector3(rotatingSprite.transform.position.x, rotatingSprite.transform.position.y + healthBarOffset, 0);
        }
        
        
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
