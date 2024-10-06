using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using _Scripts.Racer;
using UnityEngine;

public class CardDeck : MonoBehaviour
{
    private Stack<CardBase> drawPile = new Stack<CardBase>();
    private Stack<CardBase> discardPile = new Stack<CardBase>();
    private List<CardBase> hand = new List<CardBase>();
    private CardBase activeCard = null;
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
    
    public int Count => drawPile.Count + discardPile.Count + hand.Count + (activeCard== null ? 0 : 1);

    /// <summary>
    /// Add a new card to the top of the draw pile.
    /// </summary>
    /// <param name="card">The new card to add.</param>
    /// <param name="shuffleAfter">If the draw pile should be shuffled after adding the card.</param>
    public void AddCardToDeck(CardBase card, bool shuffleAfter = false)
    {
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
        hand.Add(drawPile.Pop());
    }

    /// <summary>
    /// Discard a card from hand or from the active zone. CANNOT discard a card from the draw pile!
    /// </summary>
    /// <param name="card"></param>
    public void DiscardCard(CardBase card)
    {
        // Discard from active zone.
        if (activeCard != null && activeCard.Equals(card))
        {
            discardPile.Push(card);
            activeCard = null;
        } else if (hand.Contains(card)) // Discard from hand.
        {
            discardPile.Push(card);
            hand.Remove(card);
        }
        else
        {
            throw new Exception("Invalid discard attempt.");
        }
    }
    
    /// <summary>
    /// Move all cards from hand to the discard pile.
    /// </summary>
    [ContextMenu("Discard Hand (not active card)")]
    public void DiscardHand()
    {
        foreach (var item in hand)
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
    
    
    /// <summary>
    /// Create a new shuffled draw pile from the discard pile.
    /// Discards all cards currently in the draw pile before reshuffling discarded cards.
    /// </summary>
    /// <param name="includeHand">Discard the cards in hand and in the active zone before reshuffling.</param>
    [ContextMenu("Reshuffle Discard Pile to Draw Pile")]
    public void ReshuffleDiscardToDraw(bool includeHand = false)
    {
        if (includeHand)
        {
            DiscardHand();
            DiscardCard(activeCard);
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
    public void DebugContents()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("[DRAW PILE] ");
        foreach (CardBase card in drawPile)
        {
            sb.Append(card.ToString());
            sb.Append(" ");
        }
        sb.Append("[HAND] ");
        foreach (CardBase card in hand)
        {
            sb.Append(card.ToString());
            sb.Append(" ");
        }
        sb.Append("[DISCARD PILE] ");
        foreach (CardBase card in discardPile)
        {
            sb.Append(card.ToString());
            sb.Append(" ");
        }
        Debug.Log(sb.ToString());
    }
}
