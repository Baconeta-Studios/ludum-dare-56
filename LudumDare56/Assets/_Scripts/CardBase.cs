using UnityEngine;

public abstract class CardBase : MonoBehaviour
{
    private CardDeck associatedDeck;
    
    public enum UseType { INSTANT, CONDITIONAL };
    
    public UseType useType;
    
    public string name;

    public void Initialize(CardDeck associatedDeck, string name = "Untitled Card", UseType useType = UseType.INSTANT)
    {
        this.name = name;
        this.useType = useType;
    }
    
    public virtual void UseCard()
    {
        associatedDeck.DiscardCard(this);
    }

    public override string ToString()
    {
        return name;
    }
}
