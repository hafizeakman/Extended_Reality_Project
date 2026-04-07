using UnityEngine;
using System.Collections;

public class WarpSequenceController : MonoBehaviour
{
    [Header("Particle References")]
    public ParticleSystem warpParticles;
    public ParticleSystemRenderer particleRenderer;

    [Header("Initial Hidden Spread")]
    public float startupSimulateTime = 2f;
    public float hiddenSize = 0f;
    public float baseSpeed = 2f;
    public float baseLifetime = 5f;
    public float baseEmission = 30f;
    public float baseVelocityScale = 1f;

    [Header("Build-Up")]
    public float targetBuildUpSpeed = 10f;
    public float speedAcceleration = 2f;

    [Header("Independent Size Control")]
    public float visibleSize = 1.5f;
    public float sizeIncreaseRate = 0.5f;
    public float sizeDecreaseRate = 0.1f;

    [Header("Lifetime Relation")]
    public float lifetimeRatio = 5f;

    [Header("Warp")]
    public float warpDuration = 2f;
    public float warpSpeed = 80f;
    public float warpLifetime = 0.2f;
    public float warpLength = 18f;
    public float warpEmission = 140f;
    public float warpSize = 1.5f;
    public float warpVelocityScale = 5f;

    [Header("Slowdown")]
    public float decelerationRate = 20f;

    private ParticleSystem.MainModule mainModule;
    private ParticleSystem.EmissionModule emissionModule;
    private bool isWarping = false;

    void Start()
    {
        if (warpParticles == null)
        {
            Debug.LogWarning("WarpSequenceController: warpParticles not assigned.");
            return;
        }

        if (particleRenderer == null)
        {
            particleRenderer = warpParticles.GetComponent<ParticleSystemRenderer>();
        }

        mainModule = warpParticles.main;
        emissionModule = warpParticles.emission;

        // Stretch mode from the beginning so length changes are visible during buildup
        if (particleRenderer != null)
        {
            particleRenderer.renderMode = ParticleSystemRenderMode.Stretch;
        }

        SetHiddenSpreadState();

        warpParticles.Play();
        warpParticles.Simulate(startupSimulateTime, true, true);
        warpParticles.Play();
    }

    public void StartWarpSequence()
    {
        if (warpParticles == null || isWarping)
            return;

        StartCoroutine(WarpSequence());
    }

    private IEnumerator WarpSequence()
    {
        isWarping = true;

        if (!warpParticles.isPlaying)
            warpParticles.Play();

        float currentSpeed = baseSpeed;
        float currentSize = hiddenSize;

        float minLifetime = baseLifetime / Mathf.Max(lifetimeRatio, 1f);

        // BUILD-UP: ends when speed reaches targetBuildUpSpeed
        while (currentSpeed < targetBuildUpSpeed)
        {
            currentSpeed = Mathf.MoveTowards(
                currentSpeed,
                targetBuildUpSpeed,
                speedAcceleration * Time.deltaTime
            );

            currentSize = Mathf.MoveTowards(
                currentSize,
                visibleSize,
                sizeIncreaseRate * Time.deltaTime
            );

            float progress = Mathf.InverseLerp(baseSpeed, targetBuildUpSpeed, currentSpeed);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);

            float currentLength = currentSpeed / 3f;
            float currentLifetime = Mathf.Lerp(baseLifetime, minLifetime, easedProgress);

            ApplyValues(
                currentLifetime,
                currentSpeed,
                baseEmission,
                currentLength,
                currentSize,
                baseVelocityScale
            );

            yield return null;
        }

        // WARP PHASE
        ApplyValues(
            warpLifetime,
            warpSpeed,
            warpEmission,
            warpLength,
            warpSize,
            warpVelocityScale
        );

        yield return new WaitForSeconds(warpDuration);

        // SLOWDOWN PHASE
        float currentSlowSpeed = warpSpeed;
        float currentSlowSize = warpSize;

        while (currentSlowSpeed > 0f || currentSlowSize > 0f)
        {
            currentSlowSpeed = Mathf.MoveTowards(
                currentSlowSpeed,
                0f,
                decelerationRate * Time.deltaTime
            );

            currentSlowSize = Mathf.MoveTowards(
                currentSlowSize,
                0f,
                sizeDecreaseRate * Time.deltaTime
            );

            float progress = Mathf.Clamp01(currentSlowSpeed / Mathf.Max(targetBuildUpSpeed, 0.0001f));
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);

            float currentLength = currentSlowSpeed / 3f;
            float currentLifetime = Mathf.Lerp(baseLifetime, minLifetime, easedProgress);
            float currentEmission = Mathf.Lerp(0f, baseEmission, easedProgress);
            float currentVelocityScale = Mathf.Lerp(baseVelocityScale, warpVelocityScale, easedProgress);

            ApplyValues(
                currentLifetime,
                currentSlowSpeed,
                currentEmission,
                currentLength,
                currentSlowSize,
                currentVelocityScale
            );

            yield return null;
        }

        SetFullyStoppedHiddenState();

        isWarping = false;
    }

    private void SetHiddenSpreadState()
    {
        ApplyValues(
            baseLifetime,
            baseSpeed,
            baseEmission,
            baseSpeed / 3f,
            hiddenSize,
            baseVelocityScale
        );
    }

    private void SetFullyStoppedHiddenState()
    {
        ApplyValues(
            baseLifetime,
            0f,
            0f,
            0f,
            0f,
            baseVelocityScale
        );
    }

    private void ApplyValues(
        float lifetime,
        float speed,
        float emission,
        float length,
        float size,
        float velocityScale)
    {
        mainModule.startLifetime = lifetime;
        mainModule.startSpeed = speed;
        emissionModule.rateOverTime = emission;

        ParticleSystem.MinMaxCurve sizeCurve = mainModule.startSize;
        sizeCurve.constant = size;
        mainModule.startSize = sizeCurve;

        if (particleRenderer != null)
        {
            particleRenderer.lengthScale = length;
            particleRenderer.velocityScale = velocityScale;
        }
    }
}