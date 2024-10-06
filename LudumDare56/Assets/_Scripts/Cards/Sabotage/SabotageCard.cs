using _Scripts.Cards.Sabotage;
using UnityEngine;

public class SabotageCard : CardBase
{
    private SabotageEngine sabotageEngine;

    protected void Start()
    {
        sabotageEngine = GetComponent<SabotageEngine>();
    }

    public override void UseCard()
    {
        sabotageEngine.CreateNewSabotage();
        Debug.Log("Sabotage card used");
        base.UseCard();
    }
}