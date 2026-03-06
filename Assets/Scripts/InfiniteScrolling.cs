using UnityEngine;

public class InfiniteScrolling : MonoBehaviour
{
    public float scrollSpeed = 0.1f;
    private Material mat;

    void Start()
    {
        // Grabs the material from the Quad
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        // This shifts the texture's X position over time
        Vector2 offset = mat.mainTextureOffset;
        offset.x += scrollSpeed * Time.deltaTime;
        mat.mainTextureOffset = offset;
    }
}