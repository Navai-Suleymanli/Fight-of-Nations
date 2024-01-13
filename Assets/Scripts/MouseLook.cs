using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField]
    [Range(0, 2)]
    private float sensitivity = 100f;

    public Camera cam;
    float xRotation = 0f;

    private Vector2 smoothInput = Vector2.zero;
    private Vector2 currentInput = Vector2.zero;
    private Vector2 inputVelocity = Vector2.zero;


    //light //////

    public Light spotLight;
    //public GameObject gun;



    Vector3 followCam()
    {
        return spotLight.transform.position = gameObject.transform.position + new Vector3(0f, 1f, 0f);

    }

    /* Vector3 followCamGun()
     {
         return gun.transform.position = gameObject.transform.position + new Vector3(0f, 0.1f, 0.5f);
     }*/

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Smooth mouse input
        currentInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        smoothInput = Vector2.SmoothDamp(smoothInput, currentInput, ref inputVelocity, 0.08f);

        xRotation -= smoothInput.y * sensitivity;
        xRotation = Mathf.Clamp(xRotation, -90f, 60f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * smoothInput.x * sensitivity);

        followCam();
        // followCamGun();

        spotLight.transform.rotation = Quaternion.Slerp(spotLight.transform.rotation, cam.transform.rotation, 0.1f);
        //gun.transform.rotation = Quaternion.Slerp(gun.transform.rotation, cam.transform.rotation, 0.1f);
    }
}