using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DropdownScript : MonoBehaviour
{
    [SerializeField] private TMP_Text numberText;


    public void DropdownSample(int index)
    {
        switch (index)
        {
            case 0: numberText.text = "Month: February"; break;
            case 1: numberText.text = "Month: March"; break;
            case 2: numberText.text = "Month: April"; break;
            case 3: numberText.text = "Month: May"; break;
            case 4: numberText.text = "Month: June"; break;

        }
    }


}

