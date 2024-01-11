using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="City Route",order =4)]
public class CityRouteData : ScriptableObject
{
    public int nodeId;
    public int[] nextNodesIndex;
}
