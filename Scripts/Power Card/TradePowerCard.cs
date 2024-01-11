using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradePowerCard : PowerCard
{
    public override void PlayPowerCard(bool ai=false)
    {
        base.PlayPowerCard(ai);
        FindObjectOfType<CardGameController>().TradeCardPower(ai);
    }
}
