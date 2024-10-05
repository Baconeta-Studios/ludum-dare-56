using UnityEngine;

namespace _Scripts.Cards
{
    public class SabotageCard : CardBase
    {
        public override void UseCard()
        {
            base.UseCard();
            Debug.Log("Jump card used");
        }
    }
}