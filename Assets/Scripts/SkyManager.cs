using UnityEngine;

public class SkyManager : MonoBehaviour
{
    [Header("References")]
    public PlayerController player;
    
    [System.Serializable]
    public class SkyLayer
    {
        public string name;
        public Renderer renderer;
        public float parallaxFactor = 1f;
        [Tooltip("Higher numbers appear closer to the player.")]
        public int sortingOrder = 0;
        [Tooltip("How far from the camera? (Positive numbers are further away)")]
        public float localZ = 10f;
        
        [Tooltip("Optional: If assigned, the manager will set the material's texture to this.")]
        public Texture2D layerTexture;
        
        [HideInInspector]
        public Material material;
    }

    [Header("Sky Layers")]
    public SkyLayer farSky;
    public SkyLayer middleClouds;
    public SkyLayer nearClouds;

    [Header("Settings")]
    public float baseScrollSpeed = 0.05f;

    void Start()
    {
        // Find player automatically if not assigned
        if (player == null)
            player = FindObjectOfType<PlayerController>();

        // Initialize materials and positions
        InitializeLayer(farSky);
        InitializeLayer(middleClouds);
        InitializeLayer(nearClouds);
    }

    void InitializeLayer(SkyLayer layer)
    {
        if (layer == null || layer.renderer == null) return;
        
        layer.material = layer.renderer.material;

        // Set Sorting Order
        layer.renderer.sortingOrder = layer.sortingOrder;

        // Set Local Z Position relative to camera (if it's a child)
        layer.renderer.transform.localPosition = new Vector3(0, 0, layer.localZ);

        // If a texture is assigned in the Inspector, apply it to the material
        if (layer.layerTexture != null)
        {
            layer.material.mainTexture = layer.layerTexture;
        }

        // Check if texture is set to Repeat
        if (layer.material.mainTexture != null && layer.material.mainTexture.wrapMode != TextureWrapMode.Repeat)
        {
            Debug.LogWarning($"Texture on {layer.name} is NOT set to Repeat. The sky won't scroll smoothly. Please change 'Wrap Mode' to 'Repeat' in its Import Settings.");
        }
    }

    void Update()
    {
        if (player == null) return;

        // Get current player speed to calculate offset
        float speed = player.currentSpeed;

        // Update each layer's offset
        UpdateLayerOffset(farSky, speed);
        UpdateLayerOffset(middleClouds, speed);
        UpdateLayerOffset(nearClouds, speed);
    }

    void UpdateLayerOffset(SkyLayer layer, float speed)
    {
        if (layer == null || layer.material == null) return;

        // Calculate and apply texture offset based on time, player speed, and parallax factor
        float offset = (Time.time * baseScrollSpeed) + (Time.time * speed * 0.01f * layer.parallaxFactor);
        layer.material.mainTextureOffset = new Vector2(offset, 0);
    }
}
