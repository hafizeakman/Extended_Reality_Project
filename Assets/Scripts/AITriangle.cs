using UnityEngine;

public class AITriangle : MonoBehaviour
{
    public float speed = 20f;
    public float direction = 1f;

    [HideInInspector] public bool frozen = false;
    [HideInInspector] public float pulseScale = 1f;

    RectTransform rt;
    Vector3 baseScale;

    void Start()
    {
        rt = GetComponent<RectTransform>();
        baseScale = rt.localScale;
    }

    void Update()
    {
        if (!frozen)
            rt.Rotate(0f, 0f, speed * direction * Time.deltaTime);

        rt.localScale = baseScale * pulseScale;
    }
}