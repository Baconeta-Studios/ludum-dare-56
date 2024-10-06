using UnityEngine;
using Utils;

namespace _Scripts
{
    public class CardPrefabManager : Singleton<CardPrefabManager>
    {
        public CardBase boostCard;
        public CardBase brakeCard;
        public CardBase jumpCard;
        public CardBase sabotageCard;
        public CardBase shortcutCard;
    }
}