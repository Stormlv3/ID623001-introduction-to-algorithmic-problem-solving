/// 
/// Lucas Storm
/// June 2024
/// Bugs: None known at this time.
/// 
/// This script contains the stats for the monsters and
/// handles the upgrades of them as well.

using System.Collections.Generic;
using UnityEngine;

public class MonsterData : MonoBehaviour
{
    [System.Serializable]
    public class MonsterLevel
    {
        public int cost;
        public GameObject sprite;
        public GameObject bulletPrefab;
        public float bulletSpeed;
        public int bulletDamage;
        public float shootCooldown;
    }

    public List<MonsterLevel> levels;

    private MonsterLevel currentLevel;

    public void OnEnable()
    {
        CurrentLevel = levels[0];
    }

    public MonsterLevel CurrentLevel
    {
        get { return currentLevel; }
        set
        {
            currentLevel = value;
            foreach (var level in levels)
            {
                if (level == currentLevel)
                {
                    level.sprite.SetActive(true);
                }
                else
                {
                    level.sprite.SetActive(false);
                }
            }
        }
    }

    public MonsterLevel GetNextLevel()
    {
        int index = levels.IndexOf(currentLevel);
        if (index < levels.Count - 1)
        {
            return levels[index + 1];
        }
        else
        {
            return null;
        }
    }

    public void IncreaseLevel()
    {
        MonsterLevel nextLevel = GetNextLevel();
        if (nextLevel != null)
        {
            CurrentLevel = nextLevel;
        }
        else
        {
            Debug.Log("Already at max level!");
        }
    }
}
