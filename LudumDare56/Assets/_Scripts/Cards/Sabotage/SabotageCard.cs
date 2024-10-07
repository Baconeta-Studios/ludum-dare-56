using _Scripts.Cards.Sabotage;
using UnityEngine;

public class SabotageCard : CardBase
{
    private SabotageEngine sabotageEngine;

    protected void Start()
    {
        // TODO will need a reference to the racer who played it so we can get a location to place the sabotage
        sabotageEngine = GetComponent<SabotageEngine>();
    }

    public override void UseCard()
    {
        sabotageEngine.CreateNewSabotage();
        Debug.Log("Sabotage card used");
        base.UseCard();
    }
}