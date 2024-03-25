using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public Vector3 move;
    // CharacterController controller;
    [SerializeField] private float walkSpeed = 0f;
    [SerializeField] private float sprintSpeed = 0f;
    [SerializeField] private float walkSpeedSniper = 0f;
    [SerializeField] private float sprintSpeedSniper = 0f;
    [SerializeField] private float walkSpeedMakarov = 0f;
    [SerializeField] private float sprintSpeedMakarov = 0f;
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
    public float speed = 0f;

    //private float timer = 0.0f;

    // Start is called before the first frame update

    private NetworkCharacterController _cc;

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
        weapon = GetComponent<WeaponController>();
        audioSource = GetComponent<AudioSource>();
        //cameraTransform = GetComponentInChildren<Camera>().transform;
        midpoint = cameraTransform.localPosition.y;
    }

 

    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        if((Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)) || (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S)))
        {
            pressedAtTheSameTime = true;
        }
        else
        {
            pressedAtTheSameTime = false;
        }

        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
            _cc.Move(5 * data.direction * Runner.DeltaTime);
        }

        // Check ground status and reset vertical velocity
        /*if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = 0f;
        }*/

        // Check for running input
        isRunning = Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W);

        // Check for aiming input
        isAiming = Input.GetKey(KeyCode.Mouse1);

        // Calculate movement direction
        //float x = Input.GetAxis("Horizontal");
        //float z = Input.GetAxis("Vertical");
       // move = transform.right * x + transform.forward * z;

        


        //***Changing the speed of the player according to the weapon he is carrying***

        if (weapon.AKM)
        {
            speed = !isRunning && !pressedAtTheSameTime ? walkSpeed : (!isAiming && !weapon.isReloading && !pressedAtTheSameTime && !Input.GetKey(KeyCode.S)) ? sprintSpeed : !pressedAtTheSameTime ? walkSpeed : 0;
        }
        else if (weapon.Sniper)
        {
            speed = !isRunning && !pressedAtTheSameTime ? walkSpeedSniper : (!isAiming && !weapon.isReloading && !pressedAtTheSameTime && !Input.GetKey(KeyCode.S)) ? sprintSpeedSniper : !pressedAtTheSameTime ? walkSpeedSniper : 0;
        }
        else if (weapon.isMakarov) {
            speed = !isRunning && !pressedAtTheSameTime ? walkSpeedMakarov : (!isAiming && !weapon.isReloading && !pressedAtTheSameTime && !Input.GetKey(KeyCode.S)) ? sprintSpeedMakarov : !pressedAtTheSameTime ? walkSpeedMakarov : 0;

        }

        //controller.Move(move * speed * Time.deltaTime);


        

        // Apply gravity
        //verticalVelocity += gravityValue * Time.deltaTime;
        //controller.Move(new Vector3(0, verticalVelocity, 0) * Time.deltaTime);



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

