using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This script is used to move the Player's hands
 * to the same position as the weapon.
 * 
 * This script should be attached to the left and right hand 
 * in the IK Rig object
*/
public class SmoothGo : MonoBehaviour
{
    [SerializeField]
    private Transform _target; // Where the hands need to go
    [SerializeField]
    private float _speed;

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(this.transform.position, _target.transform.position, _speed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(_target.transform.rotation.eulerAngles);
    }
}