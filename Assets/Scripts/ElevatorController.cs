using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.XR;
using System.Collections.Generic;

public class ElevatorController : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public Transform roomContainer;
    public ElevatorDoor elevatorDoor;

    [Header("Movement")]
    public float moveDistance = 100f;
    public float speed = 3f;
    public float shakeAmount = 0.0001f;

    [Header("Haptics")]
    public float hapticAmplitude = 0.3f;
    public float hapticDuration = 0.1f;
    public float hapticInterval = 0.2f;

    [Header("Audio Clips")]
    public AudioClip fanClip;
    public AudioClip dingClip;
    public AudioClip doorOpenClip;
    public AudioClip doorCloseClip;

    [Header("Display")]
    public TextMeshPro floorDisplay;

    private AudioSource fanSource;
    private AudioSource oneShotSource;
    private bool isMoving = false;
    private Vector3 startPosition;
    private Vector3 elevatorStartPosition;

    void Start()
    {
        startPosition = roomContainer.position;
        elevatorStartPosition = transform.position;

        // Create two audio sources automatically
        fanSource = gameObject.AddComponent<AudioSource>();
        fanSource.loop = true;
        fanSource.playOnAwake = false;
        fanSource.clip = fanClip;

        oneShotSource = gameObject.AddComponent<AudioSource>();
        oneShotSource.loop = false;
        oneShotSource.playOnAwake = false;
    }

    public void PlayDingThenOpen()
    {
        StartCoroutine(DingThenOpen());
    }

    IEnumerator DingThenOpen()
    {
        if (dingClip != null)
        {
            oneShotSource.PlayOneShot(dingClip);
            yield return new WaitForSeconds(dingClip.length);
        }

        if (doorOpenClip != null)
            oneShotSource.PlayOneShot(doorOpenClip);

        yield return new WaitForSeconds(0.7f);

        if (elevatorDoor != null)
            elevatorDoor.OpenDoor();
    }

    public void StartDescent()
    {
        if (!isMoving)
            StartCoroutine(CloseAndDescend());
    }

    IEnumerator CloseAndDescend()
    {
        if (doorCloseClip != null)
            oneShotSource.PlayOneShot(doorCloseClip);

        if (elevatorDoor != null)
            elevatorDoor.CloseDoor();

        yield return new WaitForSeconds(1.5f);

        StartCoroutine(MoveRooms());
    }

    void SendHaptics()
    {
        List<InputDevice> devices = new List<InputDevice>();

        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller,
            devices
        );

        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller,
            devices
        );

        foreach (InputDevice device in devices)
        {
            HapticCapabilities capabilities;
            if (device.TryGetHapticCapabilities(out capabilities))
            {
                if (capabilities.supportsImpulse)
                    device.SendHapticImpulse(0, hapticAmplitude, hapticDuration);
            }
        }
    }

    IEnumerator MoveRooms()
    {
        isMoving = true;
        float targetY = startPosition.y + moveDistance;
        float hapticTimer = 0f;

        if (fanSource != null && fanClip != null)
            fanSource.Play();

        while (roomContainer.position.y < targetY - 0.1f)
        {
            float newY = Mathf.MoveTowards(
                roomContainer.position.y,
                targetY,
                speed * Time.deltaTime
            );

            roomContainer.position = new Vector3(
                startPosition.x,
                newY,
                startPosition.z
            );

            float shakeX = Random.Range(-shakeAmount, shakeAmount);
            float shakeZ = Random.Range(-shakeAmount, shakeAmount);
            transform.position = new Vector3(
                elevatorStartPosition.x + shakeX,
                elevatorStartPosition.y,
                elevatorStartPosition.z + shakeZ
            );

            hapticTimer += Time.deltaTime;
            if (hapticTimer >= hapticInterval)
            {
                SendHaptics();
                hapticTimer = 0f;
            }

            if (floorDisplay != null)
            {
                int meters = Mathf.RoundToInt(targetY - roomContainer.position.y);
                floorDisplay.text = meters.ToString() + "m";
            }

            yield return null;
        }

        roomContainer.position = new Vector3(
            startPosition.x,
            targetY,
            startPosition.z
        );

        transform.position = elevatorStartPosition;
        isMoving = false;

        fanSource.Stop();

        if (dingClip != null)
        {
            oneShotSource.PlayOneShot(dingClip);
            yield return new WaitForSeconds(dingClip.length);
        }

        if (doorOpenClip != null)
            oneShotSource.PlayOneShot(doorOpenClip);

        if (elevatorDoor != null)
            elevatorDoor.OpenDoor();
    }
}