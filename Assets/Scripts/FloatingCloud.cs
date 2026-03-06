using UnityEngine;

public class FloatingCloud : MonoBehaviour
{
    public float speed;

    void Update()
    {
        // Moves the cloud left based on its unique speed
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        // Destroys the cloud once it is far off-screen to save memory
        if (transform.position.x < -15f) 
        {
            Destroy(gameObject);
        }
    }
}