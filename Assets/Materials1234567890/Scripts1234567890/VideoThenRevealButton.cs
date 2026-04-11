using UnityEngine;
using UnityEngine.Video;

public class VideoThenRevealButton : MonoBehaviour
{
    [Header("Video")]
    public VideoPlayer videoPlayer;

    [Header("Button To Reveal")]
    public CurtainButtonController curtainButtonController;

    [Header("Timing")]
    public float revealDelay = 5f;

    [Header("Safety")]
    public bool playOnlyOnce = true;

    private bool hasPlayed = false;

    public void StartSequence()
    {
        if (playOnlyOnce && hasPlayed)
            return;

        hasPlayed = true;

        if (videoPlayer != null)
        {
            videoPlayer.Play();
        }
        else
        {
            Debug.LogWarning("VideoPlayer is not assigned.");
        }

        Invoke(nameof(RevealButton), revealDelay);
    }

    private void RevealButton()
    {
        if (curtainButtonController != null)
        {
            curtainButtonController.RevealButton();
        }
        else
        {
            Debug.LogWarning("CurtainButtonController is not assigned.");
        }
    }
}