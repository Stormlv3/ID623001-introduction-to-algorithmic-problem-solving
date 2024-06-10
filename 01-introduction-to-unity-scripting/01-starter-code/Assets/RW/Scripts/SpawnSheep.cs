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

    private List<Sheep> sheepList = new List<Sheep>();

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private void SpawnNewSheep()
    {
        if (sheepSpawnPositions.Count == 0) return;

        Vector3 randomPosition = sheepSpawnPositions[Random.Range(0, sheepSpawnPositions.Count)].position;
        Sheep newSheep = Instantiate(sheepPrefab, randomPosition, sheepPrefab.transform.rotation);
        newSheep.OnAteHay.AddListener(HandleSheepAteHay); 
        newSheep.OnDropped.AddListener(HandleSheepDropped);
        sheepList.Add(newSheep);
    }

    private void HandleSheepAteHay(Sheep sheep)
    {
        sheepList.Remove(sheep);
        // Later we could add some points here.
    }

    private void HandleSheepDropped(Sheep sheep)
    {
        sheepList.Remove(sheep);
        // Later, we could subtract lives here.
    }

    private IEnumerator SpawnRoutine()
    {
        while (canSpawn)
        {
            SpawnNewSheep();
            float timeBetweenSpawns = Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns);
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }
}
