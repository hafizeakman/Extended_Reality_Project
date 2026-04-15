using UnityEngine;

public class ControlPanelZone : MonoBehaviour
{
    public JoystickGrab joystickGrab;

    void Start()
    {
        joystickGrab.enabled = false;
        Debug.Log("ControlPanelZone ready - JoystickGrab disabled");
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something entered zone: " + other.gameObject.name);
        if (other.CompareTag("Player"))
        {
            joystickGrab.enabled = true;
            Debug.Log("Player entered - JoystickGrab enabled");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            joystickGrab.enabled = false;
            Debug.Log("Player exited - JoystickGrab disabled");
        }
    }
}