using UnityEngine;

public class JumpCard : CardBase
{
    private JumpComponent jumpComponent;

    public void Start()
    {
        useType = UseType.Conditional;
        cardName = "Hop";
    }
    
    public override void UseCard()
    {
        Debug.Log("Jump card played");
        base.UseCard();
    }

    protected override void SetUsableState()
    {
        if (jumpComponent == null)
        {
            jumpComponent = associatedDeck.owner.GetComponent<JumpComponent>();
        }

        jumpComponent.IsActive = true;
        associatedDeck.owner.triggerCard = this;
    }
    
    protected override void DisableUsableState()
    {
        if (jumpComponent == null)
        {
            jumpComponent = associatedDeck.owner.GetComponent<JumpComponent>();
        }

        jumpComponent.IsActive = false;
    }
}