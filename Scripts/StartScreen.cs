using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class StartScreen : MonoBehaviour
{
    [SerializeField] GameObject continueButton;

    protected static string fileName = "/gamedata.txt";

    void Awake()
    {
        SoundManager.ins.PlayMusic("BGM");
        continueButton.SetActive(File.Exists(Application.persistentDataPath + fileName));
    }

    public void ContinueButton()
    {
        SoundManager.ins.ClickSFX();
        LevelLoader.MapScreen();
    }

    public void NewGameButton()
    {
        SoundManager.ins.ClickSFX();
        SaveSystem.DeleteSaveFile();
        LevelLoader.IntroScreen();
    }

    public void Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}