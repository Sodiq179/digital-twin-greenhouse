using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void SwitchTo3DScene()
    {
        SceneManager.LoadScene("GreenhouseScene"); // Replace "3DSceneName" with the name of your 3D scene
    }
}
