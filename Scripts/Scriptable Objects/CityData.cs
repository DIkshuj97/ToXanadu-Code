using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Node", order = 1)]
public class CityData : ScriptableObject // Scriptable object for level configuration
{
    public string cityName;
    public string bossName;
    [TextArea(2,7)]public string description;
    public int index;
    public Sprite bgImage;

    /// <summary>
    /// The portrait of boss character which will be shown in the card game screen
    /// </summary>
    [Tooltip("The portrait of boss character which will be shown in the card game screen")]

    public Sprite bossAvater;
    public MemoryCardGroupData memoryCardData;
    public CityType cityType;
    public PhaseData phase;
    public PowerCardData powerCard;
    [TextArea(2, 7)] public string[] roundDialogues; //0 for win 1 for lose 2 for draw
    [TextArea(2, 7)] public string[] gameDialogues;  //0 for win 1 for lose 2 for draw
}