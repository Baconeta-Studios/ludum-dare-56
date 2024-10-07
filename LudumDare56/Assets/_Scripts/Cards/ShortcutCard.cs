using UnityEngine;

public class ShortcutCard : CardBase
{
    private ShortcutComponent shortcutComponent;
    
    public void Start()
    {
        useType = UseType.Conditional;
        cardName = "Shortcut";
    }

    public override void UseCard()
    {
        Debug.Log("Shortcut card played");
        base.UseCard();
    }

    public override void SetUsableState()
    {
        if (shortcutComponent == null)
        {
            shortcutComponent = associatedDeck.owner.GetComponent<ShortcutComponent>();
        }

        shortcutComponent.IsActive = true;
        associatedDeck.owner.triggerCard = this;
    }
}