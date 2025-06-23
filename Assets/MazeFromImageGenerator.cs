using UnityEngine;

/// <summary>
/// Generates a maze in the scene based on a black-and-white texture,
/// grouping pixels into larger blocks to reduce prefab count.
/// Black pixels represent walls; white pixels represent paths.
/// The maze is centered on `parentTransform` if set, otherwise at world origin.
/// </summary>
public class MazeFromImageGenerator : MonoBehaviour
{
    [Header("Maze Image")]
    [Tooltip("Texture2D where black pixels represent walls and white pixels represent paths.")]
    public Texture2D mazeImage;

    [Header("Prefabs")]
    [Tooltip("Prefab for a single wall tile.")]
    public GameObject wallPrefab;
    [Tooltip("Optional parent transform to keep the hierarchy clean and center the maze.")]
    public Transform parentTransform;

    [Header("Tile Settings")]
    [Tooltip("Size of each unit tile in world units.")]
    public float tileSize = 1f;
    [Tooltip("Number of pixels to group into one wall block.")]
    public int groupSize = 4;
    public int mazeRotation = 90;

    [Header("Threshold")]
    [Tooltip("Threshold to decide if a pixel is considered wall (0 = black, 1 = white)")]
    [Range(0f, 1f)]
    public float wallThreshold = 0.5f;

    void Start()
    {
        if (mazeImage == null || wallPrefab == null)
        {
            Debug.LogError("Maze image or wall prefab not set!");
            return;
        }
        if (!mazeImage.isReadable)
        {
            Debug.LogError("Maze image must have Read/Write enabled in import settings.");
            return;
        }
        GenerateMazeGrouped();
    }

    /// <summary>
    /// Loops through blocks of pixels and instantiates grouped wall prefabs.
    /// Maze is centered at parentTransform.position or world origin.
    /// </summary>
    private void GenerateMazeGrouped()
    {
        int width = mazeImage.width;
        int height = mazeImage.height;

        // Compute base offset to center maze extents
        Vector3 baseOffset = new Vector3(-width * tileSize * 0.5f, 0f, -height * tileSize * 0.5f);
        Vector3 worldOffset = (parentTransform != null ? parentTransform.position : Vector3.zero) + baseOffset;

        // Step by groupSize to combine pixels
        for (int y = 0; y < height; y += groupSize)
        {
            for (int x = 0; x < width; x += groupSize)
            {
                // Compute average brightness in this block
                float sum = 0f;
                int count = 0;
                for (int yy = y; yy < y + groupSize && yy < height; yy++)
                {
                    for (int xx = x; xx < x + groupSize && xx < width; xx++)
                    {
                        sum += mazeImage.GetPixel(xx, yy).grayscale;
                        count++;
                    }
                }

                float average = sum / count;
                // If average darkness below threshold, spawn a grouped wall
                if (average < wallThreshold)
                {
                    // Calculate center of block
                    float centerX = x + groupSize * 0.5f;
                    float centerY = y + groupSize * 0.5f;
                    Vector3 position = new Vector3(centerX * tileSize, 0f, centerY * tileSize) + worldOffset;

                    // Instantiate with optional parent for cleaner hierarchy
                    Transform parent = parentTransform != null ? parentTransform : null;
                    GameObject wall = Instantiate(wallPrefab, position, Quaternion.identity, parent);

                    // Scale the prefab to cover the group area
                    Vector3 localScale = wall.transform.localScale;
                    localScale.x = tileSize * groupSize;
                    localScale.z = tileSize * groupSize;
                    wall.transform.localScale = localScale;
                }
            }
        }

        Debug.Log($"Maze generated: {width}x{height}, groupSize: {groupSize}, threshold: {wallThreshold}");
        parentTransform.Rotate(0f, mazeRotation, 0f);
    }
}
