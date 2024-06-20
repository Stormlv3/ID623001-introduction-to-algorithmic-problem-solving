using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    public EnemySpawner EnemySpawner;
    [SerializeField] private Text goldLabel;
    [SerializeField] private Text healthLabel;
    [SerializeField] private Text waveLabel;
    [SerializeField] private Animator topHalfWaveStartLabel;
    [SerializeField] private Animator bottomHalfWaveStartLabel;
    [SerializeField] private Animator gameOverLabel;
    [SerializeField] private Animator gameWonLabel;

    private void Awake()
    {
        GameManager.Instance.OnGoldSet.AddListener(HandleGoldSet);
        EnemySpawner.OnWaveStarted.AddListener(HandleWaveStarted);
        GameManager.Instance.OnHealthSet.AddListener(HandleHealthSet);
        GameManager.Instance.OnGameOver.AddListener(HandleGameOver);
        EnemySpawner.OnGameWon.AddListener(HandleGameWon);
    }

    private void HandleGoldSet()
    {
        goldLabel.text = "GOLD: " + GameManager.Instance.Gold.ToString();
    }

    private void HandleHealthSet()
    {
        healthLabel.text = "HEALTH " + GameManager.Instance.Health.ToString();
    }

    private void HandleWaveStarted()
    {
        waveLabel.text = "WAVE: " + (EnemySpawner.currentWaveIndex + 1).ToString();
        topHalfWaveStartLabel.SetTrigger("nextWave");
        bottomHalfWaveStartLabel.SetTrigger("nextWave");
    }

    private void HandleGameOver()
    {
        gameOverLabel.SetTrigger("gameOver");
        StartCoroutine(GameOverCoroutine());
    }

    private void HandleGameWon()
    {
        gameWonLabel.SetTrigger("gameWon");
        StartCoroutine(GameWonCoroutine());
    }

    private IEnumerator GameOverCoroutine()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(2); // Load the game over scene
    }

    private IEnumerator GameWonCoroutine()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(3); // Load the win scene
    }
}
