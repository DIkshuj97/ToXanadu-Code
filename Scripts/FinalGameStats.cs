using System.IO;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

public class FinalGameStats : MonoBehaviour
{
    private int noOfCitiesExplored;
    private int noOfWonMemoryGames;
    private int noOfAcquiredPowerCards;
    private int noOfBossCityWon;
    private int noOfBossCityDraw;

    List<CitySaveData> citySaveDatas;

    [SerializeField] TMP_Text statsText;
    [SerializeField] TMP_Text titleText;
    [SerializeField] GameObject nextButton;

    private string statsString;

    // Start is called before the first frame update
    void Start()
    {
        nextButton.SetActive(false);
       citySaveDatas=  SaveSystem.GetCityData();
        int exploredCities = 0;
        foreach (CitySaveData cityData in citySaveDatas)
        {
            if(cityData.isPlayed)
            {
                exploredCities++;
            }
        }

        noOfCitiesExplored = exploredCities;

        noOfAcquiredPowerCards= SaveSystem.GetPowerCard().Count;

        int cityIndex = 0;
        int memoryGameWon=0;
        int bossGameWon=0;

        foreach (CitySaveData cityData in citySaveDatas)
        {
            if(cityData.isWon)
            {
                if(cityIndex<11)
                {
                    memoryGameWon += cityData.timesWon;
                }
                else
                {
                    bossGameWon += cityData.timesWon;
                }
            }
            cityIndex++;
        }

        noOfWonMemoryGames = memoryGameWon;
        noOfBossCityWon = bossGameWon;

        int timesDraw = 0;
        foreach (CitySaveData cityData in citySaveDatas)
        {
            timesDraw += cityData.timesDraw;
           
        }

        noOfBossCityDraw = timesDraw;

        titleText.text = "You are a ";

        if(BossCityWonFirstTime())
        {
            titleText.text += "Masterful Storyteller";
        }
        else if(UnlockedEveryMemory())
        {
            titleText.text += "Perfect Raconteur";
        }
        else if (AcquiredAllPowerCard())
        {
            titleText.text += "Powerful Orator";
        }
        else if(ExploredAllMemoryCities())
        {
            titleText.text += "Prolific Traveller";
        }
        else
        {
            titleText.text += "Fine Adventurer";
        }

        statsString = string.Format("Your journey thus far... \n\nYou visited <b>{0}</b> cities and made <b>{1}</b> memories. You discovered {2} out of 5 power cards.\n\nIn your battles of wit " +
            "with the wise and the great, you won {3} and drew {4} games.\n\nNow it is time for you to seek new sights in new lands."
            ,noOfCitiesExplored,noOfWonMemoryGames,noOfAcquiredPowerCards,noOfBossCityWon,noOfBossCityDraw);

        statsText.text = "";
        StartCoroutine(TypeSentence(statsString));
    }

    private bool AcquiredAllPowerCard()
    {
        if(noOfAcquiredPowerCards>=5)
        {
            return true;
        }

        return false;
    }

    private bool ExploredAllMemoryCities()
    {
        int exploredmemoryCities = 0;
        for (int i = 0; i < 11; i++)
        {
            if (citySaveDatas[i].isPlayed)
            {
                exploredmemoryCities++;
                if (exploredmemoryCities >= 11)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool UnlockedEveryMemory()
    {
        int noOfAllMemoryUnlocked = 0;
        for (int i =0; i < 11; i++)
        {
            if (citySaveDatas[i].timesWon>=5)
            {
                noOfAllMemoryUnlocked++;
                if(noOfAllMemoryUnlocked>=11)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool BossCityWonFirstTime()
    {
        int noOfBossCityWon = 0;
        for (int i = 11; i < citySaveDatas.Count; i++)
        {
            if(citySaveDatas[i].isWon)
            {
                if (citySaveDatas[i].timesLoss == 0 && citySaveDatas[i].timesDraw == 0)
                {
                    
                    noOfBossCityWon++;

                    if (noOfBossCityWon >= 4)
                    {
                        return true;
                    }
                }
            }
            
        }
        return false;
    }

    IEnumerator TypeSentence(string sentence)
    {
        yield return new WaitForSeconds(1f);
        foreach (char letter in sentence.ToCharArray())
        {
            //SoundManager.ins.PlayExtraAudio("DialogueType", aS);
            statsText.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(0.05f);
        nextButton.SetActive(true);
    }

    public void LoadStartScreen()
    {
        Time.timeScale = 1;
        SaveSystem.DeleteSaveFile();
        LevelLoader.StartScreen();
    }
}