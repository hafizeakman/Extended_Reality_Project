using UnityEngine;

public class ButtonPress : MonoBehaviour
{
    public float pressDepth = 0.02f;
    public float pressSpeed = 0.1f;
    public Vector3 pressDirection = Vector3.down;

    private Vector3 startPos;
    private Vector3 targetPos;

    private bool isPressed = false;
    private bool isMoving = false;

    void Start()
    {
        startPos = transform.localPosition;
        targetPos = startPos + pressDirection.normalized * pressDepth;
    }

    void Update()
    {
        if (!isMoving) return;

        transform.localPosition = Vector3.MoveTowards(
            transform.localPosition,
            targetPos,
            pressSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.localPosition, targetPos) < 0.0001f)
        {
            transform.localPosition = targetPos;
            isMoving = false;
            isPressed = true;

            Debug.Log("Button pressed and locked.");
        }
    }

    public void PressButton()
    {
        if (isPressed) return;

        isMoving = true;
    }
}