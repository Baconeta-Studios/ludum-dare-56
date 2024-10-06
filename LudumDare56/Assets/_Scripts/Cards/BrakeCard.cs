using UnityEngine;


public class BrakeCard : CardBase
{
    private BrakeComponent bc = null;

    public override void UseCard()
    {
        if (bc == null)
        {
            bc = associatedDeck.owner.GetComponent<BrakeComponent>();
        }

        bc.StartOverride();
        Debug.Log("Brake card used");
        base.UseCard();
    }

    private void OnDisable()
    {
        bc = null;
    }
}