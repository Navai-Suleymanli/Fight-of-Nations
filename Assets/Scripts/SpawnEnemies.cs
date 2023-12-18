using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    public GameObject objectToSpawn;
    [SerializeField] private GameObject placeToSpawn;
    [SerializeField] private float minX, maxX, minZ, maxZ;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private int maxEnemyCount = 30;
    private int currentCount = 0;

    private float nextSpawnTime;

    void Start()
    {
        nextSpawnTime = Time.time + spawnInterval;
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime && currentCount < maxEnemyCount)
        {
            Spawn();
            nextSpawnTime = Time.time + spawnInterval;
        }
        Debug.Log($"Current enemy count: {currentCount}, Time until next spawn: {nextSpawnTime - Time.time}");
    }

    private void Spawn()
    {
        Vector3 randomPosition = new Vector3(
            Random.Range(minX, maxX),
            0f,
            Random.Range(minZ, maxZ)
        ) + placeToSpawn.transform.position;

        Instantiate(objectToSpawn, randomPosition, Quaternion.identity);
        currentCount++;

        Debug.Log($"Spawned enemy at {randomPosition}. Total enemies: {currentCount}");
    }
}
