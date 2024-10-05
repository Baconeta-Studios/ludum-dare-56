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
        
        deck.AddCardToDeck(deckCard1);
        deck.AddCardToDeck(deckCard2);
        deck.AddCardToDeck(deckCard3);
        deck.AddCardToDeck(deckCard4, true);
        
        Debug.Log(deck.Count);

        deck.DebugContents();
        Debug.Log("Reshuffle, ignore hand.");
        deck.ReshuffleDiscardToDraw(false);
        deck.DebugContents();
        Debug.Log("Reshuffle, include hand.");
        deck.ReshuffleDiscardToDraw(true);
        deck.DebugContents();
    }
}
