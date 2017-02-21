using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void CollisionDelegate(Collision collision);
public delegate void TriggerDelegate(Collider collider);
public delegate void AnimatorIKDelegate(int layerIndex);

public class UnityMethodsForwarder : MonoBehaviour
{
    public event CollisionDelegate CollisionEnterEvent;
    public event CollisionDelegate CollisionStayEvent;
    public event CollisionDelegate CollisionExitEvent;

    public event TriggerDelegate TriggerEnterEvent;
    public event TriggerDelegate TriggerStayEvent;
    public event TriggerDelegate TriggerExitEvent;

    public event AnimatorIKDelegate AnimatorIKEvent;


    //More will follow if i end up using them
    private void OnCollisionEnter(Collision collision)
    {
        if (CollisionEnterEvent != null)
            CollisionEnterEvent(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (CollisionStayEvent != null)
            CollisionStayEvent(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (CollisionExitEvent != null)
            CollisionExitEvent(collision);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (TriggerEnterEvent != null)
            TriggerEnterEvent(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (TriggerStayEvent != null)
            TriggerStayEvent(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (TriggerExitEvent != null)
            TriggerExitEvent(other);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (AnimatorIKEvent != null)
            AnimatorIKEvent(layerIndex);
    }
}
