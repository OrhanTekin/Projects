using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using TMPro;

public class CanvasUI : MonoBehaviour
{

    public static CanvasUI Instance { get; private set; }

    [SerializeField] private Transform powerupMenu;
    [SerializeField] private List<Transform> buttons;

    [SerializeField] private List<PowerupSO> powerups;
    [SerializeField] private List<PowerupSO> weakerups;

    [SerializeField] private List<PowerupSO> currentPowerupsLeft;  //remove serialize here at the end
    [SerializeField] private PowerupSO unlockAllPowerup;
    private List<PowerupSO> currentThree;

    private List<PowerupSO> currentList;

    private bool executeOncePerWave=true;

    [SerializeField] private PlayerStats stats;
    [SerializeField] private bool debug;


    //event
    public event EventHandler OnPowerupSelected;


    private void Awake()
    {
        Instance = this;
        currentPowerupsLeft = powerups.ToList();

    }

    private void Start()
    {
        if(WaveManager.Instance != null)
        {
            WaveManager.Instance.OnWaveOver += WaveManager_OnWaveOver;
        }
        
    }



    private void WaveManager_OnWaveOver(object sender, WaveManager.OnWaveOverEventArgs e)
    {
        if (executeOncePerWave)
        {
            currentThree = new List<PowerupSO>();

            if(e.finishedWave < 15) {
                if (e.finishedWave % 2 == 1)
                {
                    //every second wave take a powerup
                    currentList = currentPowerupsLeft;
                }
                else
                {
                    currentList = weakerups;
                }

                int size = currentList.Count;

                for (int i = 0; i < 3; i++)
                {
                    if (currentList.Count < 3)
                    {
                        Debug.LogWarning("There are not three powerups available"); // this should never happen
                        break;
                    }

                    int selectIndex = UnityEngine.Random.Range(0, size);
                    while (currentThree.Contains(currentList[selectIndex]))
                    {
                        selectIndex = UnityEngine.Random.Range(0, size);
                    }
                    currentThree.Add(currentList[selectIndex]);
                }
            }
            else
            {
                //after finishing wave 15 and after
                if(e.finishedWave == 15)
                {
                    currentThree.Add(unlockAllPowerup);
                    currentThree.Add(unlockAllPowerup);
                    currentThree.Add(unlockAllPowerup);
                }
                else
                {
                    currentThree.Add(weakerups[0]);
                    currentThree.Add(weakerups[1]);
                    currentThree.Add(weakerups[2]);
                }
                
            }

            

            SetButtonText();

            if (!debug)
            {
                powerupMenu.gameObject.SetActive(true);
            }         
            executeOncePerWave = false;
        }
        
    }

    void SetButtonText()
    {
        buttons[0].GetChild(0).GetComponent<TextMeshProUGUI>().text = currentThree[0].description;
        buttons[1].GetChild(0).GetComponent<TextMeshProUGUI>().text = currentThree[1].description;
        buttons[2].GetChild(0).GetComponent<TextMeshProUGUI>().text = currentThree[2].description;
    }

    public void PowerupSelected(int number)
    {
        powerupMenu.gameObject.SetActive(false);

        if (!currentThree[number].initInfinitely)
        {
            //selected item is not selectible infinitely so decrease currAmount
            currentThree[number].CurrAmount -= 1;
            

            if(currentThree[number].CurrAmount == 0)
            {
                if (currentList.Contains(currentThree[number]))
                {
                    currentList.Remove(currentThree[number]);
                }

            }          
        }
        

        //signal to stats which powerup player chose.
        stats.SetStat(currentThree[number]);

        //Signal to wave manager to start next wave
        OnPowerupSelected.Invoke(this, EventArgs.Empty);

        executeOncePerWave = true;
    }

    private void OnDestroy()
    {
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.OnWaveOver -= WaveManager_OnWaveOver;
        }
    }
}
