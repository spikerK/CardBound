using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class Enemy : MonoBehaviour, IDropHandler
{

    public string enemyName = "Enemy";
    public int currentHP = 20;
    public int maxHP = 20;
    public TMP_Text intentText;
    public TMP_Text hpText;
    public TMP_Text nameText;
    public Camera gameCamera;
    public Vector3 intentOffSet = new Vector3(0, 1.5f, 0);

    private bool isFrozen = false;

    void LateUpdate(){
        if (gameCamera == null){
            gameCamera = Camera.main;
        }
        if (gameCamera == null){
            return;
        }

        Vector3 screenPosition = gameCamera.WorldToScreenPoint(transform.position + intentOffSet);
        if (intentText != null){
            intentText.transform.position = screenPosition;
        }
        if (hpText != null){
            hpText.transform.position = screenPosition + new Vector3(0, 30, 0);
        }
        if (nameText != null){
            nameText.transform.position = screenPosition + new Vector3(0, 60, 0);
        }
    }
    
    public enum EnemyType{
        Goblin, Archer, GoblinKing
    }

    public enum EnemyIntent{
        Attack, Strengthen, Shoot, Reload, KingsStrike, RoyalCommand
    }

    public EnemyType enemyType;
    public EnemyIntent currentIntent;

    public int power = 0;

    private bool archerLoaded = true;

    public void PrepareIntent(){
        if (enemyType == EnemyType.Goblin){
            if (Random.value < 0.5f){
                currentIntent = EnemyIntent.Attack;
            }
            else{
                currentIntent = EnemyIntent.Strengthen;
            }
        }
        else if (enemyType == EnemyType.Archer){
            if (archerLoaded == true){
                currentIntent = EnemyIntent.Shoot;
            }
            else{
                currentIntent = EnemyIntent.Reload;
            }
        }
        else if(enemyType == EnemyType.GoblinKing){
            if(Random.value < 0.7f){
                currentIntent = EnemyIntent.KingsStrike;
            }
            else{
                currentIntent = EnemyIntent.RoyalCommand;
            }
        }

        Debug.Log(enemyName + " intends to: " + currentIntent);
        UpdateIntentText();
    }

    public void TakeTurn(Player player){
        if (isFrozen){
            isFrozen = false;
            Debug.Log(enemyName + " is frozen and skips its turn.");
            PrepareIntent();
            return;
        }
        
        switch (currentIntent){
            case EnemyIntent.Attack:
                int damage = 2 + power;
                player.TakeDamage(damage);
                Debug.Log(enemyName + " attacks for "+ damage + " damage.");
                break;
            case EnemyIntent.Strengthen:
                power++;
                Debug.Log(enemyName + " gains 1 power. Current Power: " + power);
                break;
            case EnemyIntent.Shoot:
                player.currentHP = Mathf.Max(0, player.currentHP - 3);
                Debug.Log(enemyName + " shoots for 3 damage, ignoring shield.");
                archerLoaded = false;
                break;
            case EnemyIntent.Reload:
                archerLoaded = true;
                Debug.Log("Archer has reloaded.");
                break;
            case EnemyIntent.KingsStrike:
                int kingDamage = 8 + power;
                player.TakeDamage(kingDamage);
                Debug.Log(enemyName + " uses King's Strike for " + kingDamage + " damage.");
                break;
            case EnemyIntent.RoyalCommand:
                power += 3;
                Debug.Log(enemyName + " uses Royal Command! Power is now " + power + ".");
                break;
        }

        PrepareIntent();
    }

    public void TakeDamage(int damage){
        currentHP -= damage;

        if (currentHP < 0){
            currentHP = 0;
        }

        Debug.Log(enemyName + " takes " + damage + "  damage. HP remaining: " + currentHP);
        UpdateHPText();
        if (currentHP == 0){
            UpdateHPText();
            gameObject.SetActive(false);

            if (intentText != null){
                intentText.gameObject.SetActive(false);
            }
            if (hpText != null){
                hpText.gameObject.SetActive(false);
            }
            if (nameText != null){
                nameText.gameObject.SetActive(false);
            }
            Debug.Log(enemyName + " defeated!");
        }
    }

    public void OnDrop(PointerEventData eventData){

        if (eventData.pointerDrag == null){
            return;
        }

        CardDrag draggedCard = eventData.pointerDrag.GetComponent<CardDrag>();

        if (draggedCard == null){
            return;
        }

        Card card = draggedCard.GetComponent<Card>();

        if (card == null)
        {
            return;
        }

        if (card.cardType == Card.CardType.Attack){
            Debug.Log("Used " + card.cardName + " on " + enemyName);
        }
    }

    public void Freeze(){
        isFrozen = true;
        Debug.Log(enemyName + " is frozen.");
    }

    void UpdateIntentText(){
        if (intentText == null){
            return;
        }

        if (currentIntent == EnemyIntent.Attack){
            intentText.text = "Attack: " + (2 + power);
        }
        else if (currentIntent == EnemyIntent.Strengthen){
            intentText.text = "Strengthen: +1 Power";
        }
        else if (currentIntent == EnemyIntent.Shoot){
            intentText.text = "Shoot: 3 (Piercing)";
        }
        else if (currentIntent == EnemyIntent.Reload){
            intentText.text = "Reload";
        }
        else if (currentIntent == EnemyIntent.KingsStrike){
            intentText.text = "King's Strike: " + (8 + power);
        }
        else if (currentIntent == EnemyIntent.RoyalCommand){
            intentText.text = "Royal Command: +3 Power";
        }
    }

    void UpdateHPText(){
        if (hpText != null){
            hpText.text = "HP: " + currentHP + " / " + maxHP;
        }
    }

    void UpdateNameText(){
        if (nameText != null){
            nameText.text = enemyName;
        }
    }
    void Start(){
        UpdateNameText();
        UpdateHPText();
        PrepareIntent();
    }
}
