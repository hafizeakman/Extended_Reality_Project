using UnityEngine;


public class Lever : MonoBehaviour
{
    public float minAngle = 60f;
    public float maxAngle = 122f;

    public bool isLocked = false;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;

    void Start()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        SetRotation(minAngle);
    }

    void Update()
    {
        if (isLocked) return;

        float z = transform.localEulerAngles.z;

        // Fix Unity angle wrap
        if (z > 180f) z -= 360f;

        float clampedZ = Mathf.Clamp(z, minAngle, maxAngle);
        SetRotation(clampedZ);

        // Lock when it reaches 122
        if (Mathf.Abs(clampedZ - maxAngle) < 0.5f)
        {
            LockLever();
        }
    }

    void SetRotation(float angle)
    {
        Vector3 rot = transform.localEulerAngles;
        rot.z = angle;
        transform.localEulerAngles = rot;
    }

    void LockLever()
    {
        isLocked = true;

        if (grab != null)
            grab.enabled = false;

        Debug.Log("Lever locked at 122°");
    }
}