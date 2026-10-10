using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class DeckManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform handPanel;

    public List<Card> deck = new List<Card>();
    public List<Card> hand = new List<Card>();
    public List<Card> discard = new List<Card>();

    void UpdateCardText(Card card){
        TMP_Text text = card.GetComponentInChildren<TMP_Text>();

        if (text != null){
            text.text = card.cardName + "\nCost: " + card.energyCost + "\nValue: " + card.value;
        }
    }
    
    Card Createcard(string name, int cost, int value, Card.CardType type){
        GameObject cardObject = Instantiate(cardPrefab, handPanel);
        cardObject.SetActive(false);

        Card card = cardObject.GetComponent<Card>();

        card.cardName = name;
        card.energyCost = cost;
        card.value = value;
        card.cardType = type;

        UpdateCardText(card);

        return card;
    }

    void CreateStartingDeck(){
        for (int i = 0; i < 3; i++){
            Card strike = Createcard("Strike", 1, 5, Card.CardType.Attack);
            deck.Add(strike);
        }
        for (int i = 0; i < 3; i++){
            Card heavy = Createcard("Heavy", 2, 8, Card.CardType.Attack);
            deck.Add(heavy);
        }
        for (int i = 0; i < 3; i++){
            Card block = Createcard("Block", 1, 6, Card.CardType.Skill);
            deck.Add(block);
        }
        Card focus = Createcard("Focus", 0, 2, Card.CardType.Skill);
        deck.Add(focus);

        Debug.Log("Starting deck created: " + deck.Count + " cards");
    }

    void ShuffleDeck(){
        for(int i = 0; i < deck.Count; i++){
            int randomIndex = Random.Range(i, deck.Count);

            Card temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    void DrawHand(int amount){

        for(int i = 0; i < amount; i++){
            if (deck.Count == 0){
                ShuffleDiscardIntoDeck();
            }
            if (deck.Count == 0){
                Debug.Log("No cards to draw");
                break;
            }

            Card card = deck[0];
            deck.RemoveAt(0);
            hand.Add(card);

            card.transform.SetParent(handPanel, false);
            card.gameObject.SetActive(true);
            
            CanvasGroup group = card.GetComponent<CanvasGroup>();

            if (group != null){
                group.alpha = 1f;
                group.blocksRaycasts = true;
                group.interactable = true;
            }

            RectTransform rect = card.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2((hand.Count - 1) * 250 - ((amount -1) * 250 / 2f), 0);

            rect.localScale = Vector3.one;
        }

        Debug.Log("Cards in hand: " + hand.Count);
    }

    void ShuffleDiscardIntoDeck(){
        if (discard.Count == 0){
            return;
        }

        deck.AddRange(discard);
        discard.Clear();

        for(int i = 0; i < deck.Count; i++){
            int randomIndex = Random.Range(i, deck.Count);

            Card temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }

        Debug.Log("Discard pile shuffled into the deck.");
    }
    
    void LoadSavedDeck(){
        deck.Clear();
        hand.Clear();
        discard.Clear();

        foreach (Transform child in handPanel){
            Destroy(child.gameObject);
        }

        foreach (string cardName in RunData.savedCardNames){
            Card card = null;

            if (cardName == "Strike"){
                card = Createcard("Strike", 1, 5, Card.CardType.Attack);
            }
            else if (cardName == "Heavy"){
                card = Createcard("Heavy", 2, 8, Card.CardType.Attack);
            }
            else if (cardName == "Block"){
                card = Createcard("Block", 1, 6, Card.CardType.Skill);
            }
            else if (cardName == "Focus"){
                card = Createcard("Focus", 0, 2, Card.CardType.Skill);
            }
            else if (cardName == "Heal"){
                card = Createcard("Heal", 1, 3, Card.CardType.Skill);
            }
            else if (cardName == "Freeze"){
                card = Createcard("Freeze", 1, 0, Card.CardType.Skill);
            }
            else if (cardName == "Starburst Stream"){
                card = Createcard("Starburst Stream", 3, 4, Card.CardType.Attack);
            }

            if (card != null){
                deck.Add(card);
            }
        }

        ShuffleDeck();
        DrawHand(4);

        Debug.Log("Loaded saved deck with " + deck.Count + " cards.");
    }

    public void DiscardCard(Card card){
        if (hand.Contains(card)){
            hand.Remove(card);
            discard.Add(card);
            card.gameObject.SetActive(false);

            Debug.Log(card.cardName + " has been discarded.");
        }
    }

    public void EndTurn(){
        for (int i = hand.Count - 1; i >= 0; i--){
            Card card = hand[i];

            hand.RemoveAt(i);
            discard.Add(card);
            card.gameObject.SetActive(false);
        }

        Debug.Log("Player turn ended.");
    }

    public void StartPlayerTurn(){
        Player player = FindAnyObjectByType<Player>();

        if (player != null){
            player.ResetEnergy();
        }

        DrawHand(4);

        Debug.Log("Player turn started.");
    }

    void Start(){
        if (RunData.hasSavedDeck){
            LoadSavedDeck();
            RunData.hasSavedDeck = false;
        }
        else{
            CreateStartingDeck();
            ShuffleDeck();
            DrawHand(4);
        }
    }

    public void AddRewardCard(string rewardName){
        Card reward = null;

        if (rewardName == "Heal"){
            reward = Createcard("Heal", 1, 3, Card.CardType.Skill);
        }
        else if (rewardName == "Freeze"){
            reward = Createcard("Freeze", 1, 0, Card.CardType.Skill);
        }
        else if(rewardName == "Starburst Stream"){
            reward = Createcard("Starburst Stream", 3, 4, Card.CardType.Attack);
        }

        if (reward != null){
            deck.Add(reward);
            ShuffleDeck();
            Debug.Log(rewardName + " added to your deck!");
        }
        
    }

    public void SaveDeck(){
        RunData.savedCardNames.Clear();
        
        foreach (Card card in deck){
            RunData.savedCardNames.Add(card.cardName);
        }

        foreach (Card card in hand){
            RunData.savedCardNames.Add(card.cardName);
        }

        foreach (Card card in discard){
            RunData.savedCardNames.Add(card.cardName);
        }

        RunData.hasSavedDeck = true;

        Debug.Log("Saved deck with " + RunData.savedCardNames.Count + " cards.");
    }
}
