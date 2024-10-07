public class BrakeComponent : SpeedOverrideComponent
{
    public override void EndOverride(bool forceFinish = false)
    {
        base.EndOverride(forceFinish);

        // We dont clear braking  here if its force finishing because it is already done in base above
        if (!forceFinish)
        {
            OverrideFinished();
        }
    }
}
