using UnityEngine;

public class ModelResetter : MonoBehaviour
{
    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private Vector3 _startScale;
    private Rigidbody _rb;

    void Start()
    {
        // Record the original transform values at the very beginning
        _startPosition = transform.position;
        _startRotation = transform.rotation;
        _startScale = transform.localScale;
        _rb = GetComponent<Rigidbody>();
    }

    public void ResetModel()
    {
        // Snap back to start
        transform.position = _startPosition;
        transform.rotation = _startRotation;
        transform.localScale = _startScale;

        // If it has physics, stop it from moving/falling further
        if (_rb != null)
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
    }
}