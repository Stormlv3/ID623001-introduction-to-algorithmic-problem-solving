using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public Enemy enemyPrefab;
        public float minSpawnInterval = 4; // Minimum spawn interval for this wave
        public float maxSpawnInterval = 2; // Maximum spawn interval for this wave
        public int maxEnemies = 20;
    }

    public Transform[] waypoints;
    public Wave[] waves; // The definition of all our waves (will be set in-editor)
    public int currentWaveIndex = 0;
    public int timeBetweenWaves;

    // Events
    public UnityEvent OnWaveStarted = new UnityEvent();

    IEnumerator Start()
    {
        // Current wave index will be incremented once we've spawned all the enemies for this wave.
        while (currentWaveIndex < waves.Length)
        {
            OnWaveStarted?.Invoke(); // Fire an event that we'll hook into later.
            Wave currentWave = waves[currentWaveIndex];
            float currentSpawnInterval = currentWave.maxSpawnInterval;

            for (var i = 0; i < currentWave.maxEnemies; i++)
            {
                // Spawn an enemy, then wait for the spawn interval before continuing.
                SpawnEnemy(currentWave.enemyPrefab);
                yield return new WaitForSeconds(currentSpawnInterval);

                // Gradually increase the spawn interval as the wave progresses
                currentSpawnInterval = Mathf.Lerp(currentWave.maxSpawnInterval, currentWave.minSpawnInterval, (float)i / currentWave.maxEnemies);
            }

            // Increment the current wave index. You could also write a for loop rather than a while loop if you prefer.
            currentWaveIndex++;
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    public void SpawnEnemy(Enemy enemyPrefab)
    {
        // Spawn the specified enemy prefab
        Enemy newEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        newEnemy.waypoints = waypoints;
    }
}
