using UnityEngine;

public class JumpCard : CardBase
{
    public override void UseCard()
    {
        Debug.Log("Jump card used");
        base.UseCard();
    }
}