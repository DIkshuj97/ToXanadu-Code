using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{

    #region ----------------PRIVATE FUNCTIONS---------------------
    private void Awake()
    {
        int numGameSession = FindObjectsOfType<LevelLoader>().Length;
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

    #region ----------------PUBLIC FUNCTIONS---------------------

    public static void MapScreen()
    {
        SceneManager.LoadScene("MapScene");
    }

    public static void IntroScreen()
    {
        SceneManager.LoadScene("IntroScene");
    }

    public static void StatsScreen()
    {
        SceneManager.LoadScene("FinalScreen");
    }

    public static void MemoryGame()
    {
        SceneManager.LoadScene("MemoryGameScene");
    }

    public static void StartScreen()
    {   
        SceneManager.LoadScene("StartScene");
    }

    public static void BossGame()
    {
        SceneManager.LoadScene("CardGameScene");
    }

    public static void Loadlevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }
    #endregion
}
