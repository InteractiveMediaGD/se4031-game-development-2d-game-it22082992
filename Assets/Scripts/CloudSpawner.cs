using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    public GameObject cloudPrefab;
    public float spawnRate = 2f;
    public float minY = -4f, maxY = 4f;
    public float minSpeed = 1f, maxSpeed = 3f;

    void Start()
    {
        // Starts the spawning loop
        InvokeRepeating("SpawnCloud", 0, spawnRate);
    }

    void SpawnCloud()
    {
        // Pick a random height for the cloud
        float randomY = Random.Range(minY, maxY);
        Vector3 spawnPos = new Vector3(transform.position.x, randomY, 0);

        // Create the cloud
        GameObject cloud = Instantiate(cloudPrefab, spawnPos, Quaternion.identity);
        
        // Give the cloud a random speed
        float randomSpeed = Random.Range(minSpeed, maxSpeed);
        
        // Fix: Check if component exists to avoid NullReferenceException
        FloatingCloud cloudScript = cloud.GetComponent<FloatingCloud>();
        if (cloudScript != null)
        {
            cloudScript.speed = randomSpeed;
        }
        else
        {
            Debug.LogWarning("Spawning cloud, but the prefab is missing the 'FloatingCloud' script!");
        }
    }
}