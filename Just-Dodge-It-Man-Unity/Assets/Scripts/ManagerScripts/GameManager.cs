using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PowerupSO[] allPowerups;

    [SerializeField] private GameObject retryMenu;
    [SerializeField] private GameObject pauseMenu;

    private bool gamePaused;

    private void Awake()
    {
        Instance = this;
        InitGame();
        gamePaused = false;
    }

    private void Start()
    {
        playerStats.OnHealthChanged += PlayerStats_OnHealthChanged;
        GameInput.Instance.OnPauseToggled += GameInput_OnPauseToggled;
    }

    private void GameInput_OnPauseToggled(object sender, System.EventArgs e)
    {
        gamePaused = !gamePaused;

        if (gamePaused)
        {
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
        }
    }

    private void PlayerStats_OnHealthChanged(object sender, PlayerStats.OnHealthChangedEventArgs e)
    {
        if(e.health <= 0)
        {
            //End Game - Player is dead
            GameOver();
        }
    }

    void InitGame()
    {
        playerStats.InitDefaults();
        foreach(PowerupSO powerup in allPowerups)
        {
            powerup.InitDefault();
        }
        SoundManager.Initialize();
    }

    void GameOver()
    {
        retryMenu.SetActive(true);
        //Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        retryMenu.SetActive(false);
        //Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BulletHit(float bulletDamage, Transform target)
    {
        
        if(target.GetComponent<Player>() != null)
        {
            target.GetComponent<IDamageable>().TakeDamage(bulletDamage);
        }
        else
        {
            target.GetComponent<IDamageable>().TakeDamage(bulletDamage * playerStats.DamageMultiplier);
        }
        
    }

    private void OnDestroy()
    {
        playerStats.OnHealthChanged -= PlayerStats_OnHealthChanged;
    }



    public SoundAudioClip[] soundAudioClips;

    [System.Serializable]
    public class SoundAudioClip
    {
        public SoundManager.Sound sound;
        public AudioClip audioClip;
    }
}
