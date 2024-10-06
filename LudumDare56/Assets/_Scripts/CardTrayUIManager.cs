using System;
using System.Linq;
using UnityEngine;

namespace _Scripts
{
    public class CardTrayUIManager : MonoBehaviour
    {
        public Vector3 trayUIStartPosition;
        public Vector3 trayUIOpenPosition;
        
        public RectTransform zoneUIPosition;
    
        private bool isTrayUIOpen = false;

        [SerializeField] private CardDeck playerCardDeck;
     
        public event Action OnTrayUIOpen;
        public event Action OnTrayUIClose;
    
        // Get Player Deck system controller
    
        private void Start()
        {
            gameObject.transform.localPosition = trayUIStartPosition;
        }

        public void ToggleTrayUI()
        {
            if (isTrayUIOpen)
            {
                CloseTrayUI();
            }
            else
            {
                OpenTrayUI();
            }
        }

        private void OpenTrayUI()
        {
            Debug.Log("Opening Tray UI");
            if (isTrayUIOpen)
            {
                // Do nothing
                return;
            }

            gameObject.transform.localPosition = trayUIOpenPosition;
            isTrayUIOpen = true;
            OnTrayUIOpen?.Invoke();

            Time.timeScale = 0.2f;
        }

        private void CloseTrayUI()
        {
            Debug.Log("Closing Tray UI");
            if (!isTrayUIOpen)
            {
                // Do nothing
                return;
            }

            gameObject.transform.localPosition = trayUIStartPosition;
            isTrayUIOpen = false;
            OnTrayUIClose?.Invoke();
        
            Time.timeScale = 1f;
        }

        public void AddCardToUI(GameObject card)
        {
            var newCard = Instantiate(card, gameObject.transform, false);
            newCard.GetComponent<CardBase>().Initialize(playerCardDeck);
        }
        
        // This should probably only be used for discards so may be useless
        public void RemoveCardFromUI(GameObject card)
        {
            var i = 0;
            var cardBase = card.GetComponent<CardBase>();
            if (gameObject.GetComponentsInChildren<CardBase>().Any(cardObject => cardBase == cardObject))
            {
                i = cardBase.transform.GetSiblingIndex();
            }
            Destroy(gameObject.transform.GetChild(i).gameObject);
        }

        public void PlayCard(CardBase card)
        {
            // This UI function tells the Card Deck that we have moved a card to the active play zone
            if (playerCardDeck.PlayCard(card, ZoneCardUsed))
            {
                // This will be true if the card was instantly
                // played, and therefore we should trigger destruction for the game object
            }
            else
            {
                // This likely will need to change to delay destruction and play some pretty effects
                Destroy(zoneUIPosition.GetChild(0).gameObject);
            }
        }

        private void ZoneCardUsed()
        {
            playerCardDeck.DiscardCard(zoneUIPosition.GetChild(0).GetComponent<CardBase>());
            // Callback function for the PlayCard system to tell us once it has been used 
            Destroy(zoneUIPosition.GetChild(0).gameObject);
        }
    }
}
