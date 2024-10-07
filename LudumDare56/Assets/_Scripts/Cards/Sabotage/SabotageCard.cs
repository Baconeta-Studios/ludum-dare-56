using _Scripts.Cards.Sabotage;
using UnityEngine;

public class SabotageCard : CardBase
{
    private SabotageComponent sabotageEngine;

    protected void Start()
    {
        sabotageEngine = GetComponent<SabotageComponent>();
    }

    public override void UseCard()
    {
        sabotageEngine.CreateNewSabotage();
        Debug.Log("Sabotage card used");
        base.UseCard();
    }
}