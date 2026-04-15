using UnityEngine;

public class ElevatorButtonLogic : MonoBehaviour
{
    public ElevatorDoor elevatorDoor;
    public ElevatorController elevatorController;
    public bool isInsideButton = false;

    public void OnButtonPressed()
    {
        if (elevatorController == null)
        {
            Debug.LogError("ElevatorController not assigned on " + gameObject.name);
            return;
        }

        if (isInsideButton)
            elevatorController.StartDescent();
        else
            elevatorController.PlayDingThenOpen();
    }
}