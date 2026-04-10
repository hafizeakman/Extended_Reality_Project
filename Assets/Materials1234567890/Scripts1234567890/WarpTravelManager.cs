using UnityEngine;
using System.Collections;

public class WarpTravelManager : MonoBehaviour
{
    public enum DestinationId
    {
        None,
        BlackHole,
        Earth
    }

    [System.Serializable]
    public class DestinationObject
    {
        public Transform sceneObject;
        public Transform resetPoint;
        public Transform activePoint;
    }

    [System.Serializable]
    public class DestinationDefinition
    {
        public DestinationId id;
        public string displayName;
        public DestinationObject[] objects;
    }

    public WarpSequenceController warpSequenceController;
    public MonitorScreen monitorScreen;

    public DestinationDefinition[] destinations;

    public float destinationAppearDelay = 1.25f;
    public float warpLockDuration = 3f;
    public float infoMessageDuration = 1.5f;

    public string warpingMessage = "Warping...";
    public string noSelectionMessage = "No destination selected";
    public string alreadyThereMessage = "You are already there";

    public Behaviour[] warpButtonInteractables;
    public Behaviour[] monitorToggleButtonInteractables;

    private DestinationId currentDestination = DestinationId.None;
    private DestinationId selectedDestination = DestinationId.None;

    private bool hasCompletedIntroWarp = false;
    private bool isWarping = false;
    private bool monitorUnlocked = false;
    private bool monitorScreenOn = false;

    private Coroutine messageRoutine;

    void Start()
    {
        ResetAllDestinations();

        currentDestination = DestinationId.None;
        selectedDestination = DestinationId.None;
        hasCompletedIntroWarp = false;
        isWarping = false;
        monitorUnlocked = false;
        monitorScreenOn = false;

        if (monitorScreen != null)
        {
            monitorScreen.TurnScreenOff();
            monitorScreen.ClearStatus();
        }

        SetWarpButtonInteractable(true);
        SetMonitorToggleButtonInteractable(false);
    }

    public void PressWarpButton()
    {
        if (isWarping)
            return;

        if (!hasCompletedIntroWarp)
        {
            StartCoroutine(IntroWarpRoutine());
            return;
        }

        if (selectedDestination == DestinationId.None)
        {
            ShowTemporaryMessage(noSelectionMessage);
            return;
        }

        if (selectedDestination == currentDestination)
        {
            ShowTemporaryMessage(alreadyThereMessage);
            return;
        }

        StartCoroutine(WarpToSelectedRoutine());
    }

    public void CycleMonitorSelection()
    {
        if (isWarping)
            return;

        if (!monitorUnlocked)
            return;

        if (monitorScreen == null)
            return;

        if (!monitorScreenOn)
        {
            monitorScreenOn = true;

            if (selectedDestination == DestinationId.None)
                selectedDestination = currentDestination;

            UpdateMonitorPreview(selectedDestination);
            return;
        }

        if (selectedDestination == DestinationId.BlackHole)
            selectedDestination = DestinationId.Earth;
        else
            selectedDestination = DestinationId.BlackHole;

        UpdateMonitorPreview(selectedDestination);
    }

    private IEnumerator IntroWarpRoutine()
    {
        isWarping = true;
        SetWarpButtonInteractable(false);
        SetMonitorToggleButtonInteractable(false);

        if (monitorScreen != null)
            monitorScreen.ShowStatus(warpingMessage);

        if (warpSequenceController != null)
            warpSequenceController.StartWarpSequence();

        ResetAllDestinations();

        yield return new WaitForSeconds(destinationAppearDelay);

        ActivateDestination(DestinationId.BlackHole);

        currentDestination = DestinationId.BlackHole;
        selectedDestination = DestinationId.BlackHole;

        float totalWait = Mathf.Max(warpLockDuration, destinationAppearDelay);
        float remainingWait = totalWait - destinationAppearDelay;

        if (remainingWait > 0f)
            yield return new WaitForSeconds(remainingWait);

        hasCompletedIntroWarp = true;
        monitorUnlocked = true;
        isWarping = false;

        if (monitorScreen != null)
            monitorScreen.ClearStatus();

        SetWarpButtonInteractable(true);
        SetMonitorToggleButtonInteractable(true);
    }

    private IEnumerator WarpToSelectedRoutine()
    {
        isWarping = true;
        SetWarpButtonInteractable(false);
        SetMonitorToggleButtonInteractable(false);

        if (monitorScreen != null)
            monitorScreen.ShowStatus(warpingMessage);

        if (warpSequenceController != null)
            warpSequenceController.StartWarpSequence();

        ResetAllDestinations();

        yield return new WaitForSeconds(destinationAppearDelay);

        ActivateDestination(selectedDestination);
        currentDestination = selectedDestination;

        UpdateMonitorPreview(currentDestination);

        float totalWait = Mathf.Max(warpLockDuration, destinationAppearDelay);
        float remainingWait = totalWait - destinationAppearDelay;

        if (remainingWait > 0f)
            yield return new WaitForSeconds(remainingWait);

        isWarping = false;

        if (monitorScreen != null)
            monitorScreen.ClearStatus();

        SetWarpButtonInteractable(true);
        SetMonitorToggleButtonInteractable(true);
    }

    private void ResetAllDestinations()
    {
        for (int i = 0; i < destinations.Length; i++)
        {
            DestinationDefinition destination = destinations[i];

            if (destination.objects == null)
                continue;

            for (int j = 0; j < destination.objects.Length; j++)
            {
                DestinationObject obj = destination.objects[j];

                if (obj.sceneObject == null || obj.resetPoint == null)
                    continue;

                obj.sceneObject.position = obj.resetPoint.position;
                obj.sceneObject.rotation = obj.resetPoint.rotation;
            }
        }
    }

    private void ActivateDestination(DestinationId destinationId)
    {
        DestinationDefinition destination = GetDestination(destinationId);

        if (destination == null || destination.objects == null)
            return;

        for (int i = 0; i < destination.objects.Length; i++)
        {
            DestinationObject obj = destination.objects[i];

            if (obj.sceneObject == null || obj.activePoint == null)
                continue;

            obj.sceneObject.position = obj.activePoint.position;
            obj.sceneObject.rotation = obj.activePoint.rotation;
        }
    }

    private DestinationDefinition GetDestination(DestinationId destinationId)
    {
        for (int i = 0; i < destinations.Length; i++)
        {
            if (destinations[i].id == destinationId)
                return destinations[i];
        }

        return null;
    }

    private void UpdateMonitorPreview(DestinationId destinationId)
    {
        if (monitorScreen == null)
            return;

        if (destinationId == DestinationId.Earth)
            monitorScreen.ShowEarth();
        else if (destinationId == DestinationId.BlackHole)
            monitorScreen.ShowBlackHole();
    }

    private void ShowTemporaryMessage(string message)
    {
        if (monitorScreen != null)
            monitorScreen.ShowStatus(message);

        if (messageRoutine != null)
            StopCoroutine(messageRoutine);

        messageRoutine = StartCoroutine(ClearMessageAfterDelay());
    }

    private IEnumerator ClearMessageAfterDelay()
    {
        yield return new WaitForSeconds(infoMessageDuration);

        if (!isWarping && monitorScreen != null)
            monitorScreen.ClearStatus();

        messageRoutine = null;
    }

    private void SetWarpButtonInteractable(bool value)
    {
        for (int i = 0; i < warpButtonInteractables.Length; i++)
        {
            if (warpButtonInteractables[i] != null)
                warpButtonInteractables[i].enabled = value;
        }
    }

    private void SetMonitorToggleButtonInteractable(bool value)
    {
        for (int i = 0; i < monitorToggleButtonInteractables.Length; i++)
        {
            if (monitorToggleButtonInteractables[i] != null)
                monitorToggleButtonInteractables[i].enabled = value;
        }
    }
}