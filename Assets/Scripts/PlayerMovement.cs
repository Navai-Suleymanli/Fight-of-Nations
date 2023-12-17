using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Vector3 move;
    public CharacterController controller;
    [SerializeField]
    private float walkSpeed = 0f;
    [SerializeField]
    private float sprintSpeed = 0f;
    [SerializeField]
    private bool isRunning = false;
    [SerializeField]
    private bool isAiming = false;

    private float gravityValue = -9.81f; // Gravity value
    private float verticalVelocity = 0f; // Vertical velocity due to gravity

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = 0f; // Reset vertical velocity if on the ground
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            isRunning = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRunning = false;
        }
        if (Input.GetKey(KeyCode.Mouse1))
        {
            isAiming = true;
        }
        else
        {
            isAiming = false;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        move = transform.right * x + transform.forward * z;

        if (!isRunning)
        {
            controller.Move(move * walkSpeed * Time.deltaTime);
        }
        if (isRunning && !isAiming)
        {
            controller.Move(move * sprintSpeed * Time.deltaTime);
        }
        if (isRunning && isAiming)
        {
            controller.Move(move * walkSpeed * Time.deltaTime);
        }

        // Apply gravity
        verticalVelocity += gravityValue * Time.deltaTime;
        controller.Move(new Vector3(0, verticalVelocity, 0) * Time.deltaTime);
    }
}
