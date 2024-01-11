using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region ----------------PUBLIC STATIC VARIABLES---------------------
    
    public static int maxPowerCard = 0;
    public static List<int> powerCards=new List<int>();
    public static bool isPlayed;
    public static CityData cityData;
    public static CityRouteData routeData;

    #endregion

    #region ----------------PRiVATE FUNCTIONS---------------------
    private void Awake()
    {
        
        int numGameSession = FindObjectsOfType<GameManager>().Length;
        if (numGameSession > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    #endregion
}

#region----------------GAME STATS---------------------

public enum CityType
{
    Memory = 1,
    Boss = 2
}

#endregion