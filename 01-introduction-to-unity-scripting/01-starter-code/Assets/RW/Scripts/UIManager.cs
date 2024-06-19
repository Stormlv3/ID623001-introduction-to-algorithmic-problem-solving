using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI scoreText;
    public Image[] hearts;

    public int score = 0; // Initial score
    private int lifeCount; // Number of lives

    void Awake()
    {
        // Ensure that there's only one instance of UIManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        lifeCount = hearts.Length;
        UpdateScoreText();
    }

    public void IncreaseScore()
    {
        // Add one to the score count
        score += 1;
        UpdateScoreText();
    }

    // Method to update the score display
    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + score.ToString();
    }

    // Method to update the lifes when one is lost
    public void LifeLost()
    {
        // Make sure there are lifes left
        if (lifeCount > 0)
        {
            // Remove a life from counter
            lifeCount -= 1;
            // Hide one heart when life is lost
            hearts[lifeCount].enabled = false;
        }
    }
}
