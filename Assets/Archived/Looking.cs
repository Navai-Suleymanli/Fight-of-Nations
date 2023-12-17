using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Looking : MonoBehaviour
{
    [SerializeField]
    [Range(0, 2)]
    private float sensitivity = 1f;

    [SerializeField]
    private Camera cam;
    float xRotation = 0f;

    private Vector2 smoothInput = Vector2.zero;
    private Vector2 currentInput = Vector2.zero;
    private Vector2 inputVelocity = Vector2.zero;

    // spot light
    public Light spotLight;


    Vector3 followCam ()
    {
        return spotLight.transform.position = gameObject.transform.position + new Vector3(0f,1f,0f);
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        currentInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        smoothInput = Vector2.SmoothDamp(smoothInput, currentInput, ref inputVelocity, 0.1f);

        xRotation -= smoothInput.y * sensitivity;
        xRotation = Mathf.Clamp(xRotation, -90f, 60f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * smoothInput.x * sensitivity);

        followCam();
        spotLight.transform.rotation = Quaternion.Slerp(spotLight.transform.rotation, cam.transform.rotation, 0.1f);
    }
}
