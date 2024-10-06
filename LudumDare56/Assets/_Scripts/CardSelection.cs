using UnityEngine;

namespace _Scripts
{
    public class CardSelection : MonoBehaviour
    {
        public CardDeck cardDeck;

        public void SelectFaceUpCardA()
        {
            // We give the card to the player then draw a new card to fill this spot
            cardDeck.GetFaceUpCard(0);
        }
    
        public void SelectFaceUpCardB()
        {
            // We give the card to the player then draw a new card to fill this spot
            cardDeck.GetFaceUpCard(1);
        }
    
        // Takes 2 cards from the deck
        public void SelectFaceDownCards()
        {
            cardDeck.DrawCard();
            cardDeck.DrawCard();
        }
    }
}
