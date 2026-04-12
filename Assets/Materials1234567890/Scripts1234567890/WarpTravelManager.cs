using UnityEngine;
using UnityEngine.Events;
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

    [Header("Core References")]
    public WarpSequenceController warpSequenceController;
    public MonitorScreen monitorScreen;

    [Header("Destinations")]
    public DestinationDefinition[] destinations;

    [Header("Timing")]
    public float destinationAppearDelay = 1.25f;
    public float warpLockDuration = 3f;
    public float infoMessageDuration = 1.5f;

    [Header("Messages")]
    public string warpingMessage = "Warping...";
    public string noSelectionMessage = "No destination selected";
    public string alreadyThereMessage = "You are already there";

    [Header("Interactables")]
    public Behaviour[] warpButtonInteractables;
    public Behaviour[] monitorToggleButtonInteractables;

    [Header("First Time Earth Movement")]
    public Transform earthTriggeredObject;
    public Vector3 earthMoveDirection = Vector3.up;
    public float earthMoveAmount = 2f;
    public float earthMoveSpeed = 2f;

    [Header("Travel Audio")]
    public AudioSource travelAudioSource;
    public AudioClip introWarpClip;
    public AudioClip blackHoleWarpClip;
    public AudioClip earthWarpClip;

    [Header("Extra Trigger Event")]
    public UnityEvent onWarpStarted;

    private DestinationId currentDestination = DestinationId.None;
    private DestinationId selectedDestination = DestinationId.None;

    private bool hasCompletedIntroWarp = false;
    private bool isWarping = false;
    private bool monitorUnlocked = false;
    private bool monitorScreenOn = false;
    private bool hasTriggeredEarthMove = false;

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
        hasTriggeredEarthMove = false;

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

        PlayTravelAudio(DestinationId.BlackHole, true);

        if (onWarpStarted != null)
            onWarpStarted.Invoke();

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

        isWarping = false;
        hasCompletedIntroWarp = true;
        monitorUnlocked = true;

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

        PlayTravelAudio(selectedDestination, false);

        if (onWarpStarted != null)
            onWarpStarted.Invoke();

        if (monitorScreen != null)
            monitorScreen.ShowStatus(warpingMessage);

        if (warpSequenceController != null)
            warpSequenceController.StartWarpSequence();

        ResetAllDestinations();

        yield return new WaitForSeconds(destinationAppearDelay);

        ActivateDestination(selectedDestination);
        currentDestination = selectedDestination;

        UpdateMonitorPreview(currentDestination);

        if (currentDestination == DestinationId.Earth && !hasTriggeredEarthMove)
        {
            hasTriggeredEarthMove = true;

            if (earthTriggeredObject != null)
                StartCoroutine(MoveEarthTriggeredObject());
        }

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

    private void PlayTravelAudio(DestinationId destinationId, bool isIntroWarp)
    {
        if (travelAudioSource == null)
            return;

        travelAudioSource.Stop();

        if (isIntroWarp)
        {
            if (introWarpClip != null)
                travelAudioSource.PlayOneShot(introWarpClip);
            else if (blackHoleWarpClip != null)
                travelAudioSource.PlayOneShot(blackHoleWarpClip);

            return;
        }

        if (destinationId == DestinationId.BlackHole)
        {
            if (blackHoleWarpClip != null)
                travelAudioSource.PlayOneShot(blackHoleWarpClip);
        }
        else if (destinationId == DestinationId.Earth)
        {
            if (earthWarpClip != null)
                travelAudioSource.PlayOneShot(earthWarpClip);
        }
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

    private IEnumerator MoveEarthTriggeredObject()
    {
        Vector3 startPos = earthTriggeredObject.position;
        Vector3 targetPos = startPos + earthMoveDirection.normalized * earthMoveAmount;

        while (Vector3.Distance(earthTriggeredObject.position, targetPos) > 0.01f)
        {
            earthTriggeredObject.position = Vector3.MoveTowards(
                earthTriggeredObject.position,
                targetPos,
                earthMoveSpeed * Time.deltaTime
            );

            yield return null;
        }

        earthTriggeredObject.position = targetPos;
    }
}