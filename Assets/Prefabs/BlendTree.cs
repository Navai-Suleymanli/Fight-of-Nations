using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendTree : MonoBehaviour
{
    [Header("Animation Settings")]
    public Animator animator;
    public float acceleration = 2.0f;
    public float aimAccelertion = 0.1f;
    public float deceleration = 2.0f;
    public float aimDeceleration = 0.5f;
    public float maxWalkVelocity = 0.3f;
    public float maxRunVelocity = 1.0f;
    public float maxAimValue = 0.2f;

    private int velocityZHash; // To control the vertical value of the blend tree.
    private int velocityXHash; // To control the horizontal value of the blend tree.
    private float velocityZ = 0.0f;
    private float velocityX = 0.0f;


    private void InitializeAnimator()
    {
        animator = GetComponent<Animator>(); // Get the Animator component attached to the game object.
        velocityZHash = Animator.StringToHash("vertical"); // Get the hash code for the "vertical" parameter.
        velocityXHash = Animator.StringToHash("horizontal"); //get the hash code for the horizontal parameter of the blend tree

    }

    private void UpdateVelocity(bool forwardPressed, bool runPressed, bool backPressed, float currentMaxVelocity, bool aimPressed)
    {
        if (forwardPressed && velocityZ < currentMaxVelocity)
        {
            velocityZ += Time.deltaTime * acceleration; // Accelerate forward.
        }
        if (backPressed && velocityZ > -currentMaxVelocity /*&& aimPressed == false*/ && runPressed==false)  // ERROR MIGHT BE IN THERE!!!!
        {
            velocityZ -= Time.deltaTime * acceleration; // Accelerate backward.
        }
        if (forwardPressed && runPressed && velocityZ < currentMaxVelocity)  
        {
            velocityZ += Time.deltaTime * acceleration;
        }


        if (aimPressed && velocityX < maxAimValue)
        {
            velocityX += Time.deltaTime * aimAccelertion; // Accelerate forward.
        }

        if (aimPressed)  //---------
        {
            Debug.Log("AIMING");
        }


        if (!forwardPressed && velocityZ > 0.0f)
        {
            velocityZ -= Time.deltaTime * deceleration; // Decelerate forward.
        }
        if (forwardPressed && !runPressed && velocityZ > 0.3f)  /////
        {
            velocityZ -= Time.deltaTime * deceleration;
        }
        if (!backPressed && velocityZ < 0.0f)
        {
            velocityZ += Time.deltaTime * deceleration; // Decelerate backward.
        }


        if (!aimPressed && velocityX > 0.0f)
        {
            velocityX -= Time.deltaTime * aimDeceleration;
        }
    }

    private void LockOrResetVelocity(bool forwardPressed, bool runPressed, bool backPressed, float currentMaxVelocity, bool aimPressed)
    {
        if (aimPressed && velocityX > maxAimValue)
        {
            velocityX -= Time.deltaTime * aimDeceleration;
        }
        if (!aimPressed && velocityX > 0)
        {
            velocityX -= Time.deltaTime * aimDeceleration;
        }


        if (forwardPressed && runPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * deceleration;
        }
        // Lock or reset the Z-axis velocity based on input.
        if (!forwardPressed && velocityZ > 0.0f)
        {
            velocityZ -= Time.deltaTime * deceleration; // Decelerate forward.
        }
        else if (!backPressed && velocityZ < 0.0f /*&& aimPressed == false*/)
        {
            velocityZ += Time.deltaTime * deceleration; // Decelerate backward.
        }
        else if (backPressed && velocityZ > -0.2f/*&& aimPressed == false*/)  //---
        {
            velocityZ -= Time.deltaTime * deceleration; // Decelerate backward.
            if (velocityZ < -currentMaxVelocity)
            {
                velocityZ = -currentMaxVelocity; // Limit the backward velocity to the maximum.
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeAnimator();
    }



    // Update is called once per frame
    void Update()
    {
        bool forwardPressed = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A); // Check if the forward key is pressed.
        bool backPressed = Input.GetKey(KeyCode.S); // Check if the backward key is pressed.

        bool aimPressed = Input.GetKey(KeyCode.Mouse1); //Check is right mouse button is pressed
        //bool left_rightPressed = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A);
        bool runPressed = Input.GetKey(KeyCode.LeftShift) ? true : false; // Check if the run key is pressed.
        bool aiming = Input.GetKey(KeyCode.Mouse1) ? true : false;

        float currentMaxVelocity = runPressed && !aiming ? maxRunVelocity : maxWalkVelocity; // Determine the maximum velocity based on whether the run key is pressed or not
        UpdateVelocity(forwardPressed, runPressed, backPressed, currentMaxVelocity, aimPressed); // Update the current velocity based on input.
        LockOrResetVelocity(forwardPressed, runPressed, backPressed, currentMaxVelocity, aimPressed); // Lock or reset the velocity based on input.

        UpdateAnimator(); // Update the animator parameters based on the current velocity.
    }




    private void UpdateAnimator()
    {
        animator.SetFloat(velocityZHash, velocityZ); // Update the animator's "vertical" parameter with the Z-axis velocity.
        animator.SetFloat(velocityXHash, velocityX);
    }
}