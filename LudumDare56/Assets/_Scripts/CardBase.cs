using System;
using UnityEngine;

public abstract class CardBase : MonoBehaviour
{
    protected CardDeck associatedDeck;
    private Action callOnCardUsed;
    public enum UseType { Instant, Conditional };
    public UseType useType;
    public string cardName;
    public AudioClip cardSound;
    public float cardSoundVolume = 0.5f;

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

    /// <summary>
    /// AKA discard a card from the hand before it is used by using a new card
    /// </summary>
    public void CancelUseCard()
    {
        DisableUsableState();
    }

    public virtual void UseCard()
    {
        associatedDeck.DiscardCard(this);
        AudioSystem.Instance.PlaySound(cardSound, cardSoundVolume);
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

    protected virtual void SetUsableState()
    {
        if (useType == UseType.Instant)
        {
            return; // Usable state isn't handled on instant cards
        }
    }

    protected virtual void DisableUsableState()
    {
        if (useType == UseType.Instant)
        {
            return; // Usable state isn't handled on instant cards
        }
    }
}