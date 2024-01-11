using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerCardButton : MonoBehaviour
{
    public int selectedPowerCardNumber;
    public CardGameController controller;
    public List<Button> powerCardButtons;
    public TMP_Text powerCardNumberText;
    public Animator powerCardAnimator;
    public bool isMapInventory=false;

    private void Awake()
    {
        if(isMapInventory)
        {
            if(File.Exists(Application.persistentDataPath + "/gamedata.txt"))
            {    
            List<int> powerCards = SaveSystem.GetPowerCard();
            foreach (int card in powerCards)
            {
                if (powerCardButtons.Exists(x => x.GetComponent<PowerCard>().powerCardData.cardId == card))
                {
                    Button powerCard = powerCardButtons.Find(x => x.GetComponent<PowerCard>().powerCardData.cardId == card);
                    powerCard.gameObject.SetActive(true);
                    powerCard.GetComponent<PowerCard>().DimPowerCard(true);
                }
            }

            powerCardNumberText.text = "";
            }
        }

        else
        {
            foreach (Button powerCard in powerCardButtons)
            {

                powerCard.onClick.AddListener(() => SelectORUnselectCard());
            }
            powerCardNumberText.text = selectedPowerCardNumber.ToString() + " / " + GameManager.cityData.phase.powerCards.ToString();
        }
    }


    public void SelectORUnselectCard()
    {       
        GameObject selectedGameObject = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        SoundManager.ins.ClickSFX();
        if (selectedGameObject.TryGetComponent(out PowerCard selectedPowerCard))
        {
            if (controller.playerPowerCards.Contains(selectedGameObject))
            {
                selectedPowerCardNumber--;
                controller.playerPowerCards.Remove(selectedGameObject);
                //selectedGameObject.GetComponent<Button>().image.color = new Color(255f, 255f, 255f, 255f);
                selectedPowerCard.DimPowerCard(false);
            }
            else if (!controller.playerPowerCards.Contains(selectedGameObject))
            {
                if (selectedPowerCardNumber < GameManager.cityData.phase.powerCards)
                {
                    selectedPowerCardNumber++;
                    controller.playerPowerCards.Add(selectedGameObject);
                    //selectedGameObject.GetComponent<Button>().image.color = new Color(255f, 255f, 255f, 0.5f);
                    selectedPowerCard.DimPowerCard(true);
                }
            }
        }
        

        powerCardNumberText.text = selectedPowerCardNumber.ToString() + " / " + GameManager.cityData.phase.powerCards.ToString();
    }

    public void ShowInventory()
    {
        powerCardAnimator.SetBool("Active", true);
    }

    public void CloseInventory()
    {
        powerCardAnimator.SetBool("Active", false);
    }
}