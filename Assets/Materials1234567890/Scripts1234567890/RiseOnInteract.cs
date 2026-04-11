using UnityEngine;
using System.Collections;

public class RiseOnInteract : MonoBehaviour
{
    [Header("Target")]
    public Transform targetObject;

    [Header("Movement Settings")]
    public float riseAmount = 2f;
    public float riseTime = 1f;

    private bool hasActivated = false;

    // Call this when button is interacted with
    public void Activate()
    {
        if (!hasActivated)
        {
            StartCoroutine(RaiseObject());
            hasActivated = true;
        }
    }

    private IEnumerator RaiseObject()
    {
        Vector3 startPos = targetObject.position;
        Vector3 targetPos = startPos + Vector3.up * riseAmount;

        float elapsed = 0f;

        while (elapsed < riseTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / riseTime;

            targetObject.position = Vector3.Lerp(startPos, targetPos, t);

            yield return null;
        }

        targetObject.position = targetPos;
    }
}