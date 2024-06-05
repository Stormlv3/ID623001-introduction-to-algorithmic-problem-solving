using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    public EnemySpawner EnemySpawner;
    [SerializeField] private Text goldLabel;
    [SerializeField] private Text waveLabel;
    [SerializeField] private Animator topHalfWaveStartLabel;
    [SerializeField] private Animator bottomHalfWaveStartLabel;

    private void Awake()
    {
        GameManager.Instance.OnGoldSet.AddListener(HandleGoldSet);
        EnemySpawner.OnWaveStarted.AddListener(HandleWaveStarted);
    }

    private void HandleGoldSet()
    {
        goldLabel.text = "GOLD: " + GameManager.Instance.Gold.ToString();
    }

    private void HandleWaveStarted()
    {
        waveLabel.text = "WAVE: " + (EnemySpawner.currentWaveIndex + 1).ToString();
        // Fire off the animation for both label halves. When played at the same time, these
        // create a flashy effect.
        topHalfWaveStartLabel.SetTrigger("nextWave");
        bottomHalfWaveStartLabel.SetTrigger("nextWave");
    }
}