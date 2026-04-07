using UnityEngine;

public class BRaiseRotate : MonoBehaviour
{
    [Header("Movement")]
    public Vector3 moveDirection = Vector3.up;   // choose axis here
    public float moveSpeed = 0.5f;
    public float moveDistance = 2f;

    [Header("Rotation")]
    public Vector3 rotationAxis = Vector3.right; // choose axis here
    public float rotateSpeed = 30f;
    public float maxRotation = 90f;

    private Vector3 startPos;
    private float movedDistance = 0f;
    private float rotatedAmount = 0f;

    private bool isMoving = false;

    void Start()
    {
        startPos = transform.position;
        moveDirection = moveDirection.normalized;
        rotationAxis = rotationAxis.normalized;
    }

    void Update()
    {
        if (!isMoving) return;

        // 🔼 Movement
        if (movedDistance < moveDistance)
        {
            float step = moveSpeed * Time.deltaTime;
            transform.position += moveDirection * step;
            movedDistance += step;
        }

        // 🔄 Rotation
        if (rotatedAmount < maxRotation)
        {
            float rotStep = rotateSpeed * Time.deltaTime;
            transform.Rotate(rotationAxis * rotStep);
            rotatedAmount += rotStep;
        }
    }

    // 🔥 Call from button / XR
    public void StartAction()
    {
        isMoving = true;
    }
}