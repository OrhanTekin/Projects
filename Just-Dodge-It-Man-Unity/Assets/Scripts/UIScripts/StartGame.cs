using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public void SwitchToGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    
}
