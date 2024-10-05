using UnityEngine;

namespace _Scripts.Cards
{
    public class BrakeCard : CardBase
    {
        public override void UseCard()
        {
            Debug.Log("Brake card used");
            base.UseCard();
        }
    }
}
