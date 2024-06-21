/// 
/// Author: Lucas Storm
/// June 2024
/// Bugs: None known at this time.
/// 
/// This script handles button presses on the main menu.

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{

    public TextMeshProUGUI scoreText;

    private void Start()
    {
        if (UIManager.Instance != null)
        {
            // Update score
            scoreText.text = "Score: " + UIManager.Instance.score.ToString();
        }
        
    }

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
