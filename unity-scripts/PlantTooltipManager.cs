using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlantTooltipManager : MonoBehaviour
{
    public static PlantTooltipManager _instance;

    public TextMeshProUGUI textComponentPlant;
    

    public void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        Cursor.visible = true;
        gameObject.SetActive(false);

    }

    void Update()
    {
        transform.position = Input.mousePosition;

    }

    public void SetAndShowToolTip(string message)
    {
        gameObject.SetActive(true);
        textComponentPlant.text = message;
    }

    public void HideToolTip()
    {
        gameObject.SetActive(false);
        textComponentPlant.text = string.Empty;
    }

}
