using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System.IO;


public class MemoryGameController : MonoBehaviour
{
    #region ----------------PRIVATE VARIABLES---------------------
    [SerializeField] Transform cardHolder;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject descriptionPanel;
    [SerializeField] GameObject gamePanel;
    [SerializeField] Sprite cardBgImage;
    [SerializeField] List<Sprite> cards = new List<Sprite>();
    [SerializeField] GameObject startButton;
    [SerializeField] List<Sprite> cardImages = new List<Sprite>();


    [SerializeField] private TMP_Text cityNameText;
    [SerializeField] private TMP_Text cityDescriptionText;
    [SerializeField] private GameObject memoryImageHolder;

    /// <summary>
    /// Handles game over screen's interface, on finish the memory game
    /// </summary>
    [Tooltip("Handles game over screen's interface, on finish the memory game")]
    [SerializeField] private MemoryGameOverUI gameOverUI = null;

    /// <summary>
    /// sprite to be shown in case the player has won the memory game
    /// </summary>
    [Tooltip("sprite to be shown in case the player has lost the memory game")]
    [SerializeField] private Sprite loseScreenPicture = null;

    /// <summary>
    /// message to be shown in over screen after losing the memory game
    /// </summary>
    [Tooltip("message to be shown in over screen after losing the memory game")]
    [SerializeField] private string loseMessage = "\'Take only memories,  \nleave only footprints\'";

    /// <summary>
    /// message to be shown in over screen after wining the memory game but no power cards
    /// </summary>
    [Tooltip("message to be shown in over screen after wining the memory game but no power cards. Use {0} in city name's place")]
    [SerializeField] private string continueMessage = "You have visited {0}!";

    /// <summary>
    /// message to be shown in over screen after winning a powercard
    /// </summary>
    [Tooltip("message to be shown in over screen after winning a powercard. Use {0} in power card's place")]
    [SerializeField] private string winCardMessage = "You have aquired the {0} card!";

    /// <summary>
    /// sprite to be shown in case the player has won the memory game
    /// </summary>
    [Tooltip("sprite to be shown in case the player has won the memory game")]
    [SerializeField] private Sprite winScreenPicture = null;

    /// <summary>
    /// sprite to be shown in case the player has won a powercard alognside the card picture
    /// </summary>
    [Tooltip("sprite to be shown in case the player has won a powercard alognside the card picture")]
    [SerializeField] private Sprite winCardEmblemPicture = null;

    /// <summary>
    /// Audio source component that plays the memory gmae sounds
    /// </summary>
    [Tooltip("Audio source component that plays the memory gmae sounds")]
    [SerializeField] private AudioSource audioSource = null;

    /// <summary>
    /// label of sound to play with the Win memory game screen
    /// </summary>
    [Tooltip("label of the sound to play with Win memory game screen. " +
        "corresponded to the one of sound manager list tracks")]
    [SerializeField] private string winMemorySound = "";

    /// <summary>
    /// label of sound to play with the lost memory game screen
    /// </summary>
    [Tooltip("label of the sound to play with the lost memory game screen" +
        "corresponded to the one of sound manager list tracks")]
    [SerializeField] private string loseMemorySound = "";


    private bool firstGuess, secondGuess;
    private int firstGuessIndex, secondGuessIndex;
    private string firstGuessCard, secondGuessCard;
    private float flipTimer;
    private int maxCards;
    private Sprite answerSprite;
    private int timesWon;
    private int timesLoss;

    #endregion

    #region ----------------PUBLIC VARIABLES---------------------
    public List<Button> cardBtn = new List<Button>();
   
    #endregion

    #region ----------------PRIVATE FUNCTIONS---------------------
    private void Awake()
    {

        if(!SaveSystem.GetActiveCityData(GameManager.routeData.nodeId).isPhaseCompleted)
        {
            SaveSystem.SetCityActive(GameManager.routeData.nodeId);
        }
        else
        {
            int phaseId = GameManager.cityData.phase.phaseId;

            if (phaseId == 1)
            {
                SaveSystem.SetCityActive(11);
            }
            else if (phaseId == 2)
            {
                SaveSystem.SetCityActive(12);
            }
            else if (phaseId == 3)
            {
                SaveSystem.SetCityActive(13);
            }
        }
        
        cityNameText.text = GameManager.cityData.cityName;
        cityDescriptionText.text = GameManager.cityData.description;

        cards = GameManager.cityData.memoryCardData.cardImages;
        flipTimer = GameManager.cityData.phase.memoryGameFliptime;
        maxCards = GameManager.cityData.phase.memoryCards;

       
        UnlockMemoryImages();

        // add the back button onClick listener
        if (gameOverUI)
        {
            gameOverUI.SetButtonCallBack(delegate { OpenDescriptionPage(); });
        }
        // check if audio source is referenced
        if (audioSource == null && !TryGetComponent<AudioSource>(out audioSource))
        {
            Debug.LogError("Audio source not found in Memory Game Controller");
        }

      
        SoundManager.ins.PlayMusic("Memory");
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

    private void AddCardToGameCardList()
    {
        cardImages.Clear();

        if(timesWon>0)
        {
            ShuffleNumbers(cards);
        }

        int count = 0;
        for (int i = 0; i < maxCards; i++)
        {
            count++;
            if (i < 2)
            {
                count = 0;
                cardImages.Add(cards[count]);
                answerSprite = cardImages[i];
            }
            else
            {
                cardImages.Add(cards[count]);
            }
        }
    }

    private void ShuffleNumbers(List<Sprite> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Sprite temp = list[i];
            int randomIndex = Random.Range(i, list.Count);

            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    private void ConfigureCardButton()
    {
        if(cardBtn.Count!=0)
        {
            foreach (Button card in cardBtn)
            {
                Destroy(card.gameObject);
            }

            cardBtn.Clear();
        }

        for (int i = 0; i < maxCards; i++)
        {
            GameObject card = Instantiate(cardPrefab);
            card.name = i.ToString();
            card.transform.SetParent(cardHolder, false);

            cardBtn.Add(card.GetComponent<Button>());
            card.GetComponent<Button>().image.sprite = cardBgImage;
            card.GetComponent<Button>().interactable = false;
        }

        AddListenerToCardBtn();
    }

    private void AddListenerToCardBtn()
    {
        int i = 0;
        foreach (Button card in cardBtn)
        {
            card.onClick.RemoveAllListeners();
            card.onClick.AddListener(() => PickACard(card.gameObject));
            i++;
        }
    }

    private IEnumerator CheckForMatch()
    {
        yield return new WaitForSeconds(0.5f);

        if (firstGuessCard == secondGuessCard)
        {
            SaveSystem.IncreaseTimeWon(GameManager.routeData.nodeId);
            yield return new WaitForSeconds(0.25f);
            cardBtn[firstGuessIndex].interactable = false;
            cardBtn[secondGuessIndex].interactable = false;
            CheckGameFinished();
        }
        else
        {
            SaveSystem.IncreaseTimeLoss(GameManager.routeData.nodeId);
            StartCoroutine(ShowCardsAfterResult(false));
            if (!SaveSystem.GetActiveCityData(GameManager.routeData.nodeId).isPhaseCompleted)
            {
                SaveSystem.SaveCityData(false, true, true, GameManager.routeData.nodeId);
            }
        }
    }

    /// <summary>
    /// activates the game over screen and configure it based on win/lose
    /// </summary>
    /// <param name="win">True, if player has won and flase if lost</param>
    private void ActivateGameOverScreen(bool win)
    {
        if (gameOverUI)
        {
            /// true if the player has won a power card in the game
            bool getCard = false;

            string cardName = "...";
            string cityName = GameManager.cityData != null ? GameManager.cityData.cityName : "...";

            /// power card's sprite that has been rewarded to the player
            /// retrievable from power cards data
            Sprite rewardedCardPicture = null;

            if (win && timesWon<=0 && timesLoss==0)
            {
                ///TODO: Rewarded Powercard sprite should be passed here as the third argument
                /// getCard = ?;
                /// rewardedCardPicture = ?;
                PowerCardData powerData = GameManager.cityData.powerCard;
                if (powerData != null)
                {
                    getCard = true;
                    rewardedCardPicture = powerData.bgImage;
                    cardName = powerData.name;
                }
            }

            /// message to be shown on over screen:
            /// e.g. you have visited [...] city
            /// e.g. you have won [...] power card
            /// e.g. you can't remember what you have seen
            string message = GetOverScreenMessage(win, getCard, cityName, cardName); /// TODO: insert city or card name

            /// set the emblem sprite according to state: won game and card, won only game, lost game
            Sprite emblem = win ? (getCard ? winCardEmblemPicture : winScreenPicture) : loseScreenPicture;

            /// pass data to ui controller to set up the game over screen
            gameOverUI.SetOverScreen(win, getCard, emblem, message, rewardedCardPicture, cardName);

            /// playe sfx: win or lose sound
            PlaySFX(win ? winMemorySound : loseMemorySound);
        }
        else
        {
            Debug.LogError("Game Over UI canvas is not assigned in Memory Game Controller");
        }
    }

    /// <summary>
    /// returns the game over screen message based on give game states
    /// </summary>
    /// <param name="_wonGame">if the player has won the memory game in the city</param>
    /// <param name="_wonCard">if the player has won the game and a power card in the city</param>
    /// <returns></returns>
    private string GetOverScreenMessage(bool _wonGame, bool _wonCard, string _cityName = "", string _cardName = "")
    {
        string msg = string.Empty;
        if (_wonGame)
        {
            if (_wonCard)
            {
                msg = string.Format(winCardMessage, _cardName);
            }
            else
            {
                msg = string.Format(continueMessage, _cityName);
            }
        }
        else
        {
            msg = loseMessage;
        }
        return msg;
    }

    /// <summary>
    /// Plays the give clip in one shot
    /// </summary>
    /// <param name="clip"></param>
    private void PlaySFX(string clip)
    {
        if (SoundManager.ins)
        {
            SoundManager.ins.PlaySfx(clip);
        }
    }

    private IEnumerator ShowNumbersDelay()
    {
        yield return new WaitForSeconds(flipTimer);
        SoundManager.ins.PlaySfx("Memory-FaceDown");
        foreach (Button card in cardBtn)
        {
            card.GetComponent<Button>().image.sprite = cardBgImage;
            card.GetComponent<Button>().interactable = true;
        }        
    }

    private void CheckGameFinished()
    {
        //winPanel.SetActive(true);
        if (!SaveSystem.GetActiveCityData(GameManager.routeData.nodeId).isPhaseCompleted)
        {
            if (GameManager.cityData.powerCard != null && timesWon<=1 && timesLoss==0)
            {
                SaveSystem.SavePowerCardData(GameManager.cityData.powerCard.cardId, isAcquired: true);
            }
            SaveSystem.SaveCityData(true, true, true, GameManager.routeData.nodeId);
        }
      
        StartCoroutine(ShowCardsAfterResult(true));
    }

    private IEnumerator ShowCardsAfterResult(bool isWon)
    {
        int i = 0;
        foreach (Button card in cardBtn)
        {
            card.GetComponent<Button>().image.sprite = cardImages[i];
            i++;
        }

        List<Button> answers = cardBtn.FindAll(x => x.GetComponent<Image>().sprite.name == answerSprite.name);
        yield return new WaitForSeconds(1f);
        foreach (Button image in answers)
        {
            image.gameObject.transform.DOScale(new Vector3(1.25f, 1.25f, 1.25f), 0.5f);
        }

        yield return new WaitForSeconds(2f);
        ActivateGameOverScreen(isWon);
    }

    private void ResetSettings()
    {
        firstGuess = false;
        firstGuessCard = "";
        firstGuessIndex = 0;

        secondGuess = false;
        firstGuessCard = "";
        secondGuessIndex = 0;

        UnlockMemoryImages();
    }
    
    private void UnlockMemoryImages()
    {
        timesWon = SaveSystem.TimesWon(GameManager.routeData.nodeId);
        timesLoss = SaveSystem.TimesLoss(GameManager.routeData.nodeId);
        if (maxCards > 6)
        {
            memoryImageHolder.GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedRowCount;
            memoryImageHolder.GetComponent<GridLayoutGroup>().constraintCount = 2;
        }

        List<Sprite> tempList = new List<Sprite>();

        foreach (Sprite card in cards)
        {
            tempList.Add(card);
        }

        ShuffleNumbers(tempList);

        for (int i = 0; i < maxCards - 1; i++)
        {
            if (i < timesWon)
            {
                memoryImageHolder.transform.GetChild(i).GetComponent<Image>().sprite = tempList[i];
            }
            else
            {
                memoryImageHolder.transform.GetChild(i).GetComponent<Image>().sprite = cardBgImage;
            }
            memoryImageHolder.transform.GetChild(i).gameObject.SetActive(true);
        }
    }


    #endregion

    #region ----------------PUBLIC FUNCTIONS---------------------
    public void PickACard(GameObject cardName)
    {
        SoundManager.ins.PlaySfx("CardFlip");
        if (!firstGuess)
        {
            firstGuess = true;
            firstGuessIndex = int.Parse(cardName.name);
            firstGuessCard = cardImages[firstGuessIndex].name;
            cardBtn[firstGuessIndex].image.sprite = cardImages[firstGuessIndex];
            cardBtn[firstGuessIndex].interactable = false;
        }
        else if (!secondGuess)
        {
            secondGuess = true;
            secondGuessIndex = int.Parse(cardName.name);
            secondGuessCard = cardImages[secondGuessIndex].name;
            cardBtn[secondGuessIndex].image.sprite = cardImages[secondGuessIndex];
            cardBtn[secondGuessIndex].interactable = false;
            StartCoroutine(CheckForMatch());
        }
    }

    public void StartGame()
    {
        SoundManager.ins.ClickSFX();
        descriptionPanel.SetActive(false);
        gamePanel.SetActive(true);
        AddCardToGameCardList();
        ShuffleNumbers(cardImages);
        ConfigureCardButton();
    }

    public void ShowCards()
    {
        startButton.SetActive(false);
        int i = 0;
        foreach (Button card in cardBtn)
        {
            card.GetComponent<Button>().image.sprite = cardImages[i];
            i++;
        }

        StartCoroutine(ShowNumbersDelay());
    }

    public void LoadMainMenu()
    {
        SoundManager.ins.PlaySfx("BackButton");
        Time.timeScale = 1;
        LevelLoader.StartScreen();
    }

    public void LoadMapMenu()
    {
        SoundManager.ins.PlaySfx("BackButton");
        Time.timeScale = 1;
        LevelLoader.MapScreen();
    }

    public void OpenDescriptionPage()
    {
        SoundManager.ins.PlaySfx("BackButton");
        Time.timeScale = 1;
        ResetSettings();
        gamePanel.SetActive(false);
        gameOverUI.DisableCanvas();
        descriptionPanel.SetActive(true);
        startButton.SetActive(true);
    }

    public void UnPauseGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1;
    }
    #endregion
}