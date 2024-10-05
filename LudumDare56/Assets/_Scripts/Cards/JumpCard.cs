using UnityEngine;

namespace _Scripts.Cards
{
    public class JumpCard : CardBase
    {
        public override void UseCard()
        {
            base.UseCard();
            Debug.Log("Jump card used");
        }
    }
}
