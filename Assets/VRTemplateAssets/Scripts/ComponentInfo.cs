using UnityEngine;

public class ComponentInfo : MonoBehaviour
{
    [Header("Assign the Canvas for THIS part")]
    public GameObject mySpecificCanvas;

    public void ToggleMyUI()
    {
        if (mySpecificCanvas != null)
        {
            // If it's on, turn it off. If it's off, turn it on.
            mySpecificCanvas.SetActive(!mySpecificCanvas.activeSelf);
        }
    }

    public void HideMyUI()
    {
        if (mySpecificCanvas != null)
        {
            mySpecificCanvas.SetActive(false);
        }
    }
}