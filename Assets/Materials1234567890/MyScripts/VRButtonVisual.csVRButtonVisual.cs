using UnityEngine;

public class VRButtonVisual : MonoBehaviour
{
    [Header("Movement")]
    public float pressDistance = 0.02f; // how far it moves down
    public float pressSpeed = 10f;

    private Vector3 initialPosition;
    private Vector3 targetPosition;

    private bool isPressed = false;

    void Start()
    {
        initialPosition = transform.localPosition;
        targetPosition = initialPosition;
    }

    void Update()
    {
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPosition,
            Time.deltaTime * pressSpeed
        );
    }

    public void PressButton()
    {
        isPressed = true;
        targetPosition = initialPosition + Vector3.down * pressDistance;
    }

    public void ReleaseButton()
    {
        isPressed = false;
        targetPosition = initialPosition;
    }
}