using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKController : MonoBehaviour
{
    public static IKController instance;

    Animator anim;

    [Header("Right Hand IK")]
    [Range(0, 1)] public float rightHandWeight;
    public Transform rightHandObj = null;
    public Transform rightHandHint = null;


    void Awake()
    {
        instance = this;
    }

    void Start()
    {

        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void OnAnimatorIK()
    {
        if (anim)
        {
            if (rightHandObj != null)
            {
                anim.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandWeight);
                anim.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHandWeight);
                anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                anim.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
            }
        }
    }
}
