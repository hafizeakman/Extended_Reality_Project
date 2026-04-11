using System.Collections;
using UnityEngine;

public class ElevatorDoor : MonoBehaviour
{
    [Header("Door Panels")]
    public Transform doorLeft;
    public Transform doorRight;

    [Header("Settings")]
    public float slideDistance = 0.65f;
    public float slideSpeed = 1.5f;

    private Vector3 doorLeftClosed;
    private Vector3 doorRightClosed;
    private Vector3 doorLeftOpen;
    private Vector3 doorRightOpen;
    private bool isOpen = false;
    private bool isMoving = false;

    void Start()
    {
        doorLeftClosed = doorLeft.localPosition;
        doorRightClosed = doorRight.localPosition;

        doorLeftOpen = doorLeftClosed + Vector3.left * slideDistance;
        doorRightOpen = doorRightClosed + Vector3.right * slideDistance;
    
    }

    public void OpenDoor()
    {
        if (!isMoving && !isOpen)
            StartCoroutine(SlideDoors(doorLeftOpen, doorRightOpen, true));
    }

    public void CloseDoor()
    {
        if (!isMoving && isOpen)
            StartCoroutine(SlideDoors(doorLeftClosed, doorRightClosed, false));
    }

    IEnumerator SlideDoors(Vector3 targetLeft, Vector3 targetRight, bool opening)
    {
        isMoving = true;

        while (Vector3.Distance(doorLeft.localPosition, targetLeft) > 0.01f)
        {
            doorLeft.localPosition = Vector3.MoveTowards(
                doorLeft.localPosition, targetLeft, slideSpeed * Time.deltaTime);
            doorRight.localPosition = Vector3.MoveTowards(
                doorRight.localPosition, targetRight, slideSpeed * Time.deltaTime);
            yield return null;
        }

        doorLeft.localPosition = targetLeft;
        doorRight.localPosition = targetRight;
        isOpen = opening;
        isMoving = false;
    }
}