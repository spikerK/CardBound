using UnityEngine;

public class Card : MonoBehaviour
{
    public string cardName;
    public int energyCost;
    public int value;

    public enum CardType{
        Attack, Skill
    }

    public CardType cardType;
}
