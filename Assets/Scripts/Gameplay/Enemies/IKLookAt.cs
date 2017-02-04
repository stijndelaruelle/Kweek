﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKLookAt : MonoBehaviour
{
    [SerializeField]
    private UnityMethodsForwarder m_Forwarder;

    [SerializeField]
    private Animator m_Animator;

    [SerializeField]
    private Transform m_LookAtTarget;

    private void Start()
    {
        m_Forwarder.AnimatorIKEvent += OnAnimatorIK;
    }

    private void OnDestroy()
    {
        if (m_Forwarder != null)
            m_Forwarder.AnimatorIKEvent -= OnAnimatorIK;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (!m_Animator)
            return;

        //if the IK is active, set the position and rotation directly to the goal. 
        bool ikActive = true;

        if (ikActive)
        {

            // Set the look target position, if one has been assigned
            if (m_LookAtTarget != null)
            {
                m_Animator.SetLookAtWeight(1);
                m_Animator.SetLookAtPosition(m_LookAtTarget.position);

                m_Animator.SetIKRotation(AvatarIKGoal.LeftHand, m_LookAtTarget.rotation);
                m_Animator.SetIKRotation(AvatarIKGoal.RightHand, m_LookAtTarget.rotation);
            }

            // Set the right hand target position and rotation, if one has been assigned
            //if (rightHandObj != null)
            //{
            //    m_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            //    m_Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            //    m_Animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
            //    m_Animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
            //}
        }

        //if the IK is not active, set the position and rotation of the hand and head back to the original position
        else
        {
            m_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            m_Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            m_Animator.SetLookAtWeight(0);
        }
    }
}
