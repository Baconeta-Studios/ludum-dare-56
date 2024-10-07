using UnityEngine;

public class JumpCard : CardBase
{
    private JumpComponent jumpComponent;

    public override void UseCard()
    {
        Debug.Log("Jump card played");
        base.UseCard();
    }

    public override void SetUsableState()
    {
        if (jumpComponent == null)
        {
            jumpComponent = associatedDeck.owner.GetComponent<JumpComponent>();
        }

        jumpComponent.IsActive = true;
        associatedDeck.owner.triggerCard = this;
    }
}