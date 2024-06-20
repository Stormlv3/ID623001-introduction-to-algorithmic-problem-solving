using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public Enemy[] enemyPrefabs;
        public float minSpawnInterval = 4;
        public float maxSpawnInterval = 2;
        public int maxEnemies = 20;
    }

    public Transform[] waypoints;
    public Wave[] waves;
    public int currentWaveIndex = 0;
    public int timeBetweenWaves;

    private List<Enemy> spawnedEnemies = new List<Enemy>();

    // Events
    public UnityEvent OnWaveStarted = new UnityEvent();
    public UnityEvent OnGameWon = new UnityEvent();

    private void OnEnable()
    {
        GameManager.Instance.OnGameOver.AddListener(DestroyAllEnemies);
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGameOver.RemoveListener(DestroyAllEnemies);
    }

    IEnumerator Start()
    {
        while (currentWaveIndex < waves.Length)
        {
            if (GameManager.Instance.gameOver) yield break;

            OnWaveStarted?.Invoke();
            Wave currentWave = waves[currentWaveIndex];
            float currentSpawnInterval = currentWave.maxSpawnInterval;

            for (var i = 0; i < currentWave.maxEnemies; i++)
            {
                if (GameManager.Instance.gameOver) yield break;

                SpawnRandomEnemy(currentWave.enemyPrefabs);
                yield return new WaitForSeconds(currentSpawnInterval);

                currentSpawnInterval = Mathf.Lerp(currentWave.maxSpawnInterval, currentWave.minSpawnInterval, (float)i / currentWave.maxEnemies);
            }

            currentWaveIndex++;
            yield return new WaitForSeconds(timeBetweenWaves);
        }

        OnGameWon?.Invoke();
    }

    public void SpawnRandomEnemy(Enemy[] enemyPrefabs)
    {
        if (GameManager.Instance.gameOver) return;

        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        Enemy selectedPrefab = enemyPrefabs[randomIndex];

        Enemy newEnemy = Instantiate(selectedPrefab, transform.position, Quaternion.identity);
        newEnemy.waypoints = waypoints;
        spawnedEnemies.Add(newEnemy);
    }

    public void DestroyAllEnemies()
    {
        foreach (Enemy enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy.gameObject);
            }
        }
        spawnedEnemies.Clear();
    }
}
