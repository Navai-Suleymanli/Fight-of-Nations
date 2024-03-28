using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{

    [Networked] public NetworkButtons ButtonsPrevious { get; set; }  //----------------------------------------------------------------------------




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
    public bool pressedAtTheSameTime = false;
    public float speed = 0f;


    private NetworkCharacterController _cc;
    private bool _jumpPressed;

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
        weapon = GetComponent<WeaponController>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            _jumpPressed = true;
        }
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
        if (GetInput<NetworkInputData>(out var input) == false) return;

        // compute pressed/released state -------------------------------------------
        var pressed = input.buttons.GetPressed(ButtonsPrevious);
        var released = input.buttons.GetReleased(ButtonsPrevious);

        // store latest input as 'previous' state we had
        ButtonsPrevious = input.buttons;

        // movement (check for down)
        var move_vector = default(Vector3);
        //var jump_vector = default(Vector3);

        if (input.buttons.IsSet(MyButtons.Forward)) { move_vector.z += 1; }
        if (input.buttons.IsSet(MyButtons.Backward)) { move_vector.z -= 1; }

        if (input.buttons.IsSet(MyButtons.Left)) { move_vector.x -= 1; }
        if (input.buttons.IsSet(MyButtons.Right)) { move_vector.x += 1; }
        

        //jumping-----------------------
       if(input.buttons.IsSet(MyButtons.Jump)) { _cc.Jump(); }

        DoMove(move_vector);


        // Check for running input
        isRunning = Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W);

        // Check for aiming input
        isAiming = Input.GetKey(KeyCode.Mouse1);
        


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

        // Handle audio
        HandleAudio();
    }

    private void DoMove(Vector3 vector)  // -----------------------------------------------------------------------------------------------------------
    {
        _cc.Move(speed * vector * Runner.DeltaTime);
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

