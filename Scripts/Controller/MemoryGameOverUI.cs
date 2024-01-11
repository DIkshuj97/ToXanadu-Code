using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles game over screen's interface, on finish the memory game 
/// </summary>
[RequireComponent(typeof(Canvas))]
public class MemoryGameOverUI : MonoBehaviour
{
    #region Variables

    /// <summary>
    /// the image that shows the rewarde power card if the explored city had one
    /// </summary>
    [Tooltip("the image that shows the rewarde power card if the explored city had one")]
    [SerializeField] private Image cardImage = null;

    /// <summary>
    /// the image that shows one of win/lose images provided as emblem images
    /// </summary>
    [Tooltip("the image that shows one of win/lose images provided as emblem images")]
    [SerializeField] private Image emblemImage = null;

    /// <summary>
    /// if a card is aquired the name will be shown with this tmp
    /// </summary>
    [Tooltip("if a card is aquired the name will be shown with this tmp")]
    [SerializeField] private TextMeshProUGUI cardNameTMP = null;

    /// <summary>
    /// TMP that holds the game over message text
    /// </summary>
    [Tooltip("TMP that holds the game over message text")]
    [SerializeField] private TextMeshProUGUI overMessageText = null;

    /// <summary>
    /// sprite to show in card image in the case that city had no power card but player won
    /// </summary>
    [Tooltip("sprite to show in card image in the case that city had no power card but player won")]
    [SerializeField] private Sprite emptyCardSprite = null;

    /// <summary>
    /// animator that is responsible for win/lose screen's animation 
    /// it should have a trigger parameter to activate the animation
    /// </summary>
    [Tooltip("animator that is responsible for win/lose screen's animation." +
        " it should have a trigger parameter to activate the animation")]
    [SerializeField] private Animator animator = null;

    /// <summary>
    /// the trigger parameter's name that plays the animation clip
    /// </summary>
    [Tooltip("the trigger parameter's name that plays the animation clip")]
    [SerializeField] private string animTriggerParamName = "Activate";

    /// <summary>
    /// button that gets the player confirm to go back to the map scene
    /// </summary>
    [Tooltip("button that gets the player confirm to go back to the map scene")]
    [SerializeField] private Button backBtn = null;

    /// <summary>
    /// main canvas of the game over screen. used to show/hide the menu
    /// </summary>
    [Tooltip("main canvas of the game over screen. used to show/hide the menu")]
    [SerializeField] private Canvas canvas = null;

    #endregion

    #region Unity Standards

    private void Start()
    {
        // try to find animator in the root object if it's not referenced
        if (animator == null)
        {
            if (!TryGetComponent<Animator>(out animator))
            {
                Debug.LogWarning("Assign animator to memory game over UI prefab", this);
            }
        }
        // try to find canvas in the root object if it's not referenced
        if (canvas == null)
        {
            if (!TryGetComponent<Canvas>(out canvas))
            {
                Debug.LogError("Assign main canvas to memory game over UI prefab", this);
            }
            else
            {
                // keep it disabled at the begining of the game
                canvas.enabled = false;
            }
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// runs the animation if animator exists, and trigger the param
    /// </summary>
    private void RunAnimation()
    {
        if (animator)
        {
            animator.SetTrigger(animTriggerParamName);
        }
    }
    #endregion

    #region Public Methods

    /// <summary>
    /// configure the ui elements of game over screen and runs the animation
    /// </summary>
    /// <param name="won"> true, if playser has won the game</param>
    /// <param name="emblemSprite">sprite that shows the win or lose situation</param>
    /// <param name="cardSprite">optional. power card sprie that player has won</param>
    public void SetOverScreen(bool won, bool getCard, Sprite emblemSprite,string message, Sprite cardSprite = null, string cardName = "")
    {
        // Validate arguemnts
        if (emblemImage == null)
        {
            Debug.LogWarning("emblem Image is not assigned in memory game over UI prefab", this);
            return;
        }

        if (won && cardImage == null)
        {
            Debug.LogWarning("card Image is not assigned in memory game over UI prefab", this);
            return;
        }

        if (won && cardNameTMP == null)
        {
            Debug.LogWarning("card Name tmp is not assigned in memory game over UI prefab", this);
            return;
        }

        if (emblemSprite == null)
        {
            Debug.LogError("emblem sprite is not valid", this);
            return;
        }
        if (won && cardSprite == null && emptyCardSprite == null)
        {
            Debug.LogWarning("assign empty card sprite in memory game over UI prefab", this);
            return;
        }
        if (overMessageText == null)
        {
            Debug.LogWarning("assign a text mesh pro as game over message to memory game over UI");
        }

        // set UI
        cardImage.enabled = won && getCard;
        cardImage.sprite = cardSprite;

        cardNameTMP.text = cardName;

        emblemImage.sprite = emblemSprite;

        // set message
        if (overMessageText != null && !string.IsNullOrEmpty(message))
        {
            overMessageText.text = message;
        }

        // show the canvas
        if (canvas)
        {
            canvas.enabled = true;
        }
        RunAnimation();

    }

    /// <summary>
    /// adds the given method to the back button onClick listener
    /// </summary>
    /// <param name="callback"></param>
    public void SetButtonCallBack(UnityEngine.Events.UnityAction callback)
    {
        if (backBtn)
        {
            backBtn.onClick.AddListener(callback);
        }
        else
        {
            Debug.LogError("Button not found in memory game over UI prefab", this);
        }
    }

    public void DisableCanvas()
    {
        if(canvas)
        {
            canvas.enabled = false;
        }
    }

    #endregion

}
