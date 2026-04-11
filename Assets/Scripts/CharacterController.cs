using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class VRBodyCollider : MonoBehaviour
{
    [Header("Assign this in the Inspector")]
    public Camera xrCamera;

    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector3 headLocalPosition = transform.InverseTransformPoint(xrCamera.transform.position);

        characterController.center = new Vector3(
            headLocalPosition.x,
            characterController.height / 2,
            headLocalPosition.z
        );
    }
}