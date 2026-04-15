using UnityEngine;
using System.Collections;

public class MoveObjectInDirection : MonoBehaviour
{
    [Header("Object")]
    public Transform objectToMove;

    [Header("Movement")]
    public Vector3 moveDirection = Vector3.down; // default = down
    public float moveSpeed = 1f;
    public float moveDuration = 2f;

    [Header("Safety")]
    public bool triggerOnlyOnce = false;

    private bool hasTriggered = false;
    private bool isMoving = false;

    public void StartMovement()
    {
        if (isMoving)
        {
            Debug.Log("Already moving");
            return;
        }

        if (triggerOnlyOnce && hasTriggered)
            return;

        hasTriggered = true;

        if (objectToMove == null)
        {
            Debug.LogWarning("No object assigned to move.");
            return;
        }

        StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        isMoving = true;

        float timer = 0f;
        Vector3 direction = moveDirection.normalized;

        while (timer < moveDuration)
        {
            objectToMove.position += direction * moveSpeed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        isMoving = false;
    }
}