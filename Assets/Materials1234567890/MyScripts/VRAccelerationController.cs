using UnityEngine;

public class VRAccelerationController : MonoBehaviour
{
    [Header("World To Move")]
    public Transform worldRoot;

    [Header("Speed Settings")]
    public float startSpeed = 0f;
    public float currentSpeed = 0f;
    public float maxSpeed = 30f;
    public float accelerationRate = 10f;
    public float decelerationRate = 8f;

    [Header("Direction")]
    public bool invertZDirection = false;

    private bool isAccelerating = false;

    void Start()
    {
        currentSpeed = startSpeed;
    }

    void Update()
    {
        UpdateSpeed();
        MoveWorld();
    }

    void UpdateSpeed()
    {
        if (isAccelerating)
        {
            currentSpeed += accelerationRate * Time.deltaTime;
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
        }
        else
        {
            currentSpeed -= decelerationRate * Time.deltaTime;
            currentSpeed = Mathf.Max(currentSpeed, startSpeed);
        }
    }

    void MoveWorld()
    {
        if (worldRoot == null) return;

        float direction = invertZDirection ? 1f : -1f;

        Vector3 move = new Vector3(0f, 0f, direction) * currentSpeed * Time.deltaTime;

        worldRoot.position += move;
    }

    // 🔘 Called when button is pressed
    public void StartAcceleration()
    {
        isAccelerating = true;
        Debug.Log("Acceleration STARTED");
    }

    // 🔘 Called when button is released
    public void StopAcceleration()
    {
        isAccelerating = false;
        Debug.Log("Acceleration STOPPED");
    }
}