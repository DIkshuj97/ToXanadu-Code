using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardGameController : MonoBehaviour
{
    #region ----------------PRIVATE VARIABLES---------------------
    [SerializeField] Transform playerCardHolder;
    [SerializeField] Transform aiCardHolder;
    [SerializeField] RectTransform playerSelectedPowerCard;
    [SerializeField] RectTransform aiSelectedPowerCard;
    [SerializeField] RectTransform playerUsedCards;
    [SerializeField] RectTransform canvasTransform;

    [SerializeField] GameObject numberCardPrefab;
    [SerializeField] GameObject[] playerPowerCardSlots;
    [SerializeField] GameObject[] aiPowerCardSlots;

    [SerializeField] Sprite[] numberCards;
    [SerializeField] Sprite cardbgImage;
    [SerializeField] Sprite deadCard;

    [SerializeField] Animator totumAnimator;

    [SerializeField] TMP_Text playerScoreText;
    [SerializeField] TMP_Text aiScoreText;
    [SerializeField] TMP_Text gameStatusText;
    [SerializeField] TMP_Text cityName;

    [SerializeField] GameObject losePanel;
    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject availablePowerCardPopup;

    [SerializeField] Image playerSelectedCard;
    [SerializeField] Image aiSelectedCard;
    [SerializeField] Transform usedCard;

    [SerializeField] Image bossPortraitWon;
    [SerializeField] TMP_Text bossGameLoseText;
    [SerializeField] Image bossPortraitLose;
    [SerializeField] TMP_Text bossGameWonText;
    [SerializeField] GameObject roundSpeechBox;
    [SerializeField] TMP_Text bossRoundSpeechText;
    [SerializeField] GameObject statsButton;

    private List<Button> playerCards = new List<Button>();
    private List<Button> aiCards = new List<Button>();

    private string bossName;

    private int playerSelectedNumber = 0;
    private int aiSelectedNumber = 0;

    private int playerScore;
    private int aiScore;

    private int roundPlayed = 0;
    private int maxRound = 0;
    private int maxPowerCard = 0;

    private bool canUsePowerCard = true;
    private bool playerEyeActivated = false;

    private bool canAIUsePowerCard = true;
    //private bool aiHasHiddenCard = false;
    //private bool playerHasHiddenCard = false;
    private bool positionSwaped = false;

    //private Transform playerHiddenCard;
    //private Transform aiHiddenCard;
    int aiDesiredNum = -1;

    /// <summary>
    /// store played card numbers by player
    /// </summary>
    private CardRoundSaveData currentCardGameRecord;

    /// <summary>
    /// old stored player's played cards
    /// </summary>
    private List<CardRoundSaveData> oldRecords = new();

    /// <summary>
    /// list of cards in AI's hand
    /// </summary>
    public List<int> aiHand = new();

    /// <summary>
    /// current player winning state : Winning, Losing, Draw
    /// </summary>
    private CardRoundSaveData.GameState currentPlayerWinningState = CardRoundSaveData.GameState.Tie;

    /// <summary>
    /// If true, AI's decisions and reasons will be logged in console
    /// </summary>
    [Tooltip("If true, AI's decisions and reasons will be logged in console")]
    [SerializeField] bool logAIthoughts = false;


    /// <summary>
    /// Enemy Avatar Image. it will be changed to the current city boss
    /// </summary>
    [Tooltip("Enemy Avatar Image. it will be changed to the current city boss")]
    [SerializeField] private Image bossAvatarImg = null;

    #endregion

    #region ----------------PUBLIC VARIABLES---------------------
    public List<GameObject> playerPowerCards = new List<GameObject>();
    public List<GameObject> allPowerCards = new List<GameObject>();
    public List<GameObject> aiPowerCards = new List<GameObject>();
    public List<GameObject> playerUsedPowerCard = new List<GameObject>();
    public List<GameObject> aiUsedPowerCard = new List<GameObject>();
    public List<GameObject> playerUsedNumberCard = new List<GameObject>();
    public List<GameObject> aiUsedNumberCard = new List<GameObject>();

    public GameObject selectedPlayerObject;
    public GameObject selectedPowerCard;

    public bool isShowing = false;
    #endregion

    #region ----------------PRIVATE FUNCTIONS---------------------
    private void Awake()
    {
        ShowInventory(true);
        cityName.text = GameManager.cityData.cityName;
        List<int> powerCards = SaveSystem.GetPowerCard();
        bossName = GameManager.cityData.bossName;
        foreach (int card in powerCards)
        {
            if (allPowerCards.Exists(x => x.GetComponent<PowerCard>().powerCardData.cardId == card))
            {
                GameObject powerCard = allPowerCards.Find(x => x.GetComponent<PowerCard>().powerCardData.cardId == card);
                powerCard.SetActive(true);
            }
        }

        /// Set the boss Avatar 
        /// TODO: this should be run when the screen is visible and data is loaded.
        if (bossAvatarImg != null)
        {
            CityData cityData = GameManager.cityData;
            if (cityData != null)
            {
                if (cityData.cityType == CityType.Boss)
                {
                    if (cityData.bossAvater != null)
                    {
                        bossAvatarImg.sprite = cityData.bossAvater;
                    }
                    else
                    {
                        Debug.LogWarning(cityData.name + "'s boss Avatar image not found");
                    }
                }
            }
            else
            {
                Debug.LogError("City Data is not Valid");
            }
        }
        else
        {
            Debug.LogWarning("boss Avatar image is not assigned in card game controller");
        }

        

        totumAnimator.SetBool("Show", false);
        totumAnimator.SetBool("Continue", false);

        totumAnimator.GetComponent<Button>().onClick.RemoveAllListeners();
        totumAnimator.GetComponent<Button>().onClick.AddListener(() => ShowCards());
    }

    public void UnPauseGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void StartGame()
    {
        SoundManager.ins.ClickSFX();
        SoundManager.ins.PlayMusic("Boss");
        losePanel.SetActive(false);
        winPanel.SetActive(false);
      
        ShowInventory(false);
        //availablePowerCardPopup.SetActive(false);

        maxRound = GameManager.cityData.phase.numberCardInBoss;
        maxPowerCard = GameManager.cityData.phase.powerCards;

        currentCardGameRecord = new(maxRound);
        oldRecords = SaveSystem.LoadCardGameRecords();

        bossPortraitLose.sprite = GameManager.cityData.bossAvater;
        bossPortraitWon.sprite = GameManager.cityData.bossAvater;
        bossGameLoseText.text = GameManager.cityData.gameDialogues[1];
        bossGameWonText.text = GameManager.cityData.gameDialogues[0];

        //instatiate card for player according to phase
        InstatiateCards(playerCardHolder, playerCards);

        //instatiate card for AI according to phase
        InstatiateCards(aiCardHolder, aiCards,true);

        for (int r = 0; r < maxRound; r++)
        {
            currentCardGameRecord.records.Add(new CardRoundRecord());
        }


        //instatiate power card in Power Holder
        for (int i = 0; i < playerPowerCards.Count; i++)
        {
            playerPowerCardSlots[i].SetActive(true);
            playerPowerCards[i].transform.SetParent(playerPowerCardSlots[i].transform, false);
            playerPowerCards[i].GetComponent<Button>().onClick.RemoveAllListeners();
            playerPowerCards[i].GetComponent<Button>().onClick.AddListener(() => OnPowerCardClick());
            playerPowerCards[i].GetComponent<Button>().image.color = new Color(255, 255, 255, 1);
            if (playerPowerCards[i].TryGetComponent(out PowerCard card))
            {
                card.DimPowerCard(true);
            }

           // if (playerPowerCards[i].GetComponent<HiddenPowerCard>())
            {
               // playerHiddenCard = playerPowerCards[i].transform;
               // playerHasHiddenCard = true;
            }
        }
        //instatiate power card for AI
        InstantiateAIPowerCards();

        
       
        foreach (Button card in playerCards)
        {
            card.onClick.AddListener(() => SelectPlayerNumber());
        }

        StartCoroutine(AISelectNumberDelay());
    }

    private void InstatiateCards(Transform cardHolder, List<Button> cardList,bool aiCards=false)
    {
        FillAIHand();
        for (int i = 0; i < maxRound; i++)
        {
            GameObject card = Instantiate(numberCardPrefab, cardHolder);
            card.name = (i + 1).ToString();
            card.GetComponent<Image>().sprite = numberCards[i];
            card.GetComponentInChildren<TMP_Text>().text = (i + 1).ToString();
            cardList.Add(card.GetComponent<Button>());

            if(aiCards)
            {
                card.GetComponent<Image>().sprite = cardbgImage;
            }
        }
    }

    /// <summary>
    /// populate an array as AI hand with all numbers to play the current round
    /// </summary>
    /// 
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(!pausePanel.activeSelf)
            {
                Time.timeScale = 0;
                pausePanel.SetActive(true);
            }
        }
    }
    private void FillAIHand()
    {
        aiHand.Clear();
        for (int i = 0; i < maxRound; i++)
        {
            aiHand.Add(i + 1);
        }
    }

    private void InstantiateAIPowerCards()
    {
        for (int i = 0; i < maxPowerCard; i++)
        {
            int randomCard = 0;
        randomCard:
            if (i == 0)
            {
                randomCard = Random.Range(0, 4);

            }
            else if (i == 1)
            {
                randomCard = Random.Range(4, 8);
            }
            else if (i == 2)
            {
                randomCard = Random.Range(8, 11);
            }

            GameObject powerCard = allPowerCards.Find(x => x.GetComponent<PowerCard>().powerCardData.cardId == randomCard);
            if (powerCard == null)
            {
                goto randomCard;
            }
            GameObject aiCard = Instantiate(powerCard);

           // if (aiCard.GetComponent<HiddenPowerCard>())
            {
             //   aiHiddenCard = aiCard.transform;
               // aiHasHiddenCard = true;
            }
            aiCard.SetActive(true);
            aiCard.GetComponent<Button>().enabled = false;
            aiCard.GetComponent<PowerCard>().DimPowerCard(true);
            aiPowerCardSlots[i].SetActive(true);
            aiCard.transform.SetParent(aiPowerCardSlots[i].transform, false);
            aiPowerCards.Add(aiCard);
        }
    }

    #region AI Action region
    private IEnumerator AISelectNumberDelay()
    {
        float timeDelay = Random.Range(0.5f, 1.5f);
        yield return new WaitForSeconds(3);
        AISelectPowerCard(true);
        yield return new WaitForSeconds(timeDelay);

        aiUsedNumberCard.Add(aiCards[roundPlayed].gameObject);
        aiSelectedNumber = PickAiNumber();
        SoundManager.ins.PlaySfx("CardMove");
        aiCards[roundPlayed].transform.DOMove(aiSelectedCard.rectTransform.position, 0.5f).OnComplete(() => {
            aiCards[roundPlayed].transform.SetParent(canvasTransform);
            aiCards[roundPlayed].gameObject.SetActive(false);
            aiSelectedCard.enabled = true;

            if (playerEyeActivated)
            {
                aiSelectedCard.sprite = numberCards[aiSelectedNumber - 1]; 
            }
            else
            {
                aiSelectedCard.sprite = cardbgImage;
            };

            if (playerSelectedNumber > 0)
            {
                totumAnimator.SetBool("Show", true);
                totumAnimator.SetBool("Continue", false);
            }
        });

        /// update the game status to AI Played

        string statusText = bossName+" plays their card.";
        if(playerSelectedNumber==0)
        {
            statusText += " Please play a card.";
        }
        ShowGameStatus(statusText);

        yield return new WaitUntil(() => aiSelectedNumber == 0);
        if (roundPlayed >= maxRound) yield break;
        StartCoroutine(AISelectNumberDelay());
    }

    private void AISelectPowerCard(bool isBefore = false, bool equalScore=false)
    {
        if (aiPowerCards.Count > 0  && canAIUsePowerCard)
        {
            int randomCard = Random.Range(0, aiPowerCards.Count);
            if (isBefore)
            {
                if (!aiPowerCards[randomCard].GetComponent<PowerCard>().afterReveal)
                {
                    if (aiPowerCards[randomCard].GetComponent<EyePowerCard>() && canAIUsePowerCard)
                    {
                        if (playerSelectedNumber != 0)
                        {
                            canAIUsePowerCard = false;
                            aiPowerCards[randomCard].transform.DOMove(aiSelectedPowerCard.position, 0.5f).OnComplete(
                                () => { aiPowerCards[randomCard].GetComponent<PowerCard>().PlayPowerCard(true); });
                        }
                    }
                }
            }
            else
            {
                if (equalScore && aiPowerCards.Exists(x => x.GetComponent<SignPowerCard>()) && canAIUsePowerCard)
                {
                    canAIUsePowerCard = false;
                    GameObject powerCard = aiPowerCards.Find(x => x.GetComponent<SignPowerCard>());
                    powerCard.transform.DOMove(aiSelectedPowerCard.position, 0.5f).OnComplete(
                                       () => { powerCard.GetComponent<PowerCard>().PlayPowerCard(true); });
                    
                }

                if(roundPlayed>=maxRound/2)
                {
                    if(aiPowerCards.Exists(x => x.GetComponent<SignPowerCard>()) && playerSelectedNumber-aiSelectedNumber== 1 && canAIUsePowerCard)
                    {
                        canAIUsePowerCard = false;
                        GameObject powerCard = aiPowerCards.Find(x => x.GetComponent<SignPowerCard>());
                        powerCard.transform.DOMove(aiSelectedPowerCard.position, 0.5f).OnComplete(
                                           () => { powerCard.GetComponent<PowerCard>().PlayPowerCard(true);});
                        
                    }

                    if (aiPowerCards.Find(x => x.GetComponent<PowerCard>().afterReveal) && canAIUsePowerCard)
                    {
                        GameObject powerCard = aiPowerCards.Find(x => x.GetComponent<PowerCard>().afterReveal);
                        if (powerCard.GetComponent<ContinousPowerCard>() && canAIUsePowerCard)
                        {
                            if (roundPlayed >= (2 * maxRound) / 3)
                            {
                                canAIUsePowerCard = false;
                                powerCard.transform.DOMove(aiSelectedPowerCard.position, 0.5f).OnComplete(
                                    () => { powerCard.GetComponent<PowerCard>().PlayPowerCard(true); });
                            }
                        }
                        else
                        {
                            canAIUsePowerCard = false;
                            powerCard.transform.DOMove(aiSelectedPowerCard.position, 0.5f).OnComplete(
                                () => { powerCard.GetComponent<PowerCard>().PlayPowerCard(true); });                        
                        }
                    }
                }               
            }
        }
    }
    #endregion
    private IEnumerator CheckBiggerNumber()
    {
        yield return new WaitForSeconds(0.5f);

        totumAnimator.GetComponent<Button>().onClick.RemoveAllListeners();
        totumAnimator.GetComponent<Button>().onClick.AddListener(()=>NextRound());

        totumAnimator.SetBool("Show", false);
        totumAnimator.SetBool("Continue", true);

        if (playerSelectedNumber > aiSelectedNumber)
        {
            /// update the game status to Player Won
            AISelectPowerCard();
            ShowGameStatus("You won this round");
            ShowSpeechDialogue(true);
        }
        else if (playerSelectedNumber < aiSelectedNumber)
        {

            /// update the game status to AI Won
            ShowGameStatus(bossName +" won this round");
            ShowSpeechDialogue(false);
        }
        else if(playerSelectedNumber==aiSelectedNumber)
        {
            AISelectPowerCard(isBefore:false,equalScore:true);
            ShowGameStatus("This round is a draw");
            ShowSpeechDialogue(false);
        }

        yield return new WaitForSeconds(3f);
        roundSpeechBox.SetActive(false);
    }

    private void ShowSpeechDialogue(bool isBossLose)
    {
        if(isBossLose)
        {
            bossRoundSpeechText.text = GameManager.cityData.roundDialogues[1];
        }
        else
        {
            bossRoundSpeechText.text = GameManager.cityData.roundDialogues[0]; 
        }
        roundSpeechBox.SetActive(true);
    }

    public void NextRound()
    {
        if (playerSelectedNumber > aiSelectedNumber)
        {
            playerScore++;
            playerScoreText.text = playerScore.ToString();
        }
        else if (playerSelectedNumber < aiSelectedNumber)
        {
            aiScore++;
            aiScoreText.text = aiScore.ToString();
        }

        roundPlayed++;

        if (aiScore-playerScore>maxRound-roundPlayed && playerPowerCards.Count<=0)
        {
            if (SaveSystem.GetActiveCityData(GameManager.routeData.nodeId).isPhaseCompleted)
            {
                SaveSystem.IncreaseTimeLoss(GameManager.routeData.nodeId);
                winPanel.GetComponentInChildren<TMP_Text>().text = "Game Lost!";
                bossGameLoseText.text = GameManager.cityData.gameDialogues[0];
                winPanel.SetActive(true);
                return;
            }
            SaveSystem.IncreaseTimeLoss(GameManager.routeData.nodeId);
            SaveSystem.SaveCityData(false, false, false, GameManager.routeData.nodeId);
            losePanel.SetActive(true);
            RemovePowerCard();
            SetPhaseStatus(false, false, false, false);
            return;
        }

        selectedPlayerObject.SetActive(true);
        selectedPlayerObject.transform.DOMove(playerUsedCards.position, 0.5f).OnComplete(()=>{ selectedPlayerObject.transform.SetParent(playerUsedCards); });

        ResetGameStats();

        SetGameStatus();

        if (roundPlayed >= maxRound)
        {
            bossAvatarImg.enabled = false;
           
            if (SaveSystem.GetActiveCityData(GameManager.routeData.nodeId).isPhaseCompleted)
            {
                winPanel.GetComponentInChildren<TMP_Text>().text = "Game";
                if (GameManager.routeData.nodeId == 14)
                {
                    Debug.Log("stats");
                    statsButton.SetActive(true);
                }
                else
                {
                    Debug.Log("NodeId " + GameManager.routeData.nodeId);
                }
                winPanel.SetActive(true);
                return;
            }
            if (playerScore > aiScore)
            {
                /// update the game status to Player Won
                
                if(GameManager.routeData.nodeId==14)
                {
                    statsButton.SetActive(true);
                }
                else
                {
                    Debug.Log("NodeId "+GameManager.routeData.nodeId);
                }
                SaveSystem.IncreaseTimeWon(GameManager.routeData.nodeId);
                SaveSystem.SaveCityData(true, true, true, GameManager.routeData.nodeId);
                winPanel.SetActive(true);
                SetPhaseStatus(true);
            }
            else if (playerScore < aiScore)
            {
                if (SaveSystem.GetActiveCityData(GameManager.routeData.nodeId).isPhaseCompleted)
                {
                    SaveSystem.IncreaseTimeLoss(GameManager.routeData.nodeId);
                    winPanel.GetComponentInChildren<TMP_Text>().text = "Game Lost!";
                    bossGameLoseText.text = GameManager.cityData.gameDialogues[0];
                    winPanel.SetActive(true);
                    SaveSystem.SetCityActive(GameManager.routeData.nodeId);
                }
                else
                {
                    SaveSystem.IncreaseTimeLoss(GameManager.routeData.nodeId);
                    RemovePowerCard();
                    SaveSystem.SaveCityData(false, false, false, GameManager.routeData.nodeId);
                    losePanel.SetActive(true);
                    SetPhaseStatus(false, false, false, false);
                }
                /// update the game status to AI Won
            }
            else
            {
                if (GameManager.routeData.nodeId == 14)
                {
                    statsButton.SetActive(true);
                }
                else
                {
                    Debug.Log("NodeId " + GameManager.routeData.nodeId);
                }
                /// update the game status to Draw
                SaveSystem.IncreaseTimeDraw(GameManager.routeData.nodeId);
                SaveSystem.SaveCityData(false, true, true, GameManager.routeData.nodeId);
                winPanel.SetActive(true);
                winPanel.GetComponentInChildren<TMP_Text>().text = "Game Draw";
                SetPhaseStatus(true);
            }           
            SaveSystem.SaveCardGameRecords(currentCardGameRecord);        
        }
    }

    private void RemovePowerCard()
    {
        Debug.Log("Removed PowerCard");
        int phaseNo = GameManager.cityData.phase.phaseId;
        int start = 0;
        int end = 0;
        if (phaseNo == 1)
        {
            start = 0;
            end = 4;
        }
        else if (phaseNo == 2)
        {
            start = 4;
            end = 8;
        }
        else if (phaseNo == 3)
        {
            start = 8;
            end = 11;
        }

        for (int i = start; i < end; i++)
        {
            SaveSystem.SavePowerCardData(i, false);
        }
    }

    private void SetPhaseStatus(bool isPhaseCompleted, bool isActive=false, bool isPlayed=false, bool isWon=false)
    {
        List<int> nodeIds = new List<int>();
        int activeNode = -1;
        if (GameManager.cityData.phase.phaseId == 1)
        {
            for (int i = 0; i < 4; i++)
            {
                nodeIds.Add(i);
                if(!isPhaseCompleted)
                {
                    SaveSystem.ResetTimesDraw(i);
                    SaveSystem.ResetTimesLose(i);
                    SaveSystem.ResetTimesWon(i);
                }
            }
        }
        else if (GameManager.cityData.phase.phaseId == 2)
        {
            for (int i = 4; i < 8; i++)
            {
                nodeIds.Add(i);
                if (!isPhaseCompleted)
                {
                    SaveSystem.ResetTimesDraw(i);
                    SaveSystem.ResetTimesLose(i);
                    SaveSystem.ResetTimesWon(i);
                }
            }
        }
        else if (GameManager.cityData.phase.phaseId == 3)
        {
            for (int i = 8; i < 11; i++)
            {
                nodeIds.Add(i);
                if (!isPhaseCompleted)
                {
                    SaveSystem.ResetTimesDraw(i);
                    SaveSystem.ResetTimesLose(i);
                    SaveSystem.ResetTimesWon(i);
                }
            }
        }
        nodeIds.Add(GameManager.routeData.nodeId);
        if (!isPhaseCompleted)
        {
            if (GameManager.cityData.index>11)
            {
                activeNode = GameManager.cityData.index - 1;
            }        
        }
        SaveSystem.SavePhaseData(nodeIds,isPhaseCompleted,isActive,isPlayed,isWon,activeNode);
    }

    private void ResetGameStats()
    {
        totumAnimator.GetComponent<Button>().onClick.RemoveAllListeners();
        totumAnimator.GetComponent<Button>().onClick.AddListener(() => ShowCards());


        if (positionSwaped)
        {
            Vector3 temp = playerSelectedCard.rectTransform.position;
            playerSelectedCard.rectTransform.position = aiSelectedCard.rectTransform.position;
            aiSelectedCard.rectTransform.position = temp;
        }

        if (playerUsedPowerCard.Count > 0)
        {
            foreach (GameObject card in playerUsedPowerCard)
            {
                card.transform.SetParent(usedCard, false);
                card.gameObject.SetActive(false);
            }
        }

        if (aiUsedPowerCard.Count > 0)
        {
            foreach (GameObject card in aiUsedPowerCard)
            {
                card.transform.SetParent(usedCard, false);
                card.gameObject.SetActive(false);
            }
        }

        ShowGameStatus();

        playerSelectedNumber = 0;
        aiSelectedNumber = 0;
        totumAnimator.SetBool("Show", false);
        totumAnimator.SetBool("Continue", false);

        isShowing = false;
        playerSelectedCard.enabled = false;
        aiSelectedCard.enabled = false;
        canUsePowerCard = true;
        canAIUsePowerCard = true;
        positionSwaped = false;
        playerEyeActivated = false;
        aiDesiredNum = -1;
    }

    /// <summary>
    /// changes the game status TMP (if exists) with current status text
    /// </summary>
    /// <param name="message"> current status to show as game status</param>
    private void ShowGameStatus(string message = "")
    {
        /// check wether the game status text is valid to send the status feedback
        if (gameStatusText != null)
        {
            gameStatusText.text = message ?? "";
        }
        else
        {
            Debug.LogWarning("Assign a TextMeshPro component to Games Status Text field in Card Game Controller");
        }
    }

    /// <summary>
    /// sets the current player's winning status: Ahead, Tie, Behind
    /// </summary>
    private void SetGameStatus()
    {
        if (aiScore > playerScore)
        {
            currentPlayerWinningState = CardRoundSaveData.GameState.Behind;
        }
        else if (aiScore < playerScore)
        {
            currentPlayerWinningState = CardRoundSaveData.GameState.Ahead;
        }
        else if (aiScore == playerScore)
        {
            currentPlayerWinningState = CardRoundSaveData.GameState.Tie;
        }
    }

    #endregion

    #region AI Number Card Picking Logic


    /// <summary>
    /// returns the AI choise for the current round
    /// </summary>
    /// <returns></returns>
    private int PickAiNumber()
    {
        if (aiHand.Count == 0)
        {
            LogAIThought("my Hand is empty :| ");
            return 0;
        }

        if (aiDesiredNum > 0 && aiHand.Contains(aiDesiredNum))
        {
            aiHand.Remove(aiDesiredNum);
           // aiUsedNumberCard.Add(aiCards.Find(x=>x.GetComponentInChildren<Text>().text==aiDesiredNum.ToString()).gameObject);
            return aiDesiredNum;
        }

        int chosenNum;
        if (oldRecords.Count > 0)
        {
            int dominNum = GetDominantNumber();
            if (dominNum > 0)
            {
                //Debug.Log("AI: domin is : " + dominNum.ToString());
                if (aiHand.Contains(dominNum + 1))
                {
                    chosenNum = dominNum + 1;
                    LogAIThought("domin is : " + dominNum.ToString() + " so I play " + chosenNum.ToString());
                }
                else if (dominNum == maxRound)
                {
                    chosenNum = PickLowest();
                    LogAIThought("domin is : " + dominNum.ToString() + " which is max so I play the lowest possible " + chosenNum.ToString());
                }
                else if (currentPlayerWinningState == CardRoundSaveData.GameState.Tie ||
                         currentPlayerWinningState == CardRoundSaveData.GameState.Behind)
                {
                    chosenNum = PickLowest();
                    LogAIThought("domin is : " + dominNum.ToString() + " and I don't have the " + (dominNum + 1) + " in my hand and I'm not losing so I play lowest: " + chosenNum.ToString());
                }
                else
                {
                    chosenNum = PickNextHigherAvailable(dominNum + 1);
                    LogAIThought(dominNum.ToString() + " + 1 is not in may hand and I am losing so I play the next highest possible which is " + chosenNum.ToString());
                }
            }
            else
            {
                chosenNum = PickRandom();
                LogAIThought("could not find a valid domin number, so I play random: " + chosenNum.ToString());
            }
        }
        else
        {
            chosenNum = PickRandom();
            LogAIThought("no older data so I play random: " + chosenNum.ToString());
        }

        

        /// validate the chosen number and return it
        if (aiHand.Contains(chosenNum) && chosenNum > 0)
        {

            aiHand.Remove(chosenNum);
           // aiUsedNumberCard.Add(aiCards.Find(x => x.GetComponentInChildren<Text>().text == chosenNum.ToString()).gameObject);
            return chosenNum;
        }
        else
        {
            Debug.LogError("chosen number is not in AI hand");
        }
        return 0;
    }

    /// <summary>
    /// pick a random number card from ai hand (possible cards)
    /// </summary>
    /// <returns></returns>
    private int PickRandom()
    {
        return aiHand[UnityEngine.Random.Range(0, aiHand.Count)];
    }

    /// <summary>
    /// Picks the lowest possible number in AI hand
    /// </summary>
    /// <returns></returns>
    private int PickLowest()
    {
        if (aiHand.Count > 0)
        {
            aiHand.Sort();
            return aiHand[0];
        }
        else
        {
            LogErrorPicking();
            return 0;
        }
    }

    /// <summary>
    /// logs the given string from AI if logAIthoughs is true
    /// </summary>
    /// <param name="msg"></param>
    private void LogAIThought(string msg = "hmm... I'm thinking...")
    {
        if (logAIthoughts)
        {
            Debug.Log("AI: " + msg);
        }
    }

    /// <summary>
    /// Picks the next higher value than given number in AI hand
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    private int PickNextHigherAvailable(int num)
    {
        if (aiHand.Count > 0)
        {
            List<int> highers = aiHand.Where(i => i > num).ToList();
            if (highers != null && highers.Count > 0)
            {
                highers.Sort();
                return highers[0];
            }
            return PickLowest();
        }
        LogErrorPicking();
        return 0;
    }

    /// <summary>
    /// Logs given msg as Error
    /// </summary>
    /// <param name="msg"></param>
    private void LogErrorPicking(string msg = "Couldn't find the right number in AI hand")
    {
        Debug.LogError(msg);
    }

    /// <summary>
    /// returns the most frequently played number by player in the current state
    /// according to the old recorded data
    /// </summary>
    /// <returns></returns>
    private int GetDominantNumber()
    {
        int domin = 0;
        if (oldRecords.Count > 0)
        {
            /// populate list of recorded numbers for this round and state
            Dictionary<int, int> stateNums = new();
            Dictionary<int, int> roundNums = new();
            for (int i = 1; i <= maxRound; i++)
            {
                stateNums.Add(i, 0);
                roundNums.Add(i, 0);
            }

            foreach (CardRoundSaveData d in oldRecords)
            {
                /// check if this record has the same size of cards we are playing
                if (d.roundNum == maxRound)
                {
                    foreach (CardRoundRecord r in d.records)
                    {
                        /// check if this round is the same round we are playing
                        if (r.round == roundPlayed)
                        {
                            if (roundNums.ContainsKey(r.playedNumber))
                            {
                                roundNums[r.playedNumber]++;

                            }
                            if (r.state == currentPlayerWinningState && stateNums.ContainsKey(r.playedNumber))
                            {
                                stateNums[r.playedNumber]++;
                            }
                        }
                    }
                }
            }

            /// find the domin number 
            List<KeyValuePair<int, int>> stateList = stateNums.ToList();
            stateList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
            if (stateList.Count > 1 && stateList[0].Value > 0 && stateList[0].Value > stateList[1].Value)
            {
                return stateList[0].Key;
            }
            else
            {
                /// no valid domin number in current state so find the round domin number
                List<KeyValuePair<int, int>> roundList = roundNums.ToList();
                roundList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
                if (roundList.Count > 1 && roundList[0].Value > 0 && roundList[0].Value > roundList[1].Value)
                {
                    return roundList[0].Key;
                }
            }
        }
        return domin;
    }



    #endregion

    #region ----------------PUBLIC FUNCTIONS---------------------

    #region Player click actions
    public void SelectPlayerNumber()
    {
        GameObject currentGameobject = EventSystem.current.currentSelectedGameObject;
        
        if (selectedPlayerObject == null && playerSelectedNumber==0)
        {
            selectedPlayerObject = currentGameobject;

            //currentGameobject.transform.DOMoveY(250, 0.5f);
            currentGameobject.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.25f);
            //currentGameobject.transform.DORotateQuaternion(Quaternion.Euler(0,0,0), 0.5f);
        }
        else if (selectedPlayerObject != currentGameobject && playerSelectedNumber==0)
        {
            selectedPlayerObject.transform.DOScale(new Vector3(1, 1, 1), 0.25f);
           // selectedPlayerObject.transform.DOMoveY(170f, 0.5f);
            selectedPlayerObject = currentGameobject;

            //currentGameobject.transform.DOMoveY(250, 0.5f);
            currentGameobject.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.25f);
            //currentGameobject.transform.DORotateQuaternion(Quaternion.Euler(0, 0, 0), 0.5f);
        }
        else if (selectedPlayerObject == currentGameobject)
        {
            if (playerSelectedNumber == 0)
            {
                SoundManager.ins.PlaySfx("CardMove");
                currentGameobject.transform.DOScale(new Vector3(1f, 1f, 1f), 0.25f);
                currentGameobject.transform.DOMove(playerSelectedCard.rectTransform.position, 0.5f).OnComplete(()=>{
                    currentGameobject.transform.SetParent(canvasTransform);
                    currentGameobject.SetActive(false);
                    playerSelectedCard.enabled = true;
                    playerSelectedCard.sprite = cardbgImage;
                   
                    if(aiSelectedNumber>0)
                    {
                        totumAnimator.SetBool("Show", true);
                        totumAnimator.SetBool("Continue", false);
                    }                   
                  
                });
                playerSelectedNumber = int.Parse(EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TMP_Text>().text);
                playerUsedNumberCard.Add(EventSystem.current.currentSelectedGameObject);
                /// update the game status to player played

                string statusText =  "You played your turn.";
                if (aiSelectedNumber == 0)
                {
                    statusText += string.Format("Waiting for {0}'s turn.",bossName);
                }

                ShowGameStatus(statusText);

                
                EventSystem.current.currentSelectedGameObject.GetComponent<Button>().interactable = false;
            }

            /// Record the player played card 
            if (aiScore == playerScore)
            {
                currentCardGameRecord.records[roundPlayed].state = CardRoundSaveData.GameState.Tie;
            }
            else if (aiScore > playerScore)
            {
                currentCardGameRecord.records[roundPlayed].state = CardRoundSaveData.GameState.Behind;
            }
            else if (aiScore < playerScore)
            {
                currentCardGameRecord.records[roundPlayed].state = CardRoundSaveData.GameState.Ahead;
            }
            currentCardGameRecord.records[roundPlayed].playedNumber = playerSelectedNumber;
            currentCardGameRecord.records[roundPlayed].round = roundPlayed;
        }
    }

    public void OnPowerCardClick()
    {
        SoundManager.ins.ClickSFX();
        if (!canUsePowerCard) return;


        GameObject currentGameobject = EventSystem.current.currentSelectedGameObject;
        if (selectedPowerCard == null)
        {
            PowerCard card = currentGameobject.GetComponent<PowerCard>();

            if (card.afterReveal)
            {
                if (!isShowing) return;
            }
            else
            {
                if (isShowing) return;
            }
            selectedPowerCard = currentGameobject;
            currentGameobject.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.25f);

            /// Activate Select animation
            card.RunSelectAnimation(true);
        }
        else if (selectedPowerCard != currentGameobject)
        {
            PowerCard card = currentGameobject.GetComponent<PowerCard>();
            if (card.afterReveal)
            {
                if (!isShowing) return;
            }
            else
            {
                if (isShowing) return;
            }

            selectedPowerCard.transform.DOScale(new Vector3(1, 1, 1), 0.25f);

            /// Deactivate the power card animation.
            if (selectedPowerCard.TryGetComponent(out PowerCard oldCard))
            {
                oldCard.RunSelectAnimation(false);
            }
            selectedPowerCard = currentGameobject;
            currentGameobject.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.25f);

            /// Activate Select animation
            card.RunSelectAnimation(true);
        }
        else if (selectedPowerCard == currentGameobject)
        {
            selectedPowerCard.transform.DOScale(new Vector3(1, 1, 1), 0.1f);
            PowerCard card = currentGameobject.GetComponent<PowerCard>();
            /// Deactivate the power card animation.
            if (selectedPowerCard.TryGetComponent(out PowerCard oldCard))
            {
                oldCard.RunSelectAnimation(false);
            }

            canUsePowerCard = false;
            playerSelectedPowerCard.gameObject.GetComponent<Image>().enabled = false;
            selectedPowerCard.GetComponent<PowerCard>().PlayPowerCard();
        }
    }

    public void ShowCards()
    {
        if (playerSelectedNumber == 0 || aiSelectedNumber == 0) return;
        if (!isShowing)
        {
            SoundManager.ins.ClickSFX();          
            isShowing = true;
            /// update the game status to card revealed
            ShowGameStatus("Card Revealed");

            playerSelectedCard.sprite = numberCards[playerSelectedNumber - 1];
            aiSelectedCard.sprite = numberCards[aiSelectedNumber - 1];
            StartCoroutine(CheckBiggerNumber());
        }
    }

    #endregion

    #region Power Cards
    public void ContinueCardPower()
    {
        SoundManager.ins.PlaySfx("Continious");
        roundPlayed = 0;
        ResetGameStats();
        FillAIHand();
        selectedPlayerObject = null;
        ShowGameStatus(" All cards are reset.");
        foreach (Button card in playerCards)
        {
            card.interactable = true;
            card.gameObject.SetActive(true);
            card.transform.SetParent(playerCardHolder);
        }

        foreach (Button card in aiCards)
        {
            card.gameObject.SetActive(true);
            card.transform.SetParent(aiCardHolder);
        }
    }

    public void SignCardPower(bool aiUsingCard=false)
    {
        if(aiUsingCard)
        {
            aiSelectedNumber = aiSelectedNumber + 1;
            aiSelectedCard.sprite = numberCards[aiSelectedNumber - 1];
            ShowGameStatus(bossName+" used Sign. Their card's value increases by 1.");
        }

        else
        {
            playerSelectedNumber = playerSelectedNumber + 1;
            playerSelectedCard.sprite = numberCards[playerSelectedNumber - 1];
            ShowGameStatus("You used Sign. Your card's value increases by 1.");
        }
        StartCoroutine(CheckBiggerNumber());
    }

    public void EyeCardPower(bool aiUsingCard = false)
    {
        if (aiUsingCard)
        {
            Debug.Log("AI Played Eye Card");
          //  if (playerHasHiddenCard)
            {
            //    HiddenCardPower();
              //  return;
            }
            if (playerSelectedNumber != 0)
            {
                if (aiHand.Find(x => x > playerSelectedNumber) != 0)
                {
                    aiDesiredNum = aiHand.Find(x => x > playerSelectedNumber);
                }
                else if (aiHand.Find(x => x == playerSelectedNumber) != 0)
                {
                    aiDesiredNum = aiHand.Find(x => x == playerSelectedNumber);
                }
                else
                {
                    aiDesiredNum = PickLowest();
                }
                ShowGameStatus(bossName+" used Eye. Your card is revealed.");
            }
        }
        else
        {
           // if (aiHasHiddenCard)
            {
             //   HiddenCardPower(true);
               // return;
            }
            playerEyeActivated = true;
            if (aiSelectedNumber != 0)
            {
                aiSelectedCard.sprite = numberCards[aiSelectedNumber - 1];
                ShowGameStatus("You used Eye. The opponent's card is revealed.");
            }
        }
    }

    public void HiddenCardPower(bool aiUsingCard = false)
    {
        if (aiUsingCard)
        {
           // aiHiddenCard.DOMove(aiSelectedPowerCard.position, 0.5f);
            //aiUsedPowerCard.Add(aiHiddenCard.gameObject);
            //aiPowerCards.Remove(aiHiddenCard.gameObject);
        }
        else
        {
           // playerHiddenCard.DOMove(playerSelectedPowerCard.position, 0.5f);
            //playerUsedPowerCard.Add(playerHiddenCard.gameObject);
            //playerPowerCards.Remove(playerHiddenCard.gameObject);
        }
        Debug.Log("Played Hidden Card");
    }

    public void TradeCardPower(bool aiUsingCard = false)
    {
        if (isShowing)
        {
            SoundManager.ins.PlaySfx("Trade");
            int tmpNumber = 0;
            tmpNumber = playerSelectedNumber;
            playerSelectedNumber = aiSelectedNumber;
            aiSelectedNumber = tmpNumber;

            Vector3[] waypointsP = new Vector3[] { new Vector3 { x=821,y=640,z=0 },aiSelectedCard.rectTransform.position };
            Vector3[] waypointsA = new Vector3[] { new Vector3 { x = 1321, y = 640, z = 0 }, playerSelectedCard.rectTransform.position };
            playerSelectedCard.transform.DOPath(waypointsP, 1f, pathType: PathType.CatmullRom);
            aiSelectedCard.transform.DOPath(waypointsA, 1f, pathType: PathType.CatmullRom).OnComplete(()=> { StartCoroutine(CheckBiggerNumber()); });
            positionSwaped = !positionSwaped;

            if(aiUsingCard)
            {
                ShowGameStatus(bossName+ " used Trade. Your cards are swapped.");
            }
            else
            {
                ShowGameStatus("You used Trade. Your cards are swapped.");
            }
        }
    }

    public void DeadCardPower(bool aiUsingCard = false)
    {
        Debug.Log("dead Card");
        if (isShowing)
        {
            SoundManager.ins.PlaySfx("DeathCard");
            if (aiUsingCard)
            {
                playerSelectedNumber = -1;
                playerSelectedCard.sprite = deadCard;
                ShowGameStatus(bossName+ " used Dead. Your card is destroyed.");
            }
            else
            {
                aiSelectedNumber = -1;
                aiSelectedCard.sprite = deadCard;
                ShowGameStatus("You used Dead. "+ bossName+"'s card is destroyed.");
            }
        }

        StartCoroutine(CheckBiggerNumber());
    }

    public void ShowInventory(bool state)
    {
        if (availablePowerCardPopup != null)
        {
            if (availablePowerCardPopup.TryGetComponent(out Animator anim))
            {
                anim.SetBool("Active", state);
            }
        }
    }

    #endregion

    public void LoadStatsScene()
    {
        SoundManager.ins.PlaySfx("BackButton");
        Time.timeScale = 1;
        LevelLoader.StatsScreen();
    }

    public void LoadMapScene()
    {
        SoundManager.ins.PlaySfx("BackButton");
        Time.timeScale = 1;
        LevelLoader.MapScreen();
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