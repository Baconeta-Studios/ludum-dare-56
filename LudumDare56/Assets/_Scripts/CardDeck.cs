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
    // // This class is inside the TestClass so it could access its private fields
    // // this custom editor will show up on any object with TestScript attached to it
    // // you don't need (and can't) attach this class to a gameobject
    // [CustomEditor(typeof(CardDeck))]
    // public class StackPreview : Editor {
    //     public override void OnInspectorGUI() {
    //
    //         // get the target script as TestScript and get the stack from it
    //         var ts = (CardDeck)target; 
    //         var stack = ts.discardPile;
    //         var stackhand = ts.drawPile;
    //
    //         // some styling for the header, this is optional
    //         var bold = new GUIStyle(); 
    //         bold.fontStyle = FontStyle.Bold; 
    //         GUILayout.Label("Items in my stack", bold);
    //
    //         // add a label for each item, you can add more properties
    //         // you can even access components inside each item and display them
    //         // for example if every item had a sprite we could easily show it 
    //         // foreach (var item in stack) {
    //         //     GUILayout.Label(item.name); 
    //         // }
    //         // GUILayout.Label("Items in my stack", bold);
    //         foreach (var item in stackhand) {
    //             GUILayout.Label(item.name); 
    //         }
    //     }
    // }
    
    private Stack<CardBase> drawPile = new();
    private readonly Stack<CardBase> discardPile = new();

    public List<CardBase> Hand { get; } = new();

    public CardBase ActiveCard { get; set; } = null;

    public List<CardBase> FaceUpDeck { get; set; }

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
        for (var i = 0; i < deckSetup.boostCards; i++)
        {
            var go = new GameObject();
            go.transform.SetParent(transform);
            go.AddComponent<BoostCard>();
            AddCardToDeck(go.GetComponent<CardBase>());
        }
        for (var i = 0; i < deckSetup.brakeCards; i++)
        {
            var go = new GameObject();
            go.AddComponent<BrakeCard>();
            go.transform.SetParent(transform);
            AddCardToDeck(go.GetComponent<CardBase>());
        }
        for (var i = 0; i < deckSetup.jumpCards; i++)
        {
            var go = new GameObject();
            go.AddComponent<JumpCard>();
            go.transform.SetParent(transform);
            AddCardToDeck(go.GetComponent<CardBase>());
        }
        for (var i = 0; i < deckSetup.sabotageCards; i++)
        {
            var go = new GameObject();
            go.AddComponent<SabotageCard>();
            go.transform.SetParent(transform);
            AddCardToDeck(go.GetComponent<CardBase>());
        }
        for (var i = 0; i < deckSetup.shortcutCards; i++)
        {
            var go = new GameObject();
            go.AddComponent<ShortcutCard>();
            go.transform.SetParent(transform);
            AddCardToDeck(go.GetComponent<CardBase>());
        }
        
        ShuffleDrawPile();
        
        // Put the top two cards into the face up pile
        var card = drawPile.Pop();
        var card2 = drawPile.Pop();
        FaceUpDeck = new List<CardBase>
        {
            card,
            card2
        };
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
        if (drawPile.Count == 0)
        {
            ReshuffleDiscardToDraw();
        }

        if (drawPile.Count == 0) // still
        {
            Debug.LogWarning(owner.name + " has no cards to draw.");
            return;
        }
        
        var card = drawPile.Pop();
        TakeCardToHand(card);
    }

    private void TakeCardToHand(CardBase card)
    {
        Hand.Add(card);

        if (owner.GetType() == typeof(RacerPlayer))
        {
            var ui = FindFirstObjectByType<CardTrayUIManager>();
            ui.AddCardToUI(card);
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
            throw new Exception($"Invalid discard attempt by {owner.gameObject.name}.");
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
        if (ActiveCard != null)
        {
            ActiveCard.CancelUseCard();
            ActiveCard.EndCardEffect();
            DiscardCard(ActiveCard);
        }

        // Physically move the card in the deck systems
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

    /// <summary>
    /// Gets and replaces the face-up card at the int position
    /// </summary>
    public void GetFaceUpCard(int index)
    {
        if (FaceUpDeck.Count <= index)
        {
            Debug.LogError("Face up deck index is out of range");
            return;
        }

        var cardToGive = FaceUpDeck[index];
        if (drawPile.Count == 0)
        {
            ReshuffleDiscardToDraw();
            if (drawPile.Count == 0)
            {
                Debug.LogWarning("No cards to draw to refill face up cards with");
                return;
            }
        }
        var replacementCard = drawPile.Pop();
        FaceUpDeck[index] = replacementCard;
        TakeCardToHand(cardToGive);
    }
}
