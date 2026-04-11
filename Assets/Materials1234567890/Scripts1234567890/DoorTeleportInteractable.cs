using UnityEngine;

public class DoorTeleportInteractable : MonoBehaviour
{
    [Header("Teleport Target")]
    public Transform destination;

    [Header("Player Rig Root")]
    public Transform playerRigRoot;

    public void TeleportPlayer()
    {
        if (destination == null)
        {
            Debug.LogWarning("Destination is not assigned on " + gameObject.name);
            return;
        }

        if (playerRigRoot == null)
        {
            Debug.LogWarning("Player Rig Root is not assigned on " + gameObject.name);
            return;
        }

        playerRigRoot.position = destination.position;
        playerRigRoot.rotation = destination.rotation;
    }
}