using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public Player player;
    public TMP_Text hpText;
    public TMP_Text energyText;
    public TMP_Text blockText;

    void Update(){
        if (player == null){
            return;
        }

        hpText.text = "HP: " + player.currentHP;
        energyText.text = "Energy: " + player.currentEnergy;
        blockText.text = "Block " + player.currentBlock;
    }
}
