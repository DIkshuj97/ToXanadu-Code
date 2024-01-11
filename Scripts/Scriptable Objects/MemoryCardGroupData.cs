using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Memory Card Group", order = 3)]
public class MemoryCardGroupData : ScriptableObject
{
    public List<Sprite> cardImages=new List<Sprite>();
}
