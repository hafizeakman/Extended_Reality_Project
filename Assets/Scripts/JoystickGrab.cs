using UnityEngine;
using UnityEngine.InputSystem;

public class JoystickGrab : MonoBehaviour
{
    [Header("References")]
    public MirrorController mirrorController;

    // Assign these in the Inspector:
    // Left Trigger  -> XRI LeftHand Interaction/Activate Value
    // Right Trigger -> XRI RightHand Interaction/Activate Value
    [Header("VR Input Actions")]
    public InputActionReference leftTrigger;
    public InputActionReference rightTrigger;

    void OnEnable()
    {
        leftTrigger?.action.Enable();
        rightTrigger?.action.Enable();
    }

    void OnDisable()
    {
        leftTrigger?.action.Disable();
        rightTrigger?.action.Disable();
    }

    void Update()
    {
        if (mirrorController == null) return;

        float input = 0f;

        // --- VR Triggers ---
        // Left trigger = rotate left (-1), Right trigger = rotate right (+1)
        float left  = leftTrigger  != null ? leftTrigger.action.ReadValue<float>()  : 0f;
        float right = rightTrigger != null ? rightTrigger.action.ReadValue<float>() : 0f;

        // Subtract left from right so they don't cancel if both pressed
        input = right - left;

        // --- Keyboard fallback (Z = right, X = left) for Device Simulator ---
        if (Mathf.Abs(input) < 0.1f && Keyboard.current != null)
        {
            if (Keyboard.current.zKey.isPressed)      input =  1f;
            else if (Keyboard.current.xKey.isPressed) input = -1f;
        }

        bool isMoving = Mathf.Abs(input) > 0.1f;
        mirrorController.RotateByInput(input, isMoving);
    }
}