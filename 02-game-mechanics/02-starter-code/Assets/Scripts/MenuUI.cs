using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    public void OnStartButtonClicked()
    {
        // Load Scene 1
        SceneManager.LoadScene(1);
    }

    public void OnQuitButtonClicked()
    {
        // Quit the application or stop play mode if in the editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
