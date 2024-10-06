using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrakeComponent : SpeedOverrideComponent
{
    protected override void EndOverride(bool forceFinish = false)
    {
        base.EndOverride(forceFinish);

        // We dont clear braking  here if its force finishing because it is already done in base above
        if (!forceFinish)
        {
            OverrideFinished();
        }
    }
}
