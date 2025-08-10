using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

 public class Tooltip : MonoBehaviour
 {
    // public string message;

    public TextMeshProUGUI contentField;

    private void OnMouseEnter()
    {
        TooltipManager._instance.SetAndShowToolTip(contentField.text);
    }

    private void OnMouseExit()
    {
        TooltipManager._instance.HideToolTip();
    }
 }