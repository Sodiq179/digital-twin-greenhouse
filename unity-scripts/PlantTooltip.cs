using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

 public class PlantTooltip : MonoBehaviour
 {

    // public TextMeshProUGUI contentFieldPlant;

    private void OnMouseEnter()
    {
        DateTime currentTime = DateTime.Now;
        PlantTooltipManager._instance.SetAndShowToolTip($"Plant Height: 1cm\nLeaf Length: 1cm\nLeaf Width: 1cm\nCrown Diameter: 1cm\nUpdated at: {currentTime}");
    }

    private void OnMouseExit()
    {
        PlantTooltipManager._instance.HideToolTip();
    }
 }