using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignPowerCard : PowerCard
{
    public override void PlayPowerCard(bool ai = false)
    {
        base.PlayPowerCard(ai);
        FindObjectOfType<CardGameController>().SignCardPower(ai);
    }
}
