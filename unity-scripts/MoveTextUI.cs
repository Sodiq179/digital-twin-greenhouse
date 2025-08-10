using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoveTextUI : MonoBehaviour
{
    // Reference to the Text UI element
    public TextMeshProUGUI textUI;

    void Start()
    {
        // Set the position of the Text UI element
        textUI.rectTransform.position = new Vector3(-0.3443775f, -0.1572093f, 4.504f); // Example position (x, y, z)
    }
}
