using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; set; }
    [SerializeField] private EnemySO[] enemiesSO;
    [SerializeField] private EnemySO[] allBossesSO;
    [SerializeField] private List<EnemySO> enemiesToSpawn;

    public int wave;
    private int waveValue;
    [SerializeField] private int waveValueMultiplier = 10;
    private int lateGameMultiplier;

    [SerializeField] private float afterWaveDelayTimerMax=1f;
    private float afterWaveDelayTimer;
    

    public static int aliveEnemies;


    [SerializeField] private bool debug;

    //events
    public event EventHandler<OnWaveChangedEventArgs> OnWaveChanged;
    public class OnWaveChangedEventArgs : EventArgs
    {
        public int nextWave;
    }

    public event EventHandler<OnWaveOverEventArgs> OnWaveOver;
    public class OnWaveOverEventArgs: EventArgs
    {
        public int finishedWave;
    }

    public event EventHandler<OnEnemyAmountChangedEventArgs> OnEnemyAmountChanged;
    public class OnEnemyAmountChangedEventArgs: EventArgs
    {
        public int newAmount;
    }


    void Awake()
    {
        Instance = this;

        aliveEnemies = 0;
        wave = 1;
    }

    void Start()
    {
        if(CanvasUI.Instance != null)
        {
            CanvasUI.Instance.OnPowerupSelected += CanvasUI_OnPowerupSelected;
        }
        

        afterWaveDelayTimer = afterWaveDelayTimerMax;
        lateGameMultiplier = 1;

        /*
        OnWaveChanged?.Invoke(this, new OnWaveChangedEventArgs
        {
            nextWave = wave
        });
        */

        if (!debug)
        {
            SelectEnemiesToSpawn();
        }      
    }

    private void CanvasUI_OnPowerupSelected(object sender, System.EventArgs e)
    {
        //start wave
        if (!debug)
        {
            IncreaseWave();
            if(wave == 8 || wave == 12 || wave == 15)
            {
                SelectBossToSpawn();
            }
            else
            {
                if(wave > 15)
                {
                    lateGameMultiplier++;
                }
                SelectEnemiesToSpawn();
            }

        }
    }

    private void Update()
    {
        if (waveValue==0 && aliveEnemies == 0)
        {
            //wave is over

            if(afterWaveDelayTimer > 0)
            {
                afterWaveDelayTimer -= Time.deltaTime;
            }
            else
            {
                //remember: invoked multiple times!
                OnWaveOver.Invoke(this, new OnWaveOverEventArgs
                {
                    finishedWave = wave
                });                    
            }

        }

        //Should only happen in debug.
        if(waveValue == 0 && aliveEnemies < 0)
        {
            Debug.Log("AliveEnemies: " + aliveEnemies);
        }
    }

    void SelectBossToSpawn()
    {
        afterWaveDelayTimer = afterWaveDelayTimerMax;

        List<EnemySO> listOfEnemies = new List<EnemySO>();
        if(wave == 8)
        {
            listOfEnemies.Add(allBossesSO[0]);
            aliveEnemies++;
        }else if(wave == 12)
        {
            listOfEnemies.Add(allBossesSO[1]);
            aliveEnemies++;
        }
        else
        {
            //final boss
            listOfEnemies.Add(allBossesSO[2]);
            aliveEnemies++;
        }



        enemiesToSpawn.Clear();
        enemiesToSpawn = listOfEnemies;

        StartCoroutine(GenerateWave(listOfEnemies.ToList()));
    }

    void SelectEnemiesToSpawn()
    {
        waveValue = wave * waveValueMultiplier * lateGameMultiplier;
        afterWaveDelayTimer = afterWaveDelayTimerMax;

        List<EnemySO> listOfEnemies = new List<EnemySO>();
        int i = 0;
        int rand;
        while(waveValue > 0)
        {
            if (i >= wave * 100)
            {
                //built in break conditions
                Debug.Log("Should never happen: Couldn't select enemies");
                break;
            }

            rand = UnityEngine.Random.Range(0, enemiesSO.Length);
            int startsAtWave = enemiesSO[rand].firstWave;
            if(startsAtWave > wave || enemiesSO[rand].cost > waveValue)
            {
                i++;
                continue;
            }
            else
            {
                listOfEnemies.Add(enemiesSO[rand]);
                waveValue -= enemiesSO[rand].cost;
                aliveEnemies++;
                i++;
            }
             
        }
        enemiesToSpawn.Clear();
        enemiesToSpawn = listOfEnemies;

        StartCoroutine(GenerateWave(listOfEnemies.ToList()));
    }

    IEnumerator GenerateWave(List<EnemySO> spawnList)
    {

        StartCoroutine(ResetEnemiesAlive());

        if(wave <= 15)
        {
            spawnList.Sort(delegate (EnemySO x, EnemySO y)
            {
                if (x.cost < y.cost) return -1;
                else if (x.cost > y.cost) return 1;
                else return 0;
            });

            foreach (EnemySO enemy in spawnList)
            {
                float delay = enemy.spawnDelay;
                SpawnEnemy(enemy);
                yield return new WaitForSeconds(delay);

            }
        }
        else
        {
            foreach (EnemySO enemy in spawnList)
            {
                SpawnEnemy(enemy);
                yield return new WaitForSeconds(0.3f);
            }
        }
        
    }

    IEnumerator ResetEnemiesAlive()
    {
        yield return null;
        OnEnemyAmountChanged?.Invoke(this, new OnEnemyAmountChangedEventArgs
        {
            newAmount = aliveEnemies
        });
    }

    void SpawnEnemy(EnemySO enemySO)
    {
        //spawn a single enemy;   
        Instantiate(enemySO.prefab, ChooseBetweenSpawnPoints(enemySO), Quaternion.identity);
    }

    public void DestroyEnemy()
    {
        aliveEnemies--;
        OnEnemyAmountChanged?.Invoke(this, new OnEnemyAmountChangedEventArgs
        {
            newAmount = aliveEnemies
        });
    }

    Vector3 ChooseBetweenSpawnPoints(EnemySO enemySo) {
        int amount = enemySo.spawnPoints.Length;
        int choose = UnityEngine.Random.Range(0, amount);

        return enemySo.spawnPoints[choose];
    }

    void IncreaseWave()
    {
        wave++;
        OnWaveChanged?.Invoke(this, new OnWaveChangedEventArgs
        {
            nextWave = wave
        });
    }

    void OnDestroy()
    {
        if (CanvasUI.Instance != null)
        {
            CanvasUI.Instance.OnPowerupSelected -= CanvasUI_OnPowerupSelected;
        }
    }

}
