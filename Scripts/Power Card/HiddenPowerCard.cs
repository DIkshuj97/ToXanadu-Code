using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenPowerCard : PowerCard
{
    public override void PlayPowerCard(bool ai=false)
    {
        Debug.Log("Hidden Card");
    }
}
