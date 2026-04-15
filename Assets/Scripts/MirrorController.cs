using System.Collections;
using UnityEngine;
using TMPro;

public class MirrorController : MonoBehaviour
{
    [Header("Settings")]
    public float rotationSpeed = 30f;
    public float targetAngle = 141f;
    public float tolerance = 5f;

    [Header("Laser Segments")]
    public LineRenderer laserSegment1;
    public LineRenderer laserSegment2;

    [Header("Laser Positions")]
    public Transform tubeAOrigin;
    public Transform mirrorPoint;
    public Transform tubeBDestination;

    [Header("Audio Clips")]
    public AudioClip electricalBuzzClip;
    public AudioClip squeakClip;
    public AudioClip laserShootClip;

    [Header("UI")]
    public TextMeshProUGUI statusText;

    private float accumulatedAngle = 0f;
    private AudioSource buzzSource;
    private AudioSource oneShotSource;
    private bool isAligned = false;
    private bool wasMoving = false;

    void Start()
    {
        buzzSource = gameObject.AddComponent<AudioSource>();
        buzzSource.clip = electricalBuzzClip;
        buzzSource.loop = true;
        buzzSource.playOnAwake = false;

        oneShotSource = gameObject.AddComponent<AudioSource>();
        oneShotSource.loop = false;
        oneShotSource.playOnAwake = false;

        if (laserSegment1 != null) laserSegment1.gameObject.SetActive(false);
        if (laserSegment2 != null) laserSegment2.gameObject.SetActive(false);

        if (electricalBuzzClip != null) buzzSource.Play();

        accumulatedAngle = transform.localEulerAngles.z;
        if (accumulatedAngle > 180f) accumulatedAngle -= 360f;

        SetStatusText("ERROR: MIRROR NOT ALIGNED", Color.red);
    }

    public void RotateByInput(float input, bool isMoving)
    {
        if (isAligned) return;

        HandleSqueak(isMoving);

        float delta = input * rotationSpeed * Time.deltaTime;
        accumulatedAngle += delta;
        transform.Rotate(Vector3.forward, delta, Space.Self);

        CheckAlignment();
    }

    public void RotateLeft(bool isMoving)  => RotateByInput(-1f, isMoving);
    public void RotateRight(bool isMoving) => RotateByInput( 1f, isMoving);

    void HandleSqueak(bool isMoving)
    {
        if (isMoving && !wasMoving)
            if (squeakClip != null) oneShotSource.PlayOneShot(squeakClip);
        wasMoving = isMoving;
    }

    void CheckAlignment()
    {
        float normalized = accumulatedAngle % 360f;
        if (normalized > 180f)  normalized -= 360f;
        if (normalized < -180f) normalized += 360f;

        float targetNorm = targetAngle % 360f;
        if (targetNorm > 180f)  targetNorm -= 360f;
        if (targetNorm < -180f) targetNorm += 360f;

        float diff = Mathf.Abs(Mathf.DeltaAngle(normalized, targetNorm));

        if (diff <= tolerance)
        {
            isAligned = true;
            Debug.Log("ALIGNED! Firing laser.");
            StartCoroutine(SuccessSequence());
        }
    }

    void SetStatusText(string message, Color color)
    {
        if (statusText == null) return;
        statusText.text = message;
        statusText.color = color;
    }

    IEnumerator SuccessSequence()
    {
        buzzSource.Stop();

        SetStatusText("SUCCESS: LASER FIRING", Color.green);

        if (laserSegment1 != null)
        {
            laserSegment1.gameObject.SetActive(true);
            laserSegment1.SetPosition(0, tubeAOrigin.position);
            laserSegment1.SetPosition(1, mirrorPoint.position);
        }

        yield return new WaitForSeconds(0.2f);

        if (laserSegment2 != null)
        {
            laserSegment2.gameObject.SetActive(true);
            laserSegment2.SetPosition(0, mirrorPoint.position);
            laserSegment2.SetPosition(1, tubeBDestination.position);
        }

        if (laserShootClip != null) oneShotSource.PlayOneShot(laserShootClip);
    }
}