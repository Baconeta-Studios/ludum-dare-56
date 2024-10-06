using System;
using UnityEngine;

public abstract class CardBase : MonoBehaviour
{
    protected CardDeck associatedDeck;

    private Action callOnCardUsed;
    
    public enum UseType { Instant, Conditional };
    
    public UseType useType;
    
    public string cardName;

    public void Initialize(CardDeck deck)
    {
        associatedDeck = deck;
    }

    /// <summary>
    /// Returns true if the card was used immediately
    /// </summary>
    /// <param name="callBackOnCardUsed"></param>
    /// <returns></returns>
    public bool TryUseCard(Action callBackOnCardUsed=null)
    {
        callOnCardUsed = callBackOnCardUsed;
        if (useType == UseType.Instant)
        {
            UseCard();
            return true;
        }
        
        SetUsableState();
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

    public override bool Equals(object other)
    {
        var otherCard = other as CardBase;
        if (otherCard != null && cardName == otherCard.cardName)
        {
            return Equals(associatedDeck, otherCard.associatedDeck);
        }

        return false;
    }

    public virtual void SetUsableState()
    {
    }
}
