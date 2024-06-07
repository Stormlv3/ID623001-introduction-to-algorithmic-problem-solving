using System;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public UnityEvent OnGoldSet = new UnityEvent();
    public int StartingGold = 1000;
    public int StartingHealth = 5;
    private int gold;

    public GameObject[] healthIndicators;

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
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        Gold = StartingGold;
        Health = StartingHealth;
    }
}