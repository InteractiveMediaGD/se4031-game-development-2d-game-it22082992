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
        // Calculate dynamic vertical bounds based on current camera position and size
        if (Camera.main == null) return;
        float distance = transform.position.z - Camera.main.transform.position.z;
        Vector3 minBounds = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distance));
        Vector3 maxBounds = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, distance));

        // Pick a random height within visible camera bounds
        float randomY = Random.Range(minBounds.y, maxBounds.y);
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