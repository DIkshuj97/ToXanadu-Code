using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Phase", order = 2)]
public class PhaseData : ScriptableObject
{
    public int phaseId;
    public int memoryCards;
    public int powerCards;
    public float memoryGameFliptime;
    public int numberCardInBoss;
    public int swap;
}
