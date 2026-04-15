using System.Collections;
using UnityEngine;

public class AIButtonTrigger : MonoBehaviour
{
    [Header("AI")]
    public AIController aiController;
    public AIEmotion selectedEmotion = AIEmotion.Neutral;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip audioClip;

    [Header("Speaking")]
    public bool makeCompanionSpeak = true;
    public bool stopSpeakingAfterAudio = true;
    public float fallbackSpeakDuration = 2f;

    [Header("Play On Awake")]
    public bool playOnAwake = false;
    public float playDelay = 2f; // 🔥 delay after scene starts

    [Header("Safety")]
    public bool triggerOnlyOnce = false;

    private bool hasTriggered = false;
    private bool isBusy = false;
    private Coroutine finishRoutine;

    private void Awake()
    {
        if (playOnAwake)
        {
            StartCoroutine(DelayedStart());
        }
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(playDelay);
        TriggerAI();
    }

    public void TriggerAI()
    {
        if (isBusy)
        {
            Debug.Log("AI is already speaking, ignoring input.");
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

        aiController.SetEmotion(selectedEmotion);

        float duration = fallbackSpeakDuration;
        if (audioClip != null)
            duration = audioClip.length;

        if (makeCompanionSpeak)
            aiController.StartSpeaking();

        if (audioSource != null && audioClip != null)
        {
            audioSource.PlayOneShot(audioClip);
        }
        else if (audioClip != null && audioSource == null)
        {
            Debug.LogWarning("AudioSource missing.");
        }

        isBusy = true;

        if (finishRoutine != null)
            StopCoroutine(finishRoutine);

        finishRoutine = StartCoroutine(FinishAfterDelay(duration));
    }

    private IEnumerator FinishAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (aiController != null && stopSpeakingAfterAudio)
            aiController.StopSpeaking();

        isBusy = false;
        finishRoutine = null;
    }
}