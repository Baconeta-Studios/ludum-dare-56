using UnityEngine;

namespace _Scripts.Cards
{
    public class ShortcutCard : CardBase
    {
        public override void UseCard()
        {
            Debug.Log("Boost card used");
            base.UseCard();
        }
    }
}
