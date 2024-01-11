using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NodeUI : MonoBehaviour
{
    #region ----------------PUBLIC VARIABLES---------------------
    public CityData city;
    public CityRouteData routeData;

    public bool isPlayed;
    public bool isActive;
    public bool isWon;
    public Button nodeButton;
    public Image nodePointer;
    public Image cityIcon;
    public Color wonColor;
    public Color loseColor;


    /// <summary>
    /// if false, the node will be interactable in set UI
    /// </summary>
    private bool isLocked = true;

    /// <summary>
    /// Animator component that handles highlight animations
    /// </summary>
    [Tooltip(" Animator component that handles highlight animations")]
    [SerializeField] private Animator anim = null;

    /// <summary>
    /// name of the bool parameter that activates the clip for current active city
    /// </summary>
    [Tooltip("name of the bool parameter that activates the clip for current active city")]
    [SerializeField] private string activeCityBoolParam = "Current";

    /// <summary>
    /// name of the bool parameter that activates the clip for available city to travel to
    /// </summary>
    [Tooltip("name of the bool parameter that activates the clip for available city to travel to")]
    [SerializeField] private string availableCityBoolParam = "Available";

    #endregion

    #region Unity Standards
    private void Awake()
    {
        if (anim == null)
        {
            if (!TryGetComponent(out anim))
            {
                Debug.LogWarning("assign animator to node ui to run highlight animations");
            }
        }
    }

    #endregion

    #region ----------------PRIVATE FUNCTIONS---------------------

    private void AddMemoryGame(CityData data)
    {
        SoundManager.ins.PlaySfx("CitySelect");
        GameManager.cityData = data;
        GameManager.routeData = routeData;
        GameManager.isPlayed = isPlayed;
        if (data.cityType == CityType.Memory)
        {
            LevelLoader.MemoryGame();
        }
        else
        {
            LevelLoader.BossGame();
        }

    }
    #endregion

    #region ----------------PUBLIC FUNCTIONS---------------------

    public void SetUI()
    {
        if (anim)
        {
            anim.SetBool(activeCityBoolParam, isActive);
            anim.SetBool(availableCityBoolParam, (!IsLocked && !isPlayed));
        }
        if(isActive)
        {
            string music = "Phase" + city.phase.phaseId.ToString();
            SoundManager.ins.PlayMusic(music);
        }
        nodePointer.gameObject.SetActive(isActive);       
        nodeButton.onClick.AddListener(() =>AddMemoryGame(city));  
        //if(isPlayed)
        //{
        //    if (isWon)
        //    {
        //        cityIcon.color = wonColor;
        //    }
        //    else
        //    {
        //        cityIcon.color = loseColor;
        //    }
        //}
    }

    #endregion

    #region Properties

    /// <summary>
    /// if true, the node will be set to non-interactable
    /// </summary>
    public bool IsLocked
    {
        get { return isLocked; }
        set { isLocked = value; }
    }

    #endregion 
}