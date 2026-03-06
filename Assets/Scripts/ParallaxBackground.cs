using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Transform cameraTransform;
    public float parallaxEffect; // Set between 0 and 1 (0.1 is slow/far away, 0.9 is fast/close)

    private float lastCameraX;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        lastCameraX = cameraTransform.position.x;
    }

        void Update()
    {
        // Slowly drifts the background to the left constantly
        transform.Translate(Vector3.left * 0.5f * Time.deltaTime);
    }

    void LateUpdate()
    {
        // Calculate how much the camera has moved
        float deltaX = cameraTransform.position.x - lastCameraX;
        
        // Move the background by a fraction of that amount
        transform.position += Vector3.right * (deltaX * parallaxEffect);
        
        lastCameraX = cameraTransform.position.x;
    }
}