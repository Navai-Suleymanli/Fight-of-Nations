using UnityEngine;

public class WeaponMovementDetection : MonoBehaviour
{
    private Vector3 previousPosition;
    private Quaternion previousRotation;
    private bool isFirstUpdate = true;

    void Update()
    {
        if (isFirstUpdate)
        {
            // Initialize on the first Update call
            previousPosition = transform.position;
            previousRotation = transform.rotation;
            isFirstUpdate = false;
            return;
        }

        // Check for movement
        Vector3 movementDirection = transform.position - previousPosition;
        if (movementDirection.x > 0)
        {
            Debug.Log("Moving Right");
        }
        else if (movementDirection.x < 0)
        {
            Debug.Log("Moving Left");
        }

        // Check for rotation
        float angle = Quaternion.Angle(transform.rotation, previousRotation);
        if (angle > 0) // There's some rotation
        {
            // Determine the direction of rotation
            Vector3 rotationDirection = transform.rotation * Vector3.up - previousRotation * Vector3.up;
            if (rotationDirection.x > 0)
            {
                Debug.Log("Rotating Right");
            }
            else if (rotationDirection.x < 0)
            {
                Debug.Log("Rotating Left");
            }
        }

        // Update previous values for the next frame
        previousPosition = transform.position;
        previousRotation = transform.rotation;
    }
}
