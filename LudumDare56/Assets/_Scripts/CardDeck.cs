using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using _Scripts;
using _Scripts.Racer;
using UnityEngine;

[Serializable]
public struct NumberOfEachCard
{
    public int boostCards;
    public int brakeCards;
    public int jumpCards;
    public int sabotageCards;
    public int shortcutCards;
}

public class CardDeck : MonoBehaviour
{
    private Stack<CardBase> drawPile = new Stack<CardBase>();
    private Stack<CardBase> discardPile = new Stack<CardBase>();

    public List<CardBase> Hand { get; } = new List<CardBase>();

    public CardBase ActiveCard { get; set; } = null;

    public RacerBase owner = null;
    public RacerBase Owner
    {
        get { return owner; }
        set {
            if (owner != null) 
            {
                throw new Exception("Owner is already set and cannot be changed.");
            }
            owner = value;
        }
    }

    public NumberOfEachCard deckSetup;

    public void SetupDeck()
    {
        for (int i = 0; i < deckSetup.boostCards; i++)
        {
            AddCardToDeck(CardPrefabManager.Instance.boostCard);
        }
        for (int i = 0; i < deckSetup.brakeCards; i++)
        {
            AddCardToDeck(CardPrefabManager.Instance.brakeCard);
        }
        for (int i = 0; i < deckSetup.jumpCards; i++)
        {
            AddCardToDeck(CardPrefabManager.Instance.jumpCard);
        }
        for (int i = 0; i < deckSetup.sabotageCards; i++)
        {
            AddCardToDeck(CardPrefabManager.Instance.sabotageCard);
        }
        for (int i = 0; i < deckSetup.shortcutCards; i++)
        {
            AddCardToDeck(CardPrefabManager.Instance.shortcutCard);
        }
        
        ShuffleDrawPile();
    }

    public int Count => drawPile.Count + discardPile.Count + Hand.Count + (ActiveCard== null ? 0 : 1);

    /// <summary>
    /// Add a new card to the top of the draw pile.
    /// </summary>
    /// <param name="card">The new card to add.</param>
    /// <param name="shuffleAfter">If the draw pile should be shuffled after adding the card.</param>
    public void AddCardToDeck(CardBase card, bool shuffleAfter = false)
    {
        card.Initialize(this);
        drawPile.Push(card);
        if (shuffleAfter)
        {
            ShuffleDrawPile();
        }
    }
    
    /// <summary>
    /// Add the top card from the draw pile into hand.
    /// </summary>
    [ContextMenu("Draw 1")]
    public void DrawCard()
    {
        var card = drawPile.Pop();
        Hand.Add(card);

        if (owner.GetType() == typeof(RacerPlayer))
        {
            var ui = FindFirstObjectByType<CardTrayUIManager>();
            ui.AddCardToUI(card.gameObject);
        }
    }

    /// <summary>
    /// Discard a card from hand or from the active zone. CANNOT discard a card from the draw pile!
    /// </summary>
    /// <param name="card"></param>
    public void DiscardCard(CardBase card)
    {
        // Discard from active zone.
        if (ActiveCard != null && ActiveCard.Equals(card))
        {
            discardPile.Push(card);
            ActiveCard = null;
        } else if (Hand.Contains(card)) // Discard from hand.
        {
            discardPile.Push(card);
            Hand.Remove(card);
        }
        else
        {
            Debug.LogError($"{owner.gameObject.name}");
            throw new Exception("Invalid discard attempt.");
        }
    }
    
    /// <summary>
    /// Move all cards from hand to the discard pile.
    /// </summary>
    [ContextMenu("Discard Hand (not active card)")]
    public void DiscardHand()
    {
        foreach (var item in Hand)
            DiscardCard(item);
    }

    /// <summary>
    /// Move all cards from the draw pile to the discard pile.
    /// </summary>
    [ContextMenu("Discard Draw Pile")]
    public void DiscardDrawPile()
    {
        while (drawPile.Count > 0)
        {
            discardPile.Push(drawPile.Pop());
        }
    }

    [ContextMenu("Reshuffle Discard Pile to Draw Pile")]
    public void MenuDebugReshuffleDiscardToDraw()
    {
        ReshuffleDiscardToDraw();
    }

    /// <summary>
    /// Create a new shuffled draw pile from the discard pile.
    /// Discards all cards currently in the draw pile before reshuffling discarded cards.
    /// </summary>
    /// <param name="includeHand">Discard the cards in hand and in the active zone before reshuffling.</param>
    public void ReshuffleDiscardToDraw(bool includeHand = false)
    {
        if (includeHand)
        {
            DiscardHand();
            DiscardCard(ActiveCard);
        }
        DiscardDrawPile();
        drawPile = ShuffleStack(discardPile);
        discardPile.Clear();
    }

    [ContextMenu("Shuffle Draw Pile")]
    public void ShuffleDrawPile()
    {
        drawPile = ShuffleStack(drawPile);
    }
    
    /// <summary>
    /// Shuffle a stack of cards.
    /// </summary>
    /// <param name="pile">Stack of cards to shuffle.</param>
    /// <returns>Shuffled cards.</returns>
    private static Stack<CardBase> ShuffleStack(Stack<CardBase> pile)
    {
        List<CardBase> cards = pile.ToList();
        Stack<CardBase> newPile = new Stack<CardBase>();
        
        // Form a new drawPile by selecting random cards.
        while (cards.Count != 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, cards.Count);
            newPile.Push(cards[randomIndex]);
            cards.RemoveAt(randomIndex);

        }

        return newPile;
    }

    /// <summary>
    /// Debug.Log all cards, from all piles, in order.
    /// </summary>
    [ContextMenu("Log Cards")]
    public void DebugContents()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append($"DECK CONTENTS: {gameObject.name}\n");
        sb.Append("[DRAW PILE]\n");
        foreach (CardBase card in drawPile)
        {
            sb.Append(card.ToString());
            sb.Append(", ");
        }
        sb.Append("\n\n[HAND]\n");
        foreach (CardBase card in Hand)
        {
            sb.Append(card.ToString());
            sb.Append(", ");
        }
        sb.Append("\n\n[DISCARD PILE]\n");
        foreach (CardBase card in discardPile)
        {
            sb.Append(card.ToString());
            sb.Append(", ");
        }
        Debug.Log(sb.ToString());
    }

    public bool PlayCard(CardBase cardToUse, Action zoneCardUsed)
    {
        // End card effect for card in zone and discard it if it exists
        ActiveCard?.EndCardEffect();
        if (ActiveCard != null)
        {
            DiscardCard(ActiveCard);
        }

        // Physically move the card in the desk systems
        CardBase handCardRef = null;
        foreach (var card in Hand)
        {
            if (card.Equals(cardToUse))
            {
                handCardRef = card;
                break;
            }
        }

        // Find the card in the hand and move it to the play zone
        if (handCardRef != null)
        {
            Hand.Remove(handCardRef);
            ActiveCard = handCardRef;
        }
        
        return cardToUse.TryUseCard(zoneCardUsed);
    }
}
