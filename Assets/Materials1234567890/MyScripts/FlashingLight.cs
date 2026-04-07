using UnityEngine;
using System.Collections;

public class FlashingLamp : MonoBehaviour
{
    public enum eColor
    {
        Red,
        Yellow,
        Green,
        Blue,
    }

    [Header("Lamp Setup")]
    public Transform lamp;
    public eColor lightColor;

    [Header("Timing")]
    public float totalEventTime = 10f;
    public float phaseOneDuration = 5f;
    public float phaseOneInterval = 0.5f;
    public float phaseTwoInterval = 0.15f;

    [Header("State")]
    public bool playOnStart = false;

    private Renderer rend;
    private bool isOn = false;
    private bool isRunning = false;
    private Coroutine flashRoutine;

    void Start()
    {
        if (lamp != null)
        {
            rend = lamp.GetComponent<Renderer>();
        }

        SetLampOff();

        if (playOnStart)
        {
            StartFlashingEvent();
        }
    }

    public void StartFlashingEvent()
    {
        if (isRunning) return;

        flashRoutine = StartCoroutine(FlashRoutine());
    }

    public void StopFlashingEvent()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
        }

        isRunning = false;
        isOn = false;
        SetLampOff();
    }

    private IEnumerator FlashRoutine()
    {
        isRunning = true;

        float elapsed = 0f;

        while (elapsed < totalEventTime)
        {
            float currentInterval;

            if (elapsed < phaseOneDuration)
            {
                currentInterval = phaseOneInterval;
            }
            else
            {
                currentInterval = phaseTwoInterval;
            }

            ToggleLamp();

            yield return new WaitForSeconds(currentInterval);
            elapsed += currentInterval;
        }

        isRunning = false;
        isOn = false;
        SetLampOff();
        flashRoutine = null;
    }

    private void ToggleLamp()
    {
        isOn = !isOn;

        if (isOn)
        {
            SetLampOn();
        }
        else
        {
            SetLampOff();
        }
    }

    private void SetLampOn()
    {
        if (rend == null) return;

        Color emissionColor = Color.black;

        switch (lightColor)
        {
            case eColor.Red:
                emissionColor = new Color(3f, 0f, 0f);
                break;
            case eColor.Yellow:
                emissionColor = new Color(3f, 2f, 0f);
                break;
            case eColor.Green:
                emissionColor = new Color(0f, 3f, 0f);
                break;
            case eColor.Blue:
                emissionColor = new Color(0f, 1f, 3f);
                break;
        }

        rend.material.EnableKeyword("_EMISSION");
        rend.material.SetColor("_EmissionColor", emissionColor);
    }

    private void SetLampOff()
    {
        if (rend == null) return;

        rend.material.EnableKeyword("_EMISSION");
        rend.material.SetColor("_EmissionColor", Color.black);
    }
}