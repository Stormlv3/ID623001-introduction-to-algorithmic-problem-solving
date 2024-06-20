using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic; // Include this namespace for List<T>

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public UnityEvent OnGoldSet = new UnityEvent();
    public int StartingGold = 1000;
    public int StartingHealth = 5;
    private int gold;

    public List<GameObject> healthIndicators;

    public bool gameOver = false;
    public UnityEvent OnHealthSet = new UnityEvent();
    public UnityEvent OnGameOver = new UnityEvent();
    private int health;

    public int Health
    {
        get { return health; }
        set
        {
            health = value;
            OnHealthSet?.Invoke();

            if (health < StartingHealth)
            {
                SFXManager.Instance.PlayLifeLostSFX();
                if (healthIndicators.Count > 0)
                {
                    int randomIndex = Random.Range(0, healthIndicators.Count);
                    healthIndicators[randomIndex].SetActive(false);
                    healthIndicators.RemoveAt(randomIndex);
                }
            }

            if (health <= 0 && !gameOver)
            {
                OnGameOver?.Invoke();
                gameOver = true;
            }
        }
    }

    public int Gold
    {
        get { return gold; }
        set
        {
            gold = value;
            OnGoldSet?.Invoke();
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        Gold = StartingGold;
        Health = StartingHealth;
    }

    public void AddGold(int amount)
    {
        Gold += amount;
    }
}
