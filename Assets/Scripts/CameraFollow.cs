using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform playerTransform;
    public float xOffset = 5f; // Keeps the player on the left side of the screen

    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.1f;
    private Vector3 originalPos;

    public void TriggerShake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }

    void LateUpdate()
    {
        if (playerTransform != null)
        {
            // Calculate base position
            Vector3 targetPos = new Vector3(playerTransform.position.x + xOffset, transform.position.y, transform.position.z);

            // Apply shake if active
            if (shakeDuration > 0)
            {
                targetPos += (Vector3)Random.insideUnitCircle * shakeMagnitude;
                shakeDuration -= Time.deltaTime;
            }

            transform.position = targetPos;
        }
    }
}