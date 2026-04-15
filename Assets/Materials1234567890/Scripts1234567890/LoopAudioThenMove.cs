using System.Collections;
using UnityEngine;

public class LoopAudioThenMove : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip audioClip;
    public bool playOnAwake = false;
    public float audioLoopDuration = 3f;

    [Header("Delay After Audio")]
    public float delayBeforeMovement = 1f;

    [Header("Movement")]
    public Transform objectToMove;
    public Vector3 moveDirection = Vector3.down;
    public float moveSpeed = 1f;
    public float moveDistance = 2f;

    [Header("Safety")]
    public bool triggerOnlyOnce = true;

    private bool hasTriggered = false;
    private bool isMoving = false;

    private void Awake()
    {
        if (playOnAwake)
        {
            TriggerSequence();
        }
    }

    public void TriggerSequence()
    {
        if (triggerOnlyOnce && hasTriggered)
            return;

        hasTriggered = true;
        StartCoroutine(MainRoutine());
    }

    private IEnumerator MainRoutine()
    {
        // AUDIO
        if (audioSource != null && audioClip != null)
        {
            audioSource.clip = audioClip;
            audioSource.loop = true;
            audioSource.Play();

            if (audioLoopDuration > 0f)
                yield return new WaitForSeconds(audioLoopDuration);

            audioSource.Stop();
            audioSource.loop = false;
            audioSource.clip = null;
        }
        else
        {
            Debug.LogWarning("Missing AudioSource or AudioClip.");
        }

        // DELAY BEFORE MOVEMENT
        if (delayBeforeMovement > 0f)
            yield return new WaitForSeconds(delayBeforeMovement);

        // MOVEMENT
        if (objectToMove != null)
        {
            yield return StartCoroutine(MoveObjectRoutine());
        }
        else
        {
            Debug.LogWarning("Object To Move is not assigned.");
        }
    }

    private IEnumerator MoveObjectRoutine()
    {
        if (isMoving)
            yield break;

        isMoving = true;

        Vector3 startPosition = objectToMove.position;
        Vector3 direction = moveDirection.normalized;
        Vector3 targetPosition = startPosition + direction * moveDistance;

        while (Vector3.Distance(objectToMove.position, targetPosition) > 0.01f)
        {
            objectToMove.position = Vector3.MoveTowards(
                objectToMove.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        objectToMove.position = targetPosition;
        isMoving = false;
    }
}