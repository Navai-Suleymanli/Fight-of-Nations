using System.Security.Cryptography;
using UnityEngine;

public class Combined : MonoBehaviour
{
    [SerializeField]
    public float sensitivity = 100f;
    public Camera cam;
    private float xRotation = 0f;
    private Vector2 smoothInput = Vector2.zero;
    private Vector2 inputVelocity = Vector2.zero;

    public Light spotLight;
    public GameObject gun;

    private Vector3 currentRotation, targetRotation;
    private Vector3 initialGunPosition;
    private Vector3 originalCameraRotation; // To store the original rotation of the camera
    private bool shouldApplyRecoil = false;

    public float recoilX = 0.0f;
    public float recoilY = 0.0f;
    public float recoilZ = 0.0f;
    public float snappiness = 0.0f;
    public float returnSpeed = 0.0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        initialGunPosition = gun.transform.localPosition;
        originalCameraRotation = cam.transform.localRotation.eulerAngles; // Store the original camera rotation
    }

    void Update()
    {
        HandleMouseInput();

        if (shouldApplyRecoil)
        {
            ApplyRecoilEffect();
        }
        else if (!shouldApplyRecoil)
        {
            ResetCameraPositionAndRotation();
        }
        ApplyFinalRotations();
    }

    void HandleMouseInput()
    {
        Vector2 currentInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        smoothInput = Vector2.SmoothDamp(smoothInput, currentInput, ref inputVelocity, 0.08f);

        xRotation -= smoothInput.y * sensitivity;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply only pitch to the camera
        Vector3 cameraEuler = cam.transform.localEulerAngles;
        cameraEuler.x = xRotation;
        cam.transform.localEulerAngles = cameraEuler;

        // Rotate the player object for yaw
        transform.Rotate(Vector3.up * smoothInput.x * sensitivity);
    }

    void ApplyRecoilEffect()
    {
        // Check if right mouse button is pressed for aiming
        float aimMultiplier = Input.GetMouseButton(1) ? 0.5f : 1.0f; // Halves recoil when aiming

        // Calculate target rotation for recoil (only pitch)
        targetRotation.x += -recoilX * aimMultiplier;
        targetRotation.y = Random.Range(-recoilY, recoilY) * aimMultiplier;
        targetRotation.z = Random.Range(-recoilZ, recoilZ) * aimMultiplier;

        // Apply recoil to pitch while keeping current yaw and roll
        Vector3 cameraEuler = cam.transform.localEulerAngles;
        currentRotation = Vector3.Lerp(currentRotation, targetRotation, Time.deltaTime * snappiness);
        cameraEuler.x += currentRotation.x;
        cameraEuler.y += currentRotation.y;
        cameraEuler.z += currentRotation.z;
        cam.transform.localEulerAngles = cameraEuler;

        // Smoothly return to the original rotation
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, Time.deltaTime * returnSpeed);
        if (targetRotation.magnitude < 0.01f)
        {
            targetRotation = Vector3.zero;
        }

        if (currentRotation.magnitude < 0.01f)
        {
            currentRotation = Vector3.zero;
            shouldApplyRecoil = false;
        }
    }

    void ResetCameraPositionAndRotation()
    {
        // Smoothly interpolate the current rotation towards zero
        currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, Time.deltaTime * returnSpeed);

        // Use Slerp for smoother rotational interpolation
        Quaternion originalRotation = Quaternion.Euler(originalCameraRotation);
        cam.transform.localRotation = Quaternion.Slerp(cam.transform.localRotation, originalRotation, Time.deltaTime * returnSpeed);
        Debug.Log("RESETTING!!!");
    }

    void ApplyFinalRotations()
    {
        cam.transform.localRotation = Quaternion.Euler(xRotation + currentRotation.x, originalCameraRotation.y, currentRotation.z);
        spotLight.transform.rotation = Quaternion.Slerp(spotLight.transform.rotation, cam.transform.rotation, 0.1f);
    }

    public void TriggerRecoil()
    {
        Debug.Log("Recoil Triggered");
        shouldApplyRecoil = true;
        float aimMultiplier = Input.GetMouseButton(1) ? 0.5f : 1.0f; // Halves recoil when aiming
        targetRotation = new Vector3(-recoilX * aimMultiplier, Random.Range(-recoilY, recoilY) * aimMultiplier, Random.Range(-recoilZ, recoilZ) * aimMultiplier);
    }

    public void Dayandir()
    {
        Debug.Log("Recoil Stopped");
        shouldApplyRecoil = false;
    }
}
