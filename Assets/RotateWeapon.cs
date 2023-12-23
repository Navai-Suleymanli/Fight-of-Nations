using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWeapon : MonoBehaviour
{
    private float currentRotation = 0f;
    private float targetRotation = 0f;
    private float rotationSpeed = 90f; // Adjust as needed for smoother rotation
    private const float MAX_ROTATION = 30f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Q Pressed");
            targetRotation = targetRotation == 0 ? MAX_ROTATION : 0;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E Pressed");
            targetRotation = targetRotation == 0 ? -MAX_ROTATION : 0;
        }

        // Gradually rotate towards the target rotation
        currentRotation = Mathf.MoveTowards(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(0, 0, currentRotation);
    }
}