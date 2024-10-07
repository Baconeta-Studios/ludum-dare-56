using _Scripts.Cards.Sabotage;
using UnityEngine;

public class SabotageCard : CardBase
{
    private SabotageComponent sabotageComponent;
    private bool isActive;
    
    public void Start()
    {
        useType = UseType.Instant;
        cardName = "Sabotage";
    }

    public override void UseCard()
    {
        sabotageComponent = associatedDeck.owner.GetComponent<SabotageComponent>();
        sabotageComponent.CreateNewSabotage();
        Debug.Log("Sabotage card used");
        base.UseCard();
    }
}