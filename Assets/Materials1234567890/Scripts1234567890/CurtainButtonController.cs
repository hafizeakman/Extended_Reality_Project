using UnityEngine;

public class CurtainButtonController : MonoBehaviour
{
    [Header("Curtain Target")]
    public Transform curtain;

    [Header("Curtain Movement")]
    public float curtainRiseAmount = 3f;
    public float curtainSpeed = 2f;

    [Header("Button Reveal Movement")]
    public float buttonRiseAmount = 2f;
    public float buttonRiseSpeed = 2f;

    private Vector3 curtainTargetPosition;
    private Vector3 buttonTargetPosition;

    private bool isCurtainMoving = false;
    private bool isButtonMoving = false;

    public void OpenCurtain()
    {
        if (curtain == null)
        {
            Debug.LogWarning("Curtain is not assigned.");
            return;
        }

        curtainTargetPosition = curtain.position + Vector3.up * curtainRiseAmount;
        isCurtainMoving = true;
    }

    public void RevealButton()
    {
        buttonTargetPosition = transform.position + Vector3.up * buttonRiseAmount;
        isButtonMoving = true;
    }

    private void Update()
    {
        if (isCurtainMoving && curtain != null)
        {
            curtain.position = Vector3.MoveTowards(
                curtain.position,
                curtainTargetPosition,
                curtainSpeed * Time.deltaTime
            );

            if (Vector3.Distance(curtain.position, curtainTargetPosition) < 0.01f)
            {
                curtain.position = curtainTargetPosition;
                isCurtainMoving = false;
            }
        }

        if (isButtonMoving)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                buttonTargetPosition,
                buttonRiseSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, buttonTargetPosition) < 0.01f)
            {
                transform.position = buttonTargetPosition;
                isButtonMoving = false;
            }
        }
    }
}