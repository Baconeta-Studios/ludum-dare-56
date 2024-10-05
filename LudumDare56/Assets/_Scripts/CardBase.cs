using UnityEngine;

public abstract class CardBase : MonoBehaviour
{
    private CardDeck associatedDeck;
    
    public enum UseType { Instant, Conditional };
    
    public UseType useType;
    
    public string cardName;

    public void Initialize(CardDeck associatedDeck, string cardName = "Untitled Card", UseType useType = UseType.Instant)
    {
        this.cardName = cardName;
        this.useType = useType;
    }
    
    public virtual void UseCard()
    {
        associatedDeck.DiscardCard(this);
    }

    public override string ToString()
    {
        return cardName;
    }
}
