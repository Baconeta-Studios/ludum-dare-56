using System;
using UnityEngine;

public abstract class CardBase : MonoBehaviour
{
    protected CardDeck associatedDeck;

    private Action callOnCardUsed;
    
    public enum UseType { Instant, Conditional };
    
    public UseType useType;
    
    public string cardName;

    public void Initialize(CardDeck associatedDeck, string cardName = "Untitled Card", UseType useType = UseType.Instant)
    {
        this.cardName = cardName;
        this.useType = useType;
    }

    /// <summary>
    /// Returns true if the card was used immediately
    /// </summary>
    /// <param name="callBackOnCardUsed"></param>
    /// <returns></returns>
    public bool TryUseCard(Action callBackOnCardUsed)
    {
        callOnCardUsed = callBackOnCardUsed;
        if (useType == UseType.Instant)
        {
            UseCard();
            return true;
        }
        return false;
    }

    public virtual void UseCard()
    {
        associatedDeck.DiscardCard(this);
        callOnCardUsed?.Invoke();
    }

    public virtual void EndCardEffect()
    {
        
    }

    public override string ToString()
    {
        return cardName;
    }
}
