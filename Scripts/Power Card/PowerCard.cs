using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public abstract class PowerCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] public PowerCardData powerCardData;
    private CardGameController cardGameController;
    public bool afterReveal;

    /// <summary>
    /// power card image
    /// </summary>
    [SerializeField] private Image image = null;

    /// <summary>
    /// TooltipUI prefab that should be visible when the cursor goes over the power card
    /// </summary>
    [SerializeField] private TooltipUI tooltipUI = null;

    /// <summary>
    /// the power card's rect transrom
    /// </summary>
    [SerializeField] private RectTransform rect = null;

    /// <summary>
    /// Animator thath handles power card animations
    /// </summary>
    [SerializeField, Tooltip("Animator thath handles power card animations.\nparameters:\nbool:\"Select\"")]
    private Animator animator = null;

    /// <summary>
    /// Bool parameter to trigger the Select animation
    /// </summary>
    [SerializeField, Tooltip("Bool parameter to trigger the Select animation")]
    private string selectBoolParamLabel = "Select";

    /// <summary>
    /// the instance object of tooltipUI prefab
    /// </summary>
    private TooltipUI tooltipInstance = null;

    private void Awake()
    {
        if (rect == null && !TryGetComponent(out rect))
        {
            Debug.LogWarning("Rect Transform not found in power card " + this.gameObject.name);
        }
        if (animator == null && !TryGetComponent<Animator>(out animator))
        {
            Debug.LogWarning("animator not found in power card " + this.gameObject.name);
        }
        if (image == null && !TryGetComponent(out image))
        {
            Debug.LogWarning("Image not found in power card " + this.gameObject.name);
        }
    }

    public virtual void PlayPowerCard(bool ai=false)
    {
        enabled = false;
        cardGameController = FindObjectOfType<CardGameController>();
        if(ai)
        {
            cardGameController.aiPowerCards.Remove(this.gameObject);
            cardGameController.aiUsedPowerCard.Add(this.gameObject);

        }
        else
        {
            cardGameController.playerPowerCards.Remove(this.gameObject);
            cardGameController.playerUsedPowerCard.Add(this.gameObject);
            SaveSystem.SavePowerCardData(powerCardData.cardId, isAcquired: true);

        }

        /// destory tooltip instance if exists
        if (tooltipInstance != null)
        {
            Destroy(tooltipInstance.gameObject);
        }
    }

    /// <summary>
    /// called when pointer goes over this object
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        ToggleTooltip(true);
    }

    /// <summary>
    /// called when the pointer exists this object
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        ToggleTooltip(false);
    }

    /// <summary>
    /// turns the power card's select(activated) animation On/Off 
    /// </summary>
    /// <param name="state"></param>
    public void RunSelectAnimation (bool state)
    {
        if (animator != null)
        {
            animator.SetBool(selectBoolParamLabel, state);
        }
    }

    public void DimPowerCard(bool value)
    {
        if (image != null)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, value ? 1f : 0.5f);
        }
    }

    /// <summary>
    /// sets the tool tip content and visibility of the the panel according to the given state
    /// </summary>
    /// <param name="state"></param>
    private void ToggleTooltip(bool state)
    {
        if (tooltipUI != null)
        {
            /// instantiate the tooltip canvs if not exists and state is true
            if (tooltipInstance == null)
            {
                if (state)
                {
                    tooltipInstance = Instantiate(tooltipUI) as TooltipUI;
                }
            }
            
            if (powerCardData != null)
            {
                if (tooltipInstance == null)
                {
                    Debug.LogError("couldn't instantiate or find tooltip instance for power card " + powerCardData.name);
                    return;
                }

                /// set the content and postition of the tooltip panel
                string content = state ? powerCardData.name + " : " + powerCardData.description : string.Empty;
                Vector2 pos = rect != null ? rect.position : Vector2.zero;
                tooltipInstance.SetTooltip(state, pos, content);
            }
            else
            {
                Debug.LogWarning("no power card data found to show the tooltip for");
            }
        }
        else
        {
            Debug.LogWarning("tooltip canvas prefab not found");
        }
    }

}
