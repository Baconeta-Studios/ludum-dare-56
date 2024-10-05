using System.Collections.Generic;
using UnityEngine;

public class DeckTester : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
        CardDeck deck = gameObject.AddComponent<CardDeck>();
        
        BlankCard deckCard1 = gameObject.AddComponent<BlankCard>();
        deckCard1.Initialize(deck, "Card 1");
        BlankCard deckCard2 = gameObject.AddComponent<BlankCard>();
        deckCard2.Initialize(deck, "Card 2");
        BlankCard deckCard3 = gameObject.AddComponent<BlankCard>();
        deckCard3.Initialize(deck, "Card 3");
        BlankCard deckCard4 = gameObject.AddComponent<BlankCard>();
        deckCard4.Initialize(deck, "Card 4");
        
        Stack<CardBase> cards = new Stack<CardBase>();
        cards.Push(deckCard1);
        cards.Push(deckCard2);
        cards.Push(deckCard3);
        cards.Push(deckCard4);
        deck.Initialize(cards, 0);
        
        Debug.Log(deck.Count);

        deck.DebugContents();
        deck.ReshuffleDiscardToDraw();
        deck.DebugContents();
    }
}
