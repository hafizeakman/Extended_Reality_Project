using UnityEngine;
using System.Collections;

public class Bteleporter : MonoBehaviour
{
    public Transform objectToTeleport;   // what moves
    public Transform targetLocation;     // where it goes
    public float delay = 2f;             // time before teleport

    private bool isRunning = false;

    // 🔥 Call this from button / event
    public void StartTeleport()
    {
        if (!isRunning)
        {
            StartCoroutine(TeleportAfterDelay());
        }
    }

    private IEnumerator TeleportAfterDelay()
    {
        isRunning = true;

        yield return new WaitForSeconds(delay);

        if (objectToTeleport != null && targetLocation != null)
        {
            objectToTeleport.position = targetLocation.position;
            objectToTeleport.rotation = targetLocation.rotation;
        }

        isRunning = false;
    }
}