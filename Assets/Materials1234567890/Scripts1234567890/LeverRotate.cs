using UnityEngine;
using System.Collections;

public class LeverRotate : MonoBehaviour
{
    [Header("Target")]
    public Transform objectToRotate;

    [Header("Rotation Settings")]
    public Vector3 rotationAmount = new Vector3(0f, 90f, 0f);
    public float rotationDuration = 1f;

    private bool hasRotated = false;

    public void ActivateLever()
    {
        if (hasRotated) return;

        hasRotated = true;
        StartCoroutine(RotateObject());
    }

    private IEnumerator RotateObject()
    {
        Quaternion startRotation = objectToRotate.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(rotationAmount);

        float time = 0f;

        while (time < rotationDuration)
        {
            time += Time.deltaTime;
            float t = time / rotationDuration;

            objectToRotate.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return null;
        }

        objectToRotate.rotation = targetRotation;
    }
}