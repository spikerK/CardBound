using UnityEngine;
using UnityEngine.EventSystems;

public class CardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    private Vector2 startPos;

    private Camera mainCamera;

    
    void Awake(){
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null){
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        mainCamera = Camera.main;
    }
    
    public void OnBeginDrag(PointerEventData eventData){
        startPos = rectTransform.anchoredPosition;

        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false;

        rectTransform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData){
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData){
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        Card card = GetComponent<Card>();

        if(card != null && card.cardType == Card.CardType.Skill){
            if (mainCamera == null){
                mainCamera = Camera.main;
            }
            if (mainCamera != null){
                Ray ray = mainCamera.ScreenPointToRay(eventData.position);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

                if (hit.collider != null){
                    Player player = hit.collider.GetComponent<Player>();

                    if (player != null){
                        player.PlaySkill(card);
                        return;
                    }
                }
            }
        }

        if (gameObject.activeInHierarchy){
            rectTransform.anchoredPosition = startPos;
        }
    }
}
