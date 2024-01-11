using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Power Card", order = 4)]
public class PowerCardData : ScriptableObject
{
    public int cardId;
    public string cardName;
    public string cardAction;
    [TextArea(3,5)]public string description;
    public Sprite bgImage; 
}
