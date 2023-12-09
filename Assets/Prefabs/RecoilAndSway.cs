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
        if (Input.GetKey(KeyCode.Mouse1))
        {
            targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
        }
        else 
        {
            targetRotation += new Vector3(recoilX*3, Random.Range(-recoilY * 2, recoilY * 2), Random.Range(-recoilZ * 2, recoilZ * 2));           
        }
        
    }
}