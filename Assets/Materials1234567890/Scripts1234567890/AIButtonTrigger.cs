using System.Collections;
using UnityEngine;

public class AIButtonTrigger : MonoBehaviour
{
    [Header("AI")]
    public AIController aiController;
    public GameObject aiVisualRoot; // drag the AI canvas/object here
    public AIEmotion selectedEmotion = AIEmotion.Neutral;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip audioClip;

    [Header("Speaking")]
    public bool makeCompanionSpeak = true;
    public bool stopSpeakingAfterAudio = true;
    public float fallbackSpeakDuration = 2f;

    [Header("Popup")]
    public bool hideOnStart = true;
    public bool hideAfterFinished = false;
    public float hideDelay = 0f;

    [Header("Safety")]
    public bool triggerOnlyOnce = false;

    private bool hasTriggered = false;
    private bool isBusy = false;

    private void Start()
    {
        if (hideOnStart && aiVisualRoot != null)
        {
            aiVisualRoot.SetActive(false);
        }
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

        if (aiVisualRoot != null)
        {
            aiVisualRoot.SetActive(true);
        }

        aiController.SetEmotion(selectedEmotion);

        float duration = fallbackSpeakDuration;
        if (audioClip != null)
            duration = audioClip.length;

        if (makeCompanionSpeak)
        {
            aiController.StartSpeaking();
        }

        if (audioSource != null && audioClip != null)
        {
            audioSource.PlayOneShot(audioClip);
        }

        isBusy = true;
        StartCoroutine(FinishAfterDelay(duration));
    }

    private IEnumerator FinishAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (aiController != null && stopSpeakingAfterAudio)
        {
            aiController.StopSpeaking();
        }

        if (hideAfterFinished && aiVisualRoot != null)
        {
            if (hideDelay > 0f)
                yield return new WaitForSeconds(hideDelay);

            aiVisualRoot.SetActive(false);
        }

        isBusy = false;
    }
}