using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class EnemyTarget : MonoBehaviour, IDropHandler
{
    public DeckManager deckManager;
    public Enemy targetEnemy;
    public Camera gameCamera;
    public Player player;

    private RectTransform rectTransform;
    private RectTransform canvasRect;

    void Awake(){
        rectTransform = GetComponent<RectTransform>();

        Canvas canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();

        if (gameCamera == null){
            gameCamera = Camera.main;
        }
    }

    void LateUpdate(){
        if (targetEnemy == null || gameCamera == null){
            return;
        }

        Vector3 screenPosition = gameCamera.WorldToScreenPoint(targetEnemy.transform.position);

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, null, out Vector2 localPosition)){
            rectTransform.anchoredPosition = localPosition;
        }
    }

    public void OnDrop(PointerEventData eventData){
        if (eventData.pointerDrag == null || targetEnemy == null || player == null){
            return;
        }

        Card card = eventData.pointerDrag.GetComponent<Card>();
         
        if (card == null){
            return;
        }

        bool isAttack = card.cardType == Card.CardType.Attack;
        bool isStarburst = card.cardName == "Starburst Stream";
        bool isFreeze = card.cardType == Card.CardType.Skill && card.cardName == "Freeze";

        if (!isAttack && !isFreeze){
            return;
        }

        if (!player.SpendEnergy(card.energyCost)){
            return;
        }

        if (AudioManager.Instance != null){
            AudioManager.Instance.PlayCard(card.cardName);
        }
        
        if(isAttack){
            if (isStarburst){
                int dmg = 4;
                for (int i = 0; i < 3; i++){
                    dmg = 4;
                    if (Random.value < 0.5f){
                        dmg = 8;
                        Debug.Log("CRITICAL! Starburst Stream hit #" + (i + 1) + " hits for 8 damage");
                    }
                    else{
                        Debug.Log("Starburst Stream hit #" + (i + 1) + " hit for 4 damage.");
                    }
                    targetEnemy.TakeDamage(dmg);

                    if(!targetEnemy.gameObject.activeInHierarchy){
                        break;
                    }
                }
            }
            else{
                targetEnemy.TakeDamage(card.value);
            }
        }
        else if (isFreeze){
            targetEnemy.Freeze();
        }

        deckManager.DiscardCard(card);
        TurnManager turnManager = FindAnyObjectByType<TurnManager>();

        if (turnManager != null && turnManager.enemiesWiped()){
            Debug.Log("Battle won!");
            
            if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Level1"){
                RewardManager rewardManager = FindAnyObjectByType<RewardManager>();

                if (rewardManager != null){
                    rewardManager.ShowRewards();
                }
            }
            else if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Boss Level"){
                SceneManager.LoadScene("Victory");
            }
        }

        Debug.Log(card.cardName + " used on " + targetEnemy.enemyName + " for " + card.value + " damage.");
    }
}
