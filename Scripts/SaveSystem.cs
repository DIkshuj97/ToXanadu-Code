using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

public class SaveSystem:MonoBehaviour
{
    protected static string gameFileName = "/gamedata.txt";
    protected static string aiFileName = "/aidata.txt";

    public static void SaveCityData(bool isWon,bool isPlayed, bool isActive,int nodeId)
    {
        if (File.Exists(Application.persistentDataPath + gameFileName))
        {
            string json = File.ReadAllText(Application.persistentDataPath + gameFileName);
            PlayerSaveData playerData = JsonUtility.FromJson<PlayerSaveData>(json);

            foreach (CitySaveData city in playerData.citiesStats)
            {
                city.isActive = false;
            }

            CitySaveData cityData = playerData.citiesStats.Find(x => x.id == nodeId);
            cityData.isActive = isActive;
            cityData.isWon = isWon;
            cityData.isPlayed = isPlayed;

            int index = playerData.citiesStats.FindIndex(x => x.id == nodeId);
            playerData.citiesStats[index] = cityData;

            string jsonUpdated = JsonUtility.ToJson(playerData);
            File.WriteAllText(Application.persistentDataPath + gameFileName, jsonUpdated);
        }
    }

    public static CitySaveData GetActiveCityData(int id)
    {
        if (File.Exists(Application.persistentDataPath + gameFileName))
        {
            string json = File.ReadAllText(Application.persistentDataPath + gameFileName);
            PlayerSaveData playerData = JsonUtility.FromJson<PlayerSaveData>(json);
        
            CitySaveData cityData = playerData.citiesStats.Find(x => x.id == id);
            return cityData;
        }

        return null;
    }

    public static void SavePhaseData(List<int> nodeIDs,bool isPhaseCompleted,bool isActive=false, bool isPlayed=false, bool isWon= false,int activeNode=0)
    {
        if (File.Exists(Application.persistentDataPath + gameFileName))
        {
            string json = File.ReadAllText(Application.persistentDataPath + gameFileName);
            PlayerSaveData playerData = JsonUtility.FromJson<PlayerSaveData>(json);

            for (int i = 0; i < nodeIDs.Count; i++)
            {
                CitySaveData cityData = playerData.citiesStats.Find(x => x.id == nodeIDs[i]);

                if(isPhaseCompleted)
                {
                    cityData.isPhaseCompleted = isPhaseCompleted;
                }
                else
                {
                    cityData.isPhaseCompleted = isPhaseCompleted;
                    cityData.isActive = isActive;
                    cityData.isWon = isWon;
                    cityData.isPlayed = isPlayed;

                    if(activeNode>0)
                    {
                        CitySaveData activeCityData = playerData.citiesStats.Find(x => x.id == activeNode);
                        activeCityData.isActive = true;
                        int activeCityIndex = playerData.citiesStats.FindIndex(x => x.id == activeNode);
                        playerData.citiesStats[activeCityIndex] = activeCityData;
                        activeNode = -1;
                    }
                }
                
                int index = playerData.citiesStats.FindIndex(x => x.id == nodeIDs[i]);
                playerData.citiesStats[index] = cityData;
            }

            string jsonUpdated = JsonUtility.ToJson(playerData);
            File.WriteAllText(Application.persistentDataPath + gameFileName, jsonUpdated);
            Debug.Log(jsonUpdated);
        }
    }

    public static void SaveCityData(List<CitySaveData> saveData)
    {
        if (File.Exists(Application.persistentDataPath + gameFileName))
        {
            string json = File.ReadAllText(Application.persistentDataPath + gameFileName);
            PlayerSaveData playerData = JsonUtility.FromJson<PlayerSaveData>(json);

            playerData.citiesStats = saveData;

            string jsonUpdated = JsonUtility.ToJson(playerData);
            File.WriteAllText(Application.persistentDataPath + gameFileName, jsonUpdated);
        }
    }

    public static void SavePowerCardData(int id, bool isAcquired)
    {
        if (File.Exists(Application.persistentDataPath + gameFileName))
        {
            string json = File.ReadAllText(Application.persistentDataPath + gameFileName);
            PlayerSaveData playerData = JsonUtility.FromJson<PlayerSaveData>(json);

            
            PowerCardSaveData powerCard = playerData.powerCardStats.Find(x => x.id == id);
            powerCard.isAcquired = isAcquired;

            int index = playerData.powerCardStats.FindIndex(x => x.id ==id);
            playerData.powerCardStats[index] = powerCard;

            string jsonUpdated = JsonUtility.ToJson(playerData);
            File.WriteAllText(Application.persistentDataPath + gameFileName, jsonUpdated);
        }
    }


    public static void SaveCardGameRecords(CardRoundSaveData data)
    {
        if (File.Exists(Application.persistentDataPath + aiFileName))
        {
            string json = File.ReadAllText(Application.persistentDataPath + aiFileName);
            CardRoundData roundData = JsonUtility.FromJson<CardRoundData>(json);
            
            if (data != null)
            {
                roundData.cardGameRecords.Add(data);
            }

            string jsonUpdated = JsonUtility.ToJson(roundData);
            File.WriteAllText(Application.persistentDataPath + aiFileName, jsonUpdated);
            Debug.Log(json);
        }
    }


    public static List<CardRoundSaveData> LoadCardGameRecords()
    {
        if (File.Exists(Application.persistentDataPath + aiFileName))
        {
            string json = File.ReadAllText(Application.persistentDataPath + aiFileName);
            CardRoundData roundData = JsonUtility.FromJson<CardRoundData>(json);

            return roundData.cardGameRecords;
        }
        else
        {
            CardRoundData roundData = new CardRoundData();
            string rounddataJson = JsonUtility.ToJson(roundData);
            File.WriteAllText(Application.persistentDataPath + aiFileName, rounddataJson);
        }
        return new List<CardRoundSaveData>();
    }

    public static PlayerSaveData CreateEmptyFile()
    {
        PlayerSaveData playerSaveData = new PlayerSaveData();
        CitySaveData city;

        for (int i = 0; i < 15; i++)
        {
            city = new CitySaveData();
            city.id = i;
            city.isActive = false;
            city.isPlayed = false;
            city.isWon = false;
            city.isPhaseCompleted = false;
            city.timesWon = 0;
            city.timesDraw = 0;
            city.timesLoss = 0;
            playerSaveData.citiesStats.Add(city);
        }

        PowerCardSaveData powerCard;
        for (int i = 0; i < 11; i++)
        {
            powerCard = new PowerCardSaveData();
            powerCard.id = i;
            powerCard.isAcquired = false;

            playerSaveData.powerCardStats.Add(powerCard);
        }

        string json = JsonUtility.ToJson(playerSaveData);
        File.WriteAllText(Application.persistentDataPath + gameFileName, json);
        return playerSaveData;
    }

    public static List<int> GetPowerCard()
    {
        PlayerSaveData playerData;
        List<int> acquiredPowerCard = new List<int>();
        if (File.Exists(Application.persistentDataPath + gameFileName))
        {
            string json = File.ReadAllText(Application.persistentDataPath + gameFileName);
            playerData = JsonUtility.FromJson<PlayerSaveData>(json);
        }
        else
        {
            playerData = CreateEmptyFile();  
        }

        foreach (PowerCardSaveData card in playerData.powerCardStats)
        {
            if(card.isAcquired)
            {
                acquiredPowerCard.Add(card.id);
            }
        }
        return acquiredPowerCard;

    }

    public static int TimesWon(int nodeID)
    {
        PlayerSaveData playerData;
        if (File.Exists(Application.persistentDataPath + gameFileName))
        {
            string json = File.ReadAllText(Application.persistentDataPath + gameFileName);
            playerData = JsonUtility.FromJson<PlayerSaveData>(json);

            CitySaveData cityData = playerData.citiesStats.Find(x => x.id == nodeID);
            return cityData.timesWon;
        }
        return 0;
    }

    public static void IncreaseTimeWon(int nodeID)
    {
        PlayerSaveData playerData;
        if (File.Exists(Application.persistentDataPath + gameFileName))
        {
            string json = File.ReadAllText(Application.persistentDataPath + gameFileName);
            playerData = JsonUtility.FromJson<PlayerSaveData>(json);

            CitySaveData cityData = playerData.citiesStats.Find(x => x.id == nodeID);
            cityData.timesWon += 1;

            int index = playerData.citiesStats.FindIndex(x => x.id == nodeID);
            playerData.citiesStats[index] = cityData;

            string jsonUpdated = JsonUtility.ToJson(playerData);
            File.WriteAllText(Application.persistentDataPath + gameFileName, jsonUpdated);
        }
    }

    public static void ResetTimesWon(int nodeID)
    {
        PlayerSaveData playerData;
        if (File.Exists(Application.persistentDataPath + gameFileName))
        {
            string json = File.ReadAllText(Application.persistentDataPath + gameFileName);
            playerData = JsonUtility.FromJson<PlayerSaveData>(json);

            CitySaveData cityData = playerData.citiesStats.Find(x => x.id == nodeID);
            cityData.timesWon =0;

            int index = playerData.citiesStats.FindIndex(x => x.id == nodeID);
            playerData.citiesStats[index] = cityData;

            string jsonUpdated = JsonUtility.ToJson(playerData);
            File.WriteAllText(Application.persistentDataPath + gameFileName, jsonUpdated);
        }
    }

    public static int TimesDraw(int nodeID)
    {
        PlayerSaveData playerData;
        if (File.Exists(Application.persistentDataPath + gameFileName))
        {
            string json = File.ReadAllText(Application.persistentDataPath + gameFileName);
            playerData = JsonUtility.FromJson<PlayerSaveData>(json);

            CitySaveData cityData = playerData.citiesStats.Find(x => x.id == nodeID);
            return cityData.timesDraw;
        }
        return 0;
    }

    public static void IncreaseTimeDraw(int nodeID)
    {
        PlayerSaveData playerData;
        if (File.Exists(Application.persistentDataPath + gameFileName))
        {
            string json = File.ReadAllText(Application.persistentDataPath + gameFileName);
            playerData = JsonUtility.FromJson<PlayerSaveData>(json);

            CitySaveData cityData = playerData.citiesStats.Find(x => x.id == nodeID);
            cityData.timesDraw += 1;

            int index = playerData.citiesStats.FindIndex(x => x.id == nodeID);
            playerData.citiesStats[index] = cityData;

            string jsonUpdated = JsonUtility.ToJson(playerData);
            File.WriteAllText(Application.persistentDataPath + gameFileName, jsonUpdated);
        }
    }

    public static void ResetTimesDraw(int nodeID)
    {
        PlayerSaveData playerData;
        if (File.Exists(Application.persistentDataPath + gameFileName))
        {
            string json = File.ReadAllText(Application.persistentDataPath + gameFileName);
            playerData = JsonUtility.FromJson<PlayerSaveData>(json);

            CitySaveData cityData = playerData.citiesStats.Find(x => x.id == nodeID);
            cityData.timesDraw =0;

            int index = playerData.citiesStats.FindIndex(x => x.id == nodeID);
            playerData.citiesStats[index] = cityData;

            string jsonUpdated = JsonUtility.ToJson(playerData);
            File.WriteAllText(Application.persistentDataPath + gameFileName, jsonUpdated);
        }
    }


    public static int TimesLoss(int nodeID)
    {
        PlayerSaveData playerData;
        if (File.Exists(Application.persistentDataPath + gameFileName))
        {
            string json = File.ReadAllText(Application.persistentDataPath + gameFileName);
            playerData = JsonUtility.FromJson<PlayerSaveData>(json);

            CitySaveData cityData = playerData.citiesStats.Find(x => x.id == nodeID);
            return cityData.timesLoss;
        }
        return 0;
    }

    public static void IncreaseTimeLoss(int nodeID)
    {
        PlayerSaveData playerData;
        if (File.Exists(Application.persistentDataPath + gameFileName))
        {
            string json = File.ReadAllText(Application.persistentDataPath + gameFileName);
            playerData = JsonUtility.FromJson<PlayerSaveData>(json);

            CitySaveData cityData = playerData.citiesStats.Find(x => x.id == nodeID);
            cityData.timesLoss += 1;

            int index = playerData.citiesStats.FindIndex(x => x.id == nodeID);
            playerData.citiesStats[index] = cityData;

            string jsonUpdated = JsonUtility.ToJson(playerData);
            File.WriteAllText(Application.persistentDataPath + gameFileName, jsonUpdated);
        }
    }

    public static void ResetTimesLose(int nodeID)
    {
        PlayerSaveData playerData;
        if (File.Exists(Application.persistentDataPath + gameFileName))
        {
            string json = File.ReadAllText(Application.persistentDataPath + gameFileName);
            playerData = JsonUtility.FromJson<PlayerSaveData>(json);

            CitySaveData cityData = playerData.citiesStats.Find(x => x.id == nodeID);
            cityData.timesLoss = 0;

            int index = playerData.citiesStats.FindIndex(x => x.id == nodeID);
            playerData.citiesStats[index] = cityData;

            string jsonUpdated = JsonUtility.ToJson(playerData);
            File.WriteAllText(Application.persistentDataPath + gameFileName, jsonUpdated);
        }
    }

    public static List<CitySaveData> GetCityData()
    {
        PlayerSaveData playerData;
        if (File.Exists(Application.persistentDataPath + gameFileName))
        {
            string json = File.ReadAllText(Application.persistentDataPath + gameFileName);
            playerData = JsonUtility.FromJson<PlayerSaveData>(json);
            return playerData.citiesStats;
        }

        return null;
    }

    public static void SetCityActive(int nodeId)
    {
        PlayerSaveData playerData;
        if (File.Exists(Application.persistentDataPath + gameFileName))
        {
            string json = File.ReadAllText(Application.persistentDataPath + gameFileName);
            playerData = JsonUtility.FromJson<PlayerSaveData>(json);
        }
        else
        {
            playerData = CreateEmptyFile();
        }

        foreach (CitySaveData city in playerData.citiesStats)
        {
            city.isActive = false;
        }

        CitySaveData cityData = playerData.citiesStats.Find(x => x.id == nodeId);
        cityData.isActive = true;

        int index = playerData.citiesStats.FindIndex(x => x.id == nodeId);
        playerData.citiesStats[index] = cityData;

        string jsonUpdated = JsonUtility.ToJson(playerData);
        File.WriteAllText(Application.persistentDataPath + gameFileName, jsonUpdated);
    }

    public static void DeleteSaveFile()
    {
        if(File.Exists(Application.persistentDataPath + gameFileName))
        {
            File.Delete(Application.persistentDataPath + gameFileName);
        }
    }
}

[System.Serializable]
public class PlayerSaveData
{
    public List<CitySaveData> citiesStats=new List<CitySaveData>();
    public List<PowerCardSaveData> powerCardStats = new List<PowerCardSaveData>();
}

[System.Serializable]
public class CardRoundData
{
    public List<CardRoundSaveData> cardGameRecords = new List<CardRoundSaveData>();
}

[System.Serializable]
public class CitySaveData
{
    public int id;
    public int index;
    public bool isPlayed;
    public bool isActive;
    public bool isWon;
    public bool isPhaseCompleted;
    public int timesWon;
    public int timesDraw;
    public int timesLoss;
}

[System.Serializable]
public class PowerCardSaveData
{
    public int id;
    public bool isAcquired;
}

/// <summary>
/// list of played number cards by player on different states 
/// used to learn the player play style and anticipate thier moves
/// </summary>

[System.Serializable]
public class CardRoundSaveData
{
    public enum GameState { Ahead = 0, Tie = 1, Behind = 2};

    public int roundNum;
    public List<CardRoundRecord> records;
    public GameState finalResult;

    public CardRoundSaveData(int _roundNum)
    {
        roundNum = _roundNum;
        records = new();
        finalResult = GameState.Tie;
    }
}

/// <summary>
/// player's play card info 
/// </summary>
[System.Serializable]
public class CardRoundRecord
{
    public int round;
    public CardRoundSaveData.GameState state;
    public int playedNumber;

    public CardRoundRecord()
    {
        round = 0;
        state = CardRoundSaveData.GameState.Tie;
        playedNumber = 0;
    }
}

