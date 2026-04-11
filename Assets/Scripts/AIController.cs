using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AIController : MonoBehaviour
{
    [Header("Triangles")]
    public AITriangle triOuter;
    public AITriangle triMid;
    public AITriangle triInner;

    [Header("Core")]
    public Image core;
    public Image coreGlow;

    AIEmotion currentEmotion = AIEmotion.Neutral;
    bool isSpeaking = false;
    Coroutine speakRoutine;

    void Start() => SetEmotion(AIEmotion.Neutral);

    [ContextMenu("Test Neutral")]
    void TestNeutral() => SetEmotion(AIEmotion.Neutral);

    [ContextMenu("Test Happy")]
    void TestHappy() => SetEmotion(AIEmotion.Happy);

    [ContextMenu("Test Angry")]
    void TestAngry() => SetEmotion(AIEmotion.Angry);

    [ContextMenu("Test Annoyed")]
    void TestAnnoyed() => SetEmotion(AIEmotion.Annoyed);

    [ContextMenu("Test Speaking ON")]
    void TestSpeakOn() => StartSpeaking();

    [ContextMenu("Test Speaking OFF")]
    void TestSpeakOff() => StopSpeaking();

    public void SetEmotion(AIEmotion emotion)
    {
        StopAllCoroutines();
        // reset shake without moving the canvas from its real position
        Vector3 pos = transform.localPosition;
        pos.x = Mathf.Round(pos.x);
        transform.localPosition = pos;
        triOuter.frozen = false;
        triInner.frozen = false;
        triInner.pulseScale = 1f;
        currentEmotion = emotion;

        switch (emotion)
        {
            case AIEmotion.Neutral:
                triOuter.speed = 18f;  triOuter.direction =  1f;
                triMid.speed   = 30f;  triMid.direction   = -1f;
                triInner.speed = 50f;  triInner.direction =  1f;
                SetColors("#c084fc", "#7a40b0", "#3a1a5a", "#9050d0", "#d8a0ff");
                StartCoroutine(PulseCore(0.85f, 1.25f, 1.5f));
                StartCoroutine(PulseGlow(0.4f, 1f, 1.5f));
                break;

            case AIEmotion.Happy:
                // starts normal, then chaos, then alignment, repeat
                triOuter.speed = 18f;  triOuter.direction =  1f;
                triMid.speed   = 30f;  triMid.direction   = -1f;
                triInner.speed = 50f;  triInner.direction =  1f;
                SetColors("#d0ff80", "#a0d040", "#6a9028", "#c0ff60", "#eeffaa");
                // glow keeps blinking throughout the whole happy state
                StartCoroutine(HappyGlowBlink());
                StartCoroutine(HappyOutburst());
                break;

            case AIEmotion.Angry:
                // divided all speeds by 8 - intense but not insane
                triOuter.speed = 13f;  triOuter.direction =  1f;
                triMid.speed   = 20f;  triMid.direction   = -1f;
                triInner.speed = 32f;  triInner.direction =  1f;
                SetColors("#ff6030", "#cc3010", "#882010", "#ff4020", "#ff9060");
                StartCoroutine(PulseCore(0.9f, 1.1f, 3f));
                // tiny tremble only
                StartCoroutine(ShakeLoop());
                break;

            case AIEmotion.Annoyed:
                triOuter.speed = 1.6f; triOuter.direction =  1f;
                triMid.speed   = 30f;  triMid.direction   = -1f;
                triInner.speed = 50f;  triInner.direction =  1f;
                SetColors("#6a3888", "#3a1a58", "#1a0a2a", "#3d1a60", "#6a3890");
                StartCoroutine(PulseCore(0.7f, 0.9f, 0.5f));
                StartCoroutine(PulseGlow(0.1f, 0.4f, 0.5f));
                break;
        }

        if (isSpeaking) StartCoroutine(SpeakingPulse());
    }

    public void StartSpeaking()
    {
        isSpeaking = true;
        if (speakRoutine != null) StopCoroutine(speakRoutine);
        speakRoutine = StartCoroutine(SpeakingPulse());
    }

    public void StopSpeaking()
    {
        isSpeaking = false;
        if (speakRoutine != null) StopCoroutine(speakRoutine);
        triInner.frozen = false;
        triInner.pulseScale = 1f;
        SetEmotion(currentEmotion);
    }

    // faster wave, bigger triangle, brighter glow
    IEnumerator SpeakingPulse()
    {
        triInner.speed = 0f;
        triInner.frozen = true;
        float t = 0f;
        while (true)
        {
            // 7f is very fast and snappy
            t += Time.deltaTime * 7f;

            // bigger triangle wave - 1f to 1.8f
            triInner.pulseScale = Mathf.Lerp(1f, 1.8f, (Mathf.Sin(t) + 1f) / 2f);

            // core gets big during speaking
            float coreScale = Mathf.Lerp(1.2f, 2f, (Mathf.Sin(t + Mathf.PI) + 1f) / 2f);
            core.transform.localScale = Vector3.one * coreScale;

            // core alpha never goes dark
            float coreAlpha = Mathf.Lerp(0.7f, 1f, (Mathf.Sin(t + Mathf.PI) + 1f) / 2f);
            Color c = core.color;
            c.a = coreAlpha;
            core.color = c;

            // glow absolutely blooms - 3x size at peak
            float glowScale = Mathf.Lerp(1.5f, 3f, (Mathf.Sin(t) + 1f) / 2f);
            coreGlow.transform.localScale = Vector3.one * glowScale;
            float glowAlpha = Mathf.Lerp(0.6f, 1f, (Mathf.Sin(t) + 1f) / 2f);
            Color gc = coreGlow.color;
            gc.a = glowAlpha;
            coreGlow.color = gc;

            yield return null;
        }
    }

    // the happy outburst loop
    // normal spin → accidental snap → FREAK OUT fast → slowly calm down → repeat
    IEnumerator HappyOutburst()
    {
        while (true)
        {
            // phase 1 - spinning normally like nothing is happening
            triOuter.speed = 18f; triOuter.direction =  1f;
            triMid.speed   = 30f; triMid.direction   = -1f;
            triInner.speed = 50f; triInner.direction =  1f;
            yield return new WaitForSeconds(1.2f);

            // phase 2 - accidental alignment, all snap to same direction
            // like whoops they just lined up
            triOuter.speed = 50f; triOuter.direction = 1f;
            triMid.speed   = 50f; triMid.direction   = 1f;
            triInner.speed = 50f; triInner.direction = 1f;
            core.transform.localScale = Vector3.one * 1.6f;
            yield return new WaitForSeconds(0.35f);

            // phase 3 - FREAKING OUT, spinning super fast in excitement
            // like clapping or yelling yohooo
            triOuter.speed = 200f; triOuter.direction =  1f;
            triMid.speed   = 260f; triMid.direction   = -1f;
            triInner.speed = 320f; triInner.direction =  1f;
            yield return new WaitForSeconds(0.5f);

            // phase 4 - slowly calming down, like catching breath
            // lerp speeds down over time manually
            float calmTimer = 0f;
            float calmDuration = 1.2f;
            while (calmTimer < calmDuration)
            {
                calmTimer += Time.deltaTime;
                float t = calmTimer / calmDuration;
                // eases from fast back to normal
                triOuter.speed = Mathf.Lerp(200f, 18f, t);
                triMid.speed   = Mathf.Lerp(260f, 30f, t);
                triInner.speed = Mathf.Lerp(320f, 50f, t);
                core.transform.localScale = Vector3.Lerp(
                    Vector3.one * 1.6f, Vector3.one, t
                );
                yield return null;
            }

            // gap before it happens again
            yield return new WaitForSeconds(0.4f);
        }
    }

    // glow keeps blinking the whole time during happy
    // independent of the outburst so it never stops
    IEnumerator HappyGlowBlink()
    {
        float t = 0f;
        while (true)
        {
            t += Time.deltaTime * 3f;
            float a = Mathf.Lerp(0.3f, 1f, (Mathf.Sin(t) + 1f) / 2f);
            Color gc = coreGlow.color;
            gc.a = a;
            coreGlow.color = gc;
            float s = Mathf.Lerp(0.9f, 1.5f, (Mathf.Sin(t) + 1f) / 2f);
            coreGlow.transform.localScale = Vector3.one * s;
            yield return null;
        }
    }

    IEnumerator PulseCore(float minS, float maxS, float speed)
    {
        float t = 0f;
        while (true)
        {
            t += Time.deltaTime * speed;
            float s = Mathf.Lerp(minS, maxS, (Mathf.Sin(t) + 1f) / 2f);
            core.transform.localScale = Vector3.one * s;
            yield return null;
        }
    }

    IEnumerator PulseGlow(float minA, float maxA, float speed)
    {
        float t = 0f;
        while (true)
        {
            t += Time.deltaTime * speed;
            float a = Mathf.Lerp(minA, maxA, (Mathf.Sin(t) + 1f) / 2f);
            Color c = coreGlow.color;
            c.a = a;
            coreGlow.color = c;
            yield return null;
        }
    }

    // tiny tremble, stays put
    // faster shake but stays in the same tiny boundary
    IEnumerator ShakeLoop()
    {
        Vector3 origin = transform.localPosition;
        while (true)
        {
            // same 1.5 boundary, but 0.08 period instead of 0.35 - much faster
            float x = Mathf.Sin(Time.time * Mathf.PI / 0.09f) * 0.01f;
            transform.localPosition = origin + new Vector3(x, 0f, 0f);
            yield return null;
        }
    }

    void SetColors(string c1, string c2, string c3, string cCore, string cGlow)
    {
        triInner.GetComponent<Image>().color = Hex(c1);
        triMid.GetComponent<Image>().color   = Hex(c2);
        triOuter.GetComponent<Image>().color = Hex(c3);
        core.color     = Hex(cCore);
        coreGlow.color = Hex(cGlow);
    }

    Color Hex(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out Color c);
        return c;
    }
}

public enum AIEmotion { Neutral, Happy, Angry, Annoyed, Speaking }