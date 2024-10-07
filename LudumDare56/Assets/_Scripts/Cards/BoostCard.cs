using UnityEngine;

public class BoostCard : CardBase
{
    private BoostComponent bc = null;

    public void Start()
    {
        useType = UseType.Instant;
        cardName = "Boost";
    }

    public override void UseCard()
    {
        associatedDeck.owner.GetComponent<BrakeComponent>().EndOverride(true);
        if (bc == null)
        {
            bc = base.associatedDeck.owner.GetComponent<BoostComponent>();
        }

        bc.StartOverride();
        Debug.Log("Boost card used");
        base.UseCard();
    }

    private void OnDisable()
    {
        bc = null;
    }
}