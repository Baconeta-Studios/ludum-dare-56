using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CardDeck : MonoBehaviour
{
    private Stack<CardBase> drawPile = new Stack<CardBase>();
    private Stack<CardBase> discardPile = new Stack<CardBase>();
    private List<CardBase> hand = new List<CardBase>();
    private CardBase activeCard = null;
    public int Count => drawPile.Count + discardPile.Count + hand.Count + (activeCard== null ? 0 : 1);

    public void Initialize(Stack<CardBase> initialDrawPile, int startingHandSize = 0)
    {
        drawPile = initialDrawPile;
        for (int i = 0; i < startingHandSize; i++)
        {
            DrawCard();
        }
    }

    public void AddCardToDeck(CardBase card)
    {
        drawPile.Push(card);
    }
    
    public void DrawCard()
    {
        hand.Add(drawPile.Pop());
    }

    public void DiscardCard(CardBase card)
    {
        // Discard from active zone.
        if (activeCard.Equals(card))
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

    public void DiscardPile(IEnumerable<CardBase> pile)
    {
        foreach (var item in pile)
            discardPile.Push(item);
    }
    
    public void ReshuffleDiscardToDraw(bool includeHand = false)
    {
        if (includeHand)
        {
            DiscardPile(hand);
            DiscardCard(activeCard);
        }
        DiscardPile(drawPile);
        drawPile = ShuffleStack(discardPile);
        discardPile.Clear();
    }
    
    public static Stack<CardBase> ShuffleStack(Stack<CardBase> pile)
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

    public string DrawPileToString()
    {
        StringBuilder sb = new StringBuilder();
        foreach (CardBase card in drawPile)
        {
            sb.Append("Dr");
            sb.Append(card.ToString());
            sb.Append(" ");
        }
        foreach (CardBase card in hand)
        {
            sb.Append("H");
            sb.Append(card.ToString());
            sb.Append(" ");
        }
        foreach (CardBase card in discardPile)
        {
            sb.Append("Di");
            sb.Append(card.ToString());
            sb.Append(" ");
        }
        return sb.ToString();
    }
}
