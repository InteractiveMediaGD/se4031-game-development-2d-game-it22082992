using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject obstaclePrefab;
    public GameObject enemyPrefab;
    public GameObject healthPackPrefab;
    public Transform playerTransform;
    
    [Header("Difficulty Scaling")]
    public float spawnDistance = 20f;
    public float initialSpawnRate = 3f;
    public float minSpawnRate = 0.8f;
    public float spawnRateScaling = 0.05f; // Decreases spawn interval over time
    
    private float currentSpawnRate;
    private float nextSpawnTime;
    private float startTime;

    void Start()
    {
        currentSpawnRate = initialSpawnRate;
        startTime = Time.time;
    }

    void Update()
    {
        // Follow the player so we are always spawning in front of them
        transform.position = new Vector3(playerTransform.position.x + spawnDistance, 0, 0);

        // Gradually increase spawn frequency
        currentSpawnRate = Mathf.Max(minSpawnRate, initialSpawnRate - (Time.time - startTime) * spawnRateScaling);

        if (Time.time > nextSpawnTime)
        {
            SpawnObject();
            nextSpawnTime = Time.time + currentSpawnRate;
        }
    }

    void SpawnObject()
    {
        // Pick a random object based on dynamic weights
        GameObject prefabToSpawn = GetWeightedRandomPrefab();
        
        // Pick a random height
        float randomY = Random.Range(-4f, 4f);
        Vector3 spawnPos = new Vector3(transform.position.x, randomY, 0);

        Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
    }

    GameObject GetWeightedRandomPrefab()
    {
        float timePassed = Time.time - startTime;

        // Weights: Higher means more likely to spawn
        // Obstacles and Enemies increase over time
        float obstacleWeight = 40 + (timePassed * 0.5f);
        float enemyWeight = 30 + (timePassed * 0.8f);
        
        // Health Packs decrease over time
        float healthWeight = Mathf.Max(5, 30 - (timePassed * 0.4f));

        float totalWeight = obstacleWeight + enemyWeight + healthWeight;
        float randomValue = Random.value * totalWeight;

        if (randomValue < obstacleWeight)
            return obstaclePrefab;
        else if (randomValue < obstacleWeight + enemyWeight)
            return enemyPrefab;
        else
            return healthPackPrefab;
    }
}