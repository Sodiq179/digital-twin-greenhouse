using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonHandler2 : MonoBehaviour
{
    void OnMouseDown()
    {
        SceneManager.LoadScene("GUIScene"); // Replace "GUISceneName" with the actual name of your GUI scene
    }
}
