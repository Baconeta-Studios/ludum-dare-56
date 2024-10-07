using System;
using System.Linq;
using _Scripts.Racer;
using UnityEngine;

namespace _Scripts
{
    public class CardTrayUIManager : MonoBehaviour
    {
        public ChoiceUI choiceUIPopup;
        public Vector3 trayUIStartPosition;
        public Vector3 trayUIOpenPosition;
        public RectTransform zoneUIPosition;
        private bool isTrayUIOpen = false;

        [SerializeField] private CardDeck playerCardDeck;

        public static event Action OnOpenCardSelection; 
        public static event Action OnCloseCardSelection; 
    
        // Get Player Deck system controller
        
        private void Awake()
        {
            playerCardDeck = GameObject.FindGameObjectWithTag("Player").GetComponent<CardDeck>();
        }
        
        public void OnEnable()
        {
            CheckPoint.OnRacerCrossCheckPoint += OnRacerCrossCheckPoint;
        }

        public void OnDisable()
        {
            CheckPoint.OnRacerCrossCheckPoint -= OnRacerCrossCheckPoint;
        }

        private void OnRacerCrossCheckPoint(RacerBase racer)
        {
            if (racer.GetType() != typeof(RacerPlayer))
            {
                return;
            }

            if (isTrayUIOpen)
            {
                CloseTrayUI();
            }

            if (RaceManager.Instance.HasRaceFinished)
            {
                return;
            }

            OpenChoiceUI();
        }

        private void OpenChoiceUI()
        {
            Time.timeScale = 0.01f;
            OnOpenCardSelection?.Invoke();
            choiceUIPopup.gameObject.SetActive(true);
        }

        public void CloseChoiceUI()
        {
            Time.timeScale = 1f;
            OnCloseCardSelection?.Invoke();
            choiceUIPopup.gameObject.SetActive(false);
        }
        
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
        
            Time.timeScale = 1f;
        }

        public void AddCardToUI(CardBase card)
        {
            string typeName = card.GetType().ToString();
            GameObject prefab = typeName switch
            {
                "BrakeCard" => CardPrefabManager.Instance.brakeCard,
                "JumpCard" => CardPrefabManager.Instance.jumpCard,
                "BoostCard" => CardPrefabManager.Instance.boostCard,
                "SabotageCard" => CardPrefabManager.Instance.sabotageCard,
                "ShortcutCard" => CardPrefabManager.Instance.shortcutCard,
                _ => null
            };

            if (prefab == null)
            {
                Debug.Log("Card Prefab Not Found"); 
            }
            var newCard = Instantiate(prefab, gameObject.transform, false);
            newCard.transform.localScale = Vector3.one;
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
            //Destroy(gameObject.transform.GetChild(i).gameObject);
        }

        public void PlayCard(CardBase card)
        {
            CloseTrayUI();

            // This UI function tells the Card Deck that we have moved a card to the active play zone
            if (playerCardDeck.PlayCard(card, ZoneCardUsed))
            {
                // This will be true if the card was instantly
                // played, and therefore we should trigger destruction for the game object
                Destroy(card.gameObject);
            }
        }

        public void ZoneCardUsed()
        {
            // playerCardDeck.DiscardCard(zoneUIPosition.GetChild(0).GetComponent<CardBase>());
            // Callback function for the PlayCard system to tell us once it has been used 
            if (zoneUIPosition.childCount > 0)
            {
                Destroy(zoneUIPosition.GetChild(0).gameObject);
            }
        }
    }
}
