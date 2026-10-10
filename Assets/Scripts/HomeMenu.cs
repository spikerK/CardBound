using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeMenu : MonoBehaviour
{
    public void PlayGame(){
        RunData.currentHP = 30;
        RunData.maxHP = 30;
        
        SceneManager.LoadScene("Level1");
    }
    public void HowToPlay(){
        SceneManager.LoadScene("HowToPlay");
    }
    public void QuitGame(){
        Application.Quit();
    }
}
