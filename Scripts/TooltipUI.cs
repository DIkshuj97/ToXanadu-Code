using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class TooltipUI : MonoBehaviour
{
    [SerializeField] private Canvas canvas = null;
    [SerializeField] private RectTransform rect = null;
    [SerializeField] private TextMeshProUGUI tmp = null;
   // public bool onToolTip = false;

    public void SetTooltip(bool _state, Vector2 _pos, string _text)
    {
        /// Validation
        /// 
        if (rect == null)
        {
            Debug.LogError("Canvas not found in tooltip ui");
        }

        if (rect == null)
        {
            Debug.LogError("rect transform not found in tooltip ui");
        }

        if (tmp == null)
        {
            Debug.LogError("text mesh pro component not found in tooltip ui");
        }

        /// position the panel in bound
        Vector2 position = _pos;

        /// set text
        if (_state)
        {
            tmp.text = _text;
            rect.ForceUpdateRectTransforms();


            /// check if panel is out of bound from left 
            if (_pos.x - (rect.pivot.x * rect.sizeDelta.x) < 0)
            {
                rect.pivot = new Vector2(0, rect.pivot.y);
            }

            /// check if panel is out of bound from right
            else if (_pos.x + rect.sizeDelta.x > Screen.width)
            {
                rect.pivot = new Vector2(1, rect.pivot.y);
            }

            /// check if panel is out of bound from top
            if (_pos.y - (rect.pivot.y * rect.sizeDelta.y) < 0)
            {
                rect.pivot = new Vector2(rect.pivot.x, 0);
            }

            /// check if panel is out of bound from button
            else if (_pos.y + rect.sizeDelta.y > Screen.height)
            {
                rect.pivot = new Vector2(rect.pivot.x, 1);
            }
        }
        else
        {
            /// reset text and panel position
            tmp.text = string.Empty;
            rect.pivot = new(1, 0);
        }

        /// update and turn canvas
        rect.position = position;
        rect.ForceUpdateRectTransforms();
        canvas.enabled = _state;
    }
}
