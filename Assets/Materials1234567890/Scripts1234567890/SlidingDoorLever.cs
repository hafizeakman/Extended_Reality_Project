using UnityEngine;
using System.Collections;

public class SlidingDoorLever : MonoBehaviour
{
    [Header("Door Parts")]
    public Transform leftDoor;
    public Transform rightDoor;

    [Header("Slide Settings")]
    public float slideDistance = 1.5f;
    public float slideDuration = 1f;

    private Vector3 leftClosedPos;
    private Vector3 rightClosedPos;
    private Vector3 leftOpenPos;
    private Vector3 rightOpenPos;

    private bool hasOpened = false;

    private void Start()
    {
        leftClosedPos = leftDoor.position;
        rightClosedPos = rightDoor.position;

        leftOpenPos = leftClosedPos + Vector3.left * slideDistance;
        rightOpenPos = rightClosedPos + Vector3.right * slideDistance;
    }

    public void OpenDoor()
    {
        if (hasOpened) return;

        hasOpened = true;
        StartCoroutine(OpenDoorCoroutine());
    }

    private IEnumerator OpenDoorCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slideDuration;

            leftDoor.position = Vector3.Lerp(leftClosedPos, leftOpenPos, t);
            rightDoor.position = Vector3.Lerp(rightClosedPos, rightOpenPos, t);

            yield return null;
        }

        leftDoor.position = leftOpenPos;
        rightDoor.position = rightOpenPos;
    }
}