using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Scripts
{
    public class ChoiceUI : MonoBehaviour
    {
        [SerializeField] private RectTransform choiceUIFaceUpA;
        [SerializeField] private RectTransform choiceUIFaceUpB;
        [SerializeField] private RectTransform choiceUIFaceDown;
        [SerializeField] private CardSelection cardSelection;
        [SerializeField] private CardTrayUIManager cardTrayUIManager;

        private void OnEnable()
        {
            PopulateChoices();
        }

        private void PopulateChoices()
        {
            if (choiceUIFaceUpA.childCount > 0)
            {
                Destroy(choiceUIFaceUpA.transform.GetChild(0).gameObject);
            }
            if (choiceUIFaceUpB.childCount > 0)
            {
                Destroy(choiceUIFaceUpB.transform.GetChild(0).gameObject);
            }

            var player = FindAnyObjectByType<RacerPlayer>();
            if (cardSelection == null)
            {
                cardSelection = player.GetComponent<CardSelection>();
            }

            if (cardSelection.cardDeck.FaceUpDeck[0] != null)
            {
                var cardA = cardSelection.cardDeck.FaceUpDeck[0].GetType().ToString();
                var cardARef = cardA switch
                {
                    "BoostCard" => Instantiate(CardPrefabManager.Instance.boostCard, choiceUIFaceUpA, false),
                    "JumpCard" => Instantiate(CardPrefabManager.Instance.jumpCard, choiceUIFaceUpA, false),
                    "BrakeCard" => Instantiate(CardPrefabManager.Instance.brakeCard, choiceUIFaceUpA, false),
                    "SabotageCard" => Instantiate(CardPrefabManager.Instance.sabotageCard, choiceUIFaceUpA, false),
                    "ShortcutCard" => Instantiate(CardPrefabManager.Instance.shortcutCard, choiceUIFaceUpA, false),
                };
                cardARef.GetComponent<EventTrigger>().enabled = false;
                Button button = cardARef.AddComponent<Button>();
                button.onClick.AddListener(DrawCardA);
                cardARef.transform.localScale = new Vector3(1.75f, 1.75f, 1.75f);
            }
            
            if (cardSelection.cardDeck.FaceUpDeck[1] != null)
            {
                var cardB = cardSelection.cardDeck.FaceUpDeck[1].GetType().ToString();
                var cardBRef = cardB switch
                {
                    "BoostCard" => Instantiate(CardPrefabManager.Instance.boostCard, choiceUIFaceUpB, false),
                    "JumpCard" => Instantiate(CardPrefabManager.Instance.jumpCard, choiceUIFaceUpB, false),
                    "BrakeCard" => Instantiate(CardPrefabManager.Instance.brakeCard, choiceUIFaceUpB, false),
                    "SabotageCard" => Instantiate(CardPrefabManager.Instance.sabotageCard, choiceUIFaceUpB, false),
                    "ShortcutCard" => Instantiate(CardPrefabManager.Instance.shortcutCard, choiceUIFaceUpB, false),
                };
                cardBRef.GetComponent<EventTrigger>().enabled = false;
                Button button2 = cardBRef.AddComponent<Button>();
                button2.onClick.AddListener(DrawCardB);
                cardBRef.transform.localScale = new Vector3(1.75f, 1.75f, 1.75f);
            }
        }

        private void DrawCardA()
        {
            cardSelection.SelectFaceUpCardA();
            CloseUI();
        }

        private void DrawCardB()
        {
            cardSelection.SelectFaceUpCardB();
            CloseUI();
        }

        public void DrawDeck()
        {
            cardSelection.SelectFaceDownCards();
            CloseUI();
        }

        private void CloseUI()
        {
            if (cardTrayUIManager == null)
            {
                cardTrayUIManager = FindAnyObjectByType<CardTrayUIManager>();
            }
            
            cardTrayUIManager.CloseChoiceUI();
        }

    }
}