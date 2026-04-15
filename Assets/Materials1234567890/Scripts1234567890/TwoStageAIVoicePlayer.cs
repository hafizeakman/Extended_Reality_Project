using UnityEngine;
using System.Collections;

public class TwoStageAIVoicePlayer : MonoBehaviour
{
    [Header("AI")]
    public AIController aiController;

    [Header("First Press")]
    public AudioClip firstClip;
    public AIEmotion firstEmotion = AIEmotion.Neutral;

    [Header("Second Press And After")]
    public AudioClip secondClip;
    public AIEmotion secondEmotion = AIEmotion.Angry;

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Speaking")]
    public bool makeCompanionSpeak = true;
    public bool stopSpeakingAfterAudio = true;
    public float fallbackSpeakDuration = 2f;

    [Header("Safety")]
    public bool triggerOnlyOnce = false;

    private bool hasPlayedFirst = false;
    private bool isPlaying = false;
    private bool hasTriggered = false;
    private Coroutine finishRoutine;

    public void PlayAIResponse()
    {
        if (isPlaying)
        {
            Debug.Log("AI is already speaking.");
            return;
        }

        if (triggerOnlyOnce && hasTriggered)
            return;

        hasTriggered = true;

        if (aiController == null)
        {
            Debug.LogWarning("AIController is not assigned.");
            return;
        }

        AudioClip clipToPlay;
        AIEmotion emotionToPlay;

        if (!hasPlayedFirst)
        {
            clipToPlay = firstClip;
            emotionToPlay = firstEmotion;
            hasPlayedFirst = true;
        }
        else
        {
            clipToPlay = secondClip;
            emotionToPlay = secondEmotion;
        }

        aiController.SetEmotion(emotionToPlay);

        float duration = fallbackSpeakDuration;
        if (clipToPlay != null)
            duration = clipToPlay.length;

        if (makeCompanionSpeak)
            aiController.StartSpeaking();

        if (audioSource != null && clipToPlay != null)
        {
            audioSource.PlayOneShot(clipToPlay);
        }
        else if (clipToPlay != null && audioSource == null)
        {
            Debug.LogWarning("AudioSource is missing.");
        }
        else if (clipToPlay == null)
        {
            Debug.LogWarning("Selected audio clip is missing.");
        }

        isPlaying = true;

        if (finishRoutine != null)
            StopCoroutine(finishRoutine);

        finishRoutine = StartCoroutine(FinishAfterDelay(duration));
    }

    private IEnumerator FinishAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (aiController != null && stopSpeakingAfterAudio)
            aiController.StopSpeaking();

        isPlaying = false;
        finishRoutine = null;
    }

    public void ResetSequence()
    {
        hasPlayedFirst = false;
        hasTriggered = false;
        isPlaying = false;

        if (finishRoutine != null)
        {
            StopCoroutine(finishRoutine);
            finishRoutine = null;
        }

        if (aiController != null)
            aiController.StopSpeaking();
    }
}