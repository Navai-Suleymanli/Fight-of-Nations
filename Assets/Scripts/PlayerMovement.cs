using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Vector3 move;
    public CharacterController controller;
    [SerializeField] private float walkSpeed = 0f;
    [SerializeField] private float sprintSpeed = 0f;
    [SerializeField] private bool isRunning = false;
    [SerializeField] private bool isAiming = false;
    WeaponController weapon;

    private AudioSource audioSource;
    [SerializeField] AudioClip playerBreath;
    [SerializeField] AudioClip playerRunningBreath;
    private bool isAudioPlaying = false;

    private float gravityValue = -9.81f; // Gravity value
    private float verticalVelocity = 0f; // Vertical velocity due to gravity

    // Camera bobbing effect variables
    private Transform cameraTransform;
    public float bobbingSpeed = 1f;
    public float bobbingAmount = 2f;
    public float midpoint = 2.0f;
    public bool pressedAtTheSameTime = false;
    //private float timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        weapon = GetComponent<WeaponController>();
        audioSource = GetComponent<AudioSource>();
        cameraTransform = GetComponentInChildren<Camera>().transform;
        midpoint = cameraTransform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        if((Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)) || (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S)))
        {
            pressedAtTheSameTime = true;
        }
        else
        {
            pressedAtTheSameTime = false;
        }
        // Check ground status and reset vertical velocity
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = 0f;
        }

        // Check for running input
        isRunning = Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W);

        // Check for aiming input
        isAiming = Input.GetKey(KeyCode.Mouse1);

        // Calculate movement direction
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        move = transform.right * x + transform.forward * z;

        // Handle player movement
        if (!isRunning && !pressedAtTheSameTime)
        {
            controller.Move(move * walkSpeed * Time.deltaTime);
            
        }
        else if (!isAiming && !weapon.isReloading && !Input.GetKey(KeyCode.S) && !pressedAtTheSameTime)
        {
            controller.Move(move * sprintSpeed * Time.deltaTime);
            
        }
        else
        {
            if(!pressedAtTheSameTime)
            {
                controller.Move(move * walkSpeed * Time.deltaTime);

            }

        }

        // Apply gravity
        verticalVelocity += gravityValue * Time.deltaTime;
        controller.Move(new Vector3(0, verticalVelocity, 0) * Time.deltaTime);



        // Handle audio
        HandleAudio();
    }

    void HandleAudio()
    {
        if (isRunning && !isAiming && Input.GetKey(KeyCode.W))  //-------------------------------------------------
        {
            if (!isAudioPlaying || audioSource.clip != playerRunningBreath)
            {
                audioSource.clip = playerRunningBreath;
                audioSource.loop = true;
                audioSource.Play();
                isAudioPlaying = true;
            }
        }
        else if (!isRunning)
        {
            if (!isAudioPlaying || audioSource.clip != playerBreath)
            {
                audioSource.clip = playerBreath;
                audioSource.loop = true;
                audioSource.volume = 0.02f;
                audioSource.Play();
                isAudioPlaying = true;
            }
        }
        else if (isRunning && isAiming)
        {

            if (!isAudioPlaying || audioSource.clip != playerBreath)
            {
                audioSource.clip = playerBreath;
                audioSource.loop = true;
                audioSource.volume = 0.02f;
                audioSource.Play();
                isAudioPlaying = true;
            }
        }
        else
        {
            // Stop the audio if none of the conditions are met
            if (isAudioPlaying)
            {
                audioSource.Stop();
                isAudioPlaying = false;
            }
        }
    }
}

