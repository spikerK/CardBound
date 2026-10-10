using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour, IDropHandler
{
    public int maxHP = 30;
    public int currentHP = 30;
    public int startingEnergy = 3;
    public int currentEnergy = 3;
    public int currentBlock = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        maxHP = RunData.maxHP;
        currentHP = RunData.currentHP;
        currentEnergy = startingEnergy;
    }

    public void ResetEnergy(){
        currentEnergy = startingEnergy;
    }

    public bool SpendEnergy(int amount){
        if (amount > currentEnergy){
            Debug.Log("Not enough energy!");
            return false;
        }
        else{
            currentEnergy -= amount;
            return true;
        }
    }

    public void GainEnergy(int amount){
        currentEnergy += amount;
    }

    public void TakeDamage(int damage){
        int blockedDamage = Mathf.Min(currentBlock, damage);
        currentBlock -= blockedDamage;
        damage -= blockedDamage;

        currentHP -= damage;

        if (currentHP < 0){
            currentHP = 0;
        }

        Debug.Log("Player HP: " + currentHP);
        RunData.currentHP = currentHP;

        // if (RunData.currentHP <= 0){
        //     SceneManager.LoadScene("Death");
        // }
    }

    public void GainBlock(int amount){
        currentBlock += amount;
        Debug.Log("Player Block: " + currentBlock);
    }

    public void Heal(int amount){
        currentHP += amount;

        if (currentHP > maxHP){
            currentHP = maxHP;
        }

        RunData.currentHP = currentHP;
    }

    public void OnDrop(PointerEventData eventData){
        if (eventData.pointerDrag == null){
            return;
        }

        Card card = eventData.pointerDrag.GetComponent<Card>();

        if (card == null){
            return;
        }

        if (card.cardType != Card.CardType.Skill){
            return;
        }

        if (!SpendEnergy(card.energyCost)){
            return;
        }

        if (AudioManager.Instance != null){
            AudioManager.Instance.PlayCard(card.cardName);
        }

        if (card.cardName == "Block"){
            GainBlock(card.value);
            Debug.Log("Player Block. Gained " + card.value + " block.");
        }
        else if(card.cardName == "Focus"){
            GainEnergy(card.value);
            Debug.Log("Played Focus. Gained " + card.value + " energy.");
        }

        FindAnyObjectByType<DeckManager>().DiscardCard(card);

    }

    public void PlaySkill(Card card){
        if (card == null || card.cardType != Card.CardType.Skill){
            return;
        }
        if (card.cardName != "Block" && card.cardName != "Focus" && card.cardName != "Heal"){
            return;
        }
        if (!SpendEnergy(card.energyCost)){
            return;
        }

        if (AudioManager.Instance != null){
            AudioManager.Instance.PlayCard(card.cardName);
        }
        
        if (card.cardName == "Block"){
            GainBlock(card.value);
            Debug.Log("Player block. Gained " + card.value + " block.");
        }
        else if (card.cardName == "Focus"){
            GainEnergy(card.value);
            Debug.Log("Played focus. Gained " + card.value + " energy.");
        }
        else if (card.cardName == "Heal"){
            Heal(3);
            Debug.Log("Played heal. Restored 3 HP.");
        }

        FindAnyObjectByType<DeckManager>().DiscardCard(card);
    }

}
