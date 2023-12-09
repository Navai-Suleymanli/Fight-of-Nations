using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] GameObject placeToSpawn;
    [SerializeField] float minX, maxX, minZ, maxZ;
    [SerializeField] float spawnInterval = 2f; // Time interval between spawns
    [SerializeField] int maxEnemyCount = 30;
    [SerializeField] int currentCount=0;

    private float nextSpawnTime;

    // Start is called before the first frame update
    void Start()
    {
        nextSpawnTime = Time.time + spawnInterval;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextSpawnTime && OccupationManager.valueToDecrease > 0  && currentCount < maxEnemyCount)
        {
            Spawn();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    private void Spawn()
    {
        Vector3 randomPosition = new Vector3(
            placeToSpawn.transform.position.x + Random.Range(minX, maxX),
            0f, placeToSpawn.transform.position.z + Random.Range(minZ, maxZ)
        );

        Instantiate(objectToSpawn, randomPosition, Quaternion.identity);
        currentCount++;
    }
}
