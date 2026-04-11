using UnityEngine;

public class RiseObject : MonoBehaviour
{
    [Header("Movement Settings")]
    public float riseAmount = 2f;     // how much it goes up
    public float speed = 2f;          // how fast it moves

    private Vector3 targetPosition;
    private bool isMoving = false;

    public void Rise()
    {
        // Set target position relative to current position
        targetPosition = transform.position + Vector3.up * riseAmount;
        isMoving = true;
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                speed * Time.deltaTime
            );

            // Stop when close enough
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                isMoving = false;
            }
        }
    }
}