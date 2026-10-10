using UnityEngine;
using UnityEngine.SceneManagement;

public class RewardManager : MonoBehaviour
{
    public DeckManager deckManager;
    public GameObject rewardPanel;

    public void ChooseReward(string rewardName){
        deckManager.AddRewardCard(rewardName);
        deckManager.SaveDeck();

        if (rewardPanel != null){
            rewardPanel.SetActive(false);
        }

        Debug.Log("Reward selected: " + rewardName + ".");

        SceneManager.LoadScene("Boss Level");
    }

    public void ShowRewards(){
        if (rewardPanel != null){
            rewardPanel.SetActive(true);
        }
    }
}
