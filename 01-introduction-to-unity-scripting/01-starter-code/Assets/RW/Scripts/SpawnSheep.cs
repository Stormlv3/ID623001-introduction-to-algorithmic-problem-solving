/// 
/// Author: Lucas Storm
/// June 2024
/// Bugs: None known at this time.
/// 
/// This script manages the spawning of the sheep.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSheep : MonoBehaviour
{
    public bool canSpawn = true;

    public Sheep sheepPrefab;
    public List<Transform> sheepSpawnPositions = new List<Transform>();
    public float minTimeBetweenSpawns;
    public float maxTimeBetweenSpawns;

    // List to keep track of spawned sheep
    private List<Sheep> sheepList = new List<Sheep>();

    private void Start()
    {
        // Start the coroutine to spawn sheep at intervals
        StartCoroutine(SpawnRoutine());
    }

    private void SpawnNewSheep()
    {
        if (sheepSpawnPositions.Count == 0) return; // Ensure there are spawn positions available

        // Select a random position from the available spawn positions
        Vector3 randomPosition = sheepSpawnPositions[Random.Range(0, sheepSpawnPositions.Count)].position;

        // Instantiate a new sheep at the selected position
        Sheep newSheep = Instantiate(sheepPrefab, randomPosition, sheepPrefab.transform.rotation);

        // Add event listeners to handle sheep actions
        newSheep.OnAteHay.AddListener(HandleSheepAteHay);
        newSheep.OnDropped.AddListener(HandleSheepDropped);

        // Add the new sheep to the list of spawned sheep
        sheepList.Add(newSheep);
    }

    private void HandleSheepAteHay(Sheep sheep)
    {
        // Remove the sheep from the list when it eats hay
        sheepList.Remove(sheep);
    }

    private void HandleSheepDropped(Sheep sheep)
    {
        // Remove the sheep from the list when it drops off the map
        sheepList.Remove(sheep);
    }

    private IEnumerator SpawnRoutine()
    {
        while (canSpawn)
        {
            // Spawn a new sheep
            SpawnNewSheep();

            // Wait for a random time interval before spawning the next sheep
            float timeBetweenSpawns = Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns);
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }
}
