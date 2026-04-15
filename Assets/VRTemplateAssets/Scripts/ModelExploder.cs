using UnityEngine;
using UnityEngine.InputSystem; // This line fixes the red error!

public class ModelExploder : MonoBehaviour
{
    [Header("Model Parts")]
    public GameObject ceilingWalls;
    public GameObject tubesPipelines;
    public GameObject floor;
    public GameObject infoCanvas; 

    [Header("Settings")]
    public Vector3 ceilingOffset = new Vector3(0, 0.5f, 0); 
    public Vector3 floorOffset = new Vector3(0, -0.5f, 0);
    public float scaleSpeed = 1.0f; 

    private bool isExploded = false;

    void Update()
    {
        // Check if a keyboard is connected
        if (Keyboard.current == null) return;

        // Press K to Grow
        if (Keyboard.current.kKey.isPressed)
        {
            transform.localScale += Vector3.one * scaleSpeed * Time.deltaTime;
        }

        // Press J to Shrink
        if (Keyboard.current.jKey.isPressed)
        {
            // Only shrink if scale is positive to avoid the "Negative Scale" warning
            if (transform.localScale.x > 0.1f)
            {
                transform.localScale -= Vector3.one * scaleSpeed * Time.deltaTime;
            }
        }
    }

    public void ToggleExplode()
    {
        if (!isExploded)
        {
            ceilingWalls.transform.localPosition += ceilingOffset;
            floor.transform.localPosition += floorOffset;
            if(infoCanvas != null) infoCanvas.SetActive(true);
        }
        else
        {
            ceilingWalls.transform.localPosition -= ceilingOffset;
            floor.transform.localPosition -= floorOffset;
            if(infoCanvas != null) infoCanvas.SetActive(false);
        }
        isExploded = !isExploded;
    }
}