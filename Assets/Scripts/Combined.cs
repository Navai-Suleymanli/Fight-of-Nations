using UnityEngine;

public class Combined : MonoBehaviour
{
    [SerializeField]
    public float sensitivity = 100f;
    public Camera cam;
    private float xRotation = 0f;
    private float yRotation = 0f;
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
        //camRotation = Vector3.zero;
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
            ResetCameraPositionAndRotation(); // Reset camera position and rotation
        }

        ApplyFinalRotations();
    }

    void HandleMouseInput()
    {
        Vector2 currentInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        smoothInput = Vector2.SmoothDamp(smoothInput, currentInput, ref inputVelocity, 0.08f);

        xRotation -= smoothInput.y * sensitivity;
        yRotation += smoothInput.x * sensitivity;

        xRotation = Mathf.Clamp(xRotation, -90f, 60f);
    }

    void ApplyRecoilEffect()
    {
        
        targetRotation += new Vector3(-recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
        

        currentRotation = Vector3.Lerp(currentRotation, targetRotation, Time.deltaTime * snappiness);

        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, Time.deltaTime * returnSpeed);
        
    }

    void ApplyFinalRotations()
    {
        cam.transform.localRotation = Quaternion.Euler(xRotation + currentRotation.x, yRotation + currentRotation.y, currentRotation.z);
        spotLight.transform.rotation = Quaternion.Slerp(spotLight.transform.rotation, cam.transform.rotation, 0.1f);
    }

    void ResetCameraPositionAndRotation()
    {
        currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, Time.deltaTime * returnSpeed);
        Quaternion originalRotation = Quaternion.Euler(originalCameraRotation);
        cam.transform.localRotation = Quaternion.Lerp(cam.transform.localRotation, originalRotation, Time.deltaTime * returnSpeed);
    }

    public void TriggerRecoil()
    {
        shouldApplyRecoil = true;
        targetRotation = new Vector3(-recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }

    public void Dayandir()
    {
        shouldApplyRecoil = false;
    }
}
