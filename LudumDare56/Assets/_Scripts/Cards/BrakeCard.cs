using UnityEngine;

namespace _Scripts.Cards
{
    public class BrakeCard : CardBase
    {
        public override void UseCard()
        {
            base.UseCard();
            Debug.Log("Brake card used");
        }
    }
}
