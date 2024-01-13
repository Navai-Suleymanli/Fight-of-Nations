using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilAndSway : MonoBehaviour
{
    private Vector3 currentRotation;
    private Vector3 targetRotation;

    [Header("Recoil Settings")]
    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;
    [SerializeField] private float snappiness;
    [SerializeField] private float returnSpeed;

    [Header("Recoil Increase Settings")]
    [SerializeField] private float recoilIncreaseFactor = 1.1f; // Factor by which recoil increases
    [SerializeField] private float maxRecoilMultiplier = 3f; // Maximum recoil multiplier
    private float recoilMultiplier = 1f; // Current recoil multiplier

    [Header("Sway Settings")]
    [SerializeField] private float smooth;
    [SerializeField] private float multiplier;

    public GameObject gun;

    void Update()
    {
        // Recoil
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.deltaTime);

        // Sway
        float mouseX = Input.GetAxisRaw("Mouse X") * multiplier;
        float mouseY = Input.GetAxisRaw("Mouse Y") * multiplier;

        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion swayRotation = rotationX * rotationY;

        // Combine rotations
        Quaternion combinedRotation = Quaternion.Euler(currentRotation) * swayRotation;

        gun.transform.localRotation = Quaternion.Slerp(gun.transform.localRotation, combinedRotation, smooth * Time.deltaTime);
    }

    public void recoilFire()
    {
        float recoilFactorX = recoilX * recoilMultiplier;
        float recoilFactorY = recoilY * recoilMultiplier;
        float recoilFactorZ = recoilZ * recoilMultiplier;

        if (Input.GetKey(KeyCode.Mouse1)) // Aiming
        {
            targetRotation += new Vector3(recoilFactorX, Random.Range(-recoilFactorY, recoilFactorY), Random.Range(-recoilFactorZ, recoilFactorZ));
        }
        else // Not aiming
        {
            targetRotation += new Vector3(recoilFactorX * 3, Random.Range(-recoilFactorY * 2, recoilFactorY * 2), Random.Range(-recoilFactorZ * 2, recoilFactorZ * 2));
        }

        // Increase the recoil over time
        recoilMultiplier *= recoilIncreaseFactor;
        recoilMultiplier = Mathf.Min(recoilMultiplier, maxRecoilMultiplier);

        // Reset recoilMultiplier if not shooting
        if (!Input.GetKey(KeyCode.Mouse0))
        {
            recoilMultiplier = 1f;
        }
    }
}
