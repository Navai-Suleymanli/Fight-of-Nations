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

    WeaponController wc;
    [SerializeField]
    private Transform _target;
    [SerializeField]
    private Transform _target_makarov;
    [SerializeField]
    private Transform _target_sniper;
    // Where the hands need to go
    [SerializeField]
    private float _speed;

    void Start()
    {
        wc = gameObject.GetComponentInParent<WeaponController>();

    }

    private void FixedUpdate()
    {
        if (wc == null)
        {
            Debug.Log("NULLL");
        }
        if (wc.AKM)
        {
            transform.position = Vector3.Lerp(this.transform.position, _target.transform.position, _speed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(_target.transform.rotation.eulerAngles);
        }
        if (wc.Sniper)
        {
            transform.position = Vector3.Lerp(this.transform.position, _target_sniper.transform.position, _speed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(_target_sniper.transform.rotation.eulerAngles);
        }
        if (wc.isMakarov)
        {
            transform.position = Vector3.Lerp(this.transform.position, _target_makarov.transform.position, _speed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(_target_makarov.transform.rotation.eulerAngles);
        }
    }
       
}