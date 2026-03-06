using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    [Range(-1f, 1f)]
    public float scrollSpeed = 0.1f;
    private MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        // This shifts the texture itself rather than the object's position
        float offset = Time.time * scrollSpeed;
        meshRenderer.material.mainTextureOffset = new Vector2(offset, 0);
    }
}