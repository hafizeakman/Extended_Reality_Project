using UnityEngine;

public class ControlPanelZone : MonoBehaviour
{
    [Header("Buttons To Enable In Zone")]
    public GameObject leftButton;
    public GameObject rightButton;

    void Start()
    {
        if (leftButton != null)
            leftButton.SetActive(false);

        if (rightButton != null)
            rightButton.SetActive(false);

        Debug.Log("ControlPanelZone ready - buttons disabled");
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something entered zone: " + other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            if (leftButton != null)
                leftButton.SetActive(true);

            if (rightButton != null)
                rightButton.SetActive(true);

            Debug.Log("Player entered - buttons enabled");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (leftButton != null)
                leftButton.SetActive(false);

            if (rightButton != null)
                rightButton.SetActive(false);

            Debug.Log("Player exited - buttons disabled");
        }
    }
}