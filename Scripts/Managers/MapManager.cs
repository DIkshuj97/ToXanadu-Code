using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class MapManager : MonoBehaviour
{
    #region ----------------PRIVATE VARIABLES---------------------
    [SerializeField] private CityData[] citiesData;
    [SerializeField] private NodeUI[] citiesUI;
    [SerializeField] private CityRouteData[] citiesRoute;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject firstActiveImage;
    [SerializeField] private GameObject inventoryButton;

    private List<CityData> phaseOneCities = new List<CityData>();
    private List<CityData> phaseTwoCities = new List<CityData>();
    private List<CityData> phaseThirdCities = new List<CityData>();

    private List<CityData> cityDataFromSave = new List<CityData>();
    private PlayerSaveData playerData = new PlayerSaveData();
    
    #endregion

    #region ----------------PRIVATE FUNCTIONS---------------------
    
    
    private void Start()
    {
        
        List<CitySaveData> cityDatas = new List<CitySaveData>();

        if (File.Exists(Application.persistentDataPath + "/gamedata.txt"))
        {
            string json = File.ReadAllText(Application.persistentDataPath + "/gamedata.txt");
            playerData = JsonUtility.FromJson<PlayerSaveData>(json);
            inventoryButton.SetActive(true);
        }
        else
        {
            playerData = SaveSystem.CreateEmptyFile();
            cityDatas = playerData.citiesStats;
            
            foreach (CityData city in citiesData)
            {
                if (city.cityType == CityType.Memory)
                {
                    if (city.phase.phaseId == 1)
                    {
                        phaseOneCities.Add(city);
                    }
                    else if (city.phase.phaseId == 2)
                    {
                        phaseTwoCities.Add(city);
                    }
                    else if (city.phase.phaseId == 3)
                    {
                        phaseThirdCities.Add(city);
                    }
                }
            }

            ShuffleCities(phaseOneCities);
            ShuffleCities(phaseTwoCities);
            ShuffleCities(phaseThirdCities);

            for (int i = 0; i < cityDatas.Count; i++)
            {
                if (i < phaseOneCities.Count)
                {
                    cityDatas[i].index = phaseOneCities[i].index;
                }
                else if (i - phaseOneCities.Count < phaseTwoCities.Count)
                {
                    cityDatas[i].index = phaseTwoCities[i - phaseOneCities.Count].index;
                }
                else if (i - phaseOneCities.Count - phaseTwoCities.Count < phaseThirdCities.Count)
                {
                    cityDatas[i].index = phaseThirdCities[i - phaseOneCities.Count - phaseTwoCities.Count].index;
                }
                else
                {
                    cityDatas[i].index = citiesData[i].index;
                }
            }
            SaveSystem.SaveCityData(cityDatas);
        }

        cityDatas = playerData.citiesStats;
        List<CityData> citiesList = new List<CityData>();

        for (int i = 0; i < citiesData.Length; i++)
        {
            citiesList.Add(citiesData[i]);
        }
        for (int i = 0; i < cityDatas.Count; i++)
        {
            cityDataFromSave.Add(citiesList.Find(x => cityDatas[i].index == x.index));
        }

        for (int i = 0; i < citiesUI.Length; i++)
        {          
            citiesUI[i].city = cityDataFromSave[i];
            citiesUI[i].routeData = citiesRoute[i];
            citiesUI[i].isPlayed = playerData.citiesStats[i].isPlayed;
            citiesUI[i].isActive = playerData.citiesStats[i].isActive;
            citiesUI[i].isWon = playerData.citiesStats[i].isWon;
            citiesUI[i].SetUI();

            if (citiesUI[i].isPlayed && (playerData.citiesStats[i].timesWon > 0 || playerData.citiesStats[i].timesDraw>0))
            {
                for (int j = 0; j < citiesUI[i].routeData.nextNodesIndex.Length; j++)
                {
                    NodeUI node = citiesUI[citiesUI[i].routeData.nextNodesIndex[j]];
                    
                    node.IsLocked = false;
                    node.nodeButton.interactable = true;
                    node.SetUI();
                } 
            }
        }

        if (!citiesUI[0].isActive && !citiesUI[0].isPlayed)
        {
            firstActiveImage.SetActive(true);
        }

        /* if (playerData.citiesStats[11].isPhaseCompleted)
         {
             foreach (NodeUI city in citiesUI)
             {
                 if(city.city.phase.phaseId==1)
                 {
                     city.nodeButton.interactable = false;
                 }
             }
         }
         if (playerData.citiesStats[12].isPhaseCompleted)
         {
             foreach (NodeUI city in citiesUI)
             {
                 if (city.city.phase.phaseId == 2)
                 {
                     city.nodeButton.interactable = false;
                 }
             }
         }
         if (playerData.citiesStats[13].isPhaseCompleted)
         {
             foreach (NodeUI city in citiesUI)
             {
                 if (city.city.phase.phaseId == 3)
                 {
                     city.nodeButton.interactable = false;
                 }
             }
         }*/
        inventoryButton.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pausePanel.activeSelf)
            {
                Time.timeScale = 0;
                pausePanel.SetActive(true);
            }
        }
    }

    private void ShuffleCities(List<CityData> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            CityData temp = list[i];
            int randomIndex = Random.Range(i, list.Count);

            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void LoadMainMenu()
    {
        SoundManager.ins.PlaySfx("BackButton");
        Time.timeScale = 1;
        LevelLoader.StartScreen();
    }

    public void QuitGame()
    {
        SaveSystem.DeleteSaveFile();
        LevelLoader.StartScreen();
    }
    #endregion
}
