using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoldierBehaviour : MonoBehaviour
{
    [SerializeField]
    private IAbstractState m_InitialState;

    [Space(10)]
    [Header("Weapon")]
    [Space(5)]
    [SerializeField]
    private Weapon m_Weapon;
    public Weapon Weapon
    {
        get { return m_Weapon; }
    }

    [SerializeField]
    private WeaponPickup m_WeaponPickup;

    [SerializeField]
    private Transform m_FrontWeaponGrip;

    [SerializeField]
    private Transform m_BackWeaponGrip;

    [Space(10)]
    [Header("Required references")]
    [Space(5)]
    [SerializeField]
    private Animator m_Animator;
    public Animator Animator
    {
        get { return m_Animator; }
    }

    [SerializeField]
    private NavMeshAgent m_NavMeshAgent;
    public NavMeshAgent NavMeshAgent
    {
        get { return m_NavMeshAgent; }
    }

    [SerializeField]
    private UnityMethodsForwarder m_Forwarder;

    private IState m_CurrentState;

    private float m_LastSpeed; //Sometines the velocity can spike, if so reset to this value

    //Events
    public event TriggerDelegate TriggerEnterEvent;
    public event TriggerDelegate TriggerStayEvent;
    public event TriggerDelegate TriggerExitEvent;
    public event AnimatorIKDelegate AnimatorIKEvent;


    private void Start()
    {
        m_Weapon.UpdateAmmoEvent += OnUpdateWeaponAmmo;
        m_Forwarder.AnimatorIKEvent += OnAnimatorIK;

        SwitchState(m_InitialState);
    }

    public void Setup(List<Collider> ownerColliders)
    {
        m_Weapon.Setup(ownerColliders, null);

        m_WeaponPickup.enabled = false;
        m_WeaponPickup.IgnoreColliders(ownerColliders); //Make sure we don't freak out when becomming a ragdoll
    }

    private void OnDestroy()
    {
        if (m_Weapon != null)
            m_Weapon.UpdateAmmoEvent -= OnUpdateWeaponAmmo;

        if (m_Forwarder != null)
            m_Forwarder.AnimatorIKEvent -= OnAnimatorIK;
    }

    private void Update()
    {
        if (m_CurrentState != null)
            m_CurrentState.StateUpdate();

        HandleMovementAnimation();
    }

    private void HandleMovementAnimation()
    {
        //Walking animation
        float currentSpeed = m_NavMeshAgent.velocity.magnitude;

        //Every once in a while the velocity will spike (fix this)
        if (currentSpeed > m_NavMeshAgent.speed) { currentSpeed = m_LastSpeed; }
        m_LastSpeed = currentSpeed;

        m_Animator.SetFloat("VelocityX", 0.0f);
        m_Animator.SetFloat("VelocityZ", currentSpeed);
    }



    private void OnUpdateWeaponAmmo(int ammoInClip, int reserveAmmo)
    {
        m_WeaponPickup.Ammo = ammoInClip;
    }

    public IState SwitchState(IState newState)
    {
        if (m_CurrentState == newState)
        {
            Debug.LogWarning("State is trying to switch to itself, something is wrong.");
            return m_CurrentState;
        }

        if (m_CurrentState != null)
            m_CurrentState.Exit();

        m_CurrentState = newState;
        m_CurrentState.Enter();

        return m_CurrentState;
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

        //Set the left hand to the weapongrip (layer 1 so we get new positions!)
        //if (layerIndex == 1)
        //{
        //    m_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        //    m_Animator.SetIKPosition(AvatarIKGoal.RightHand, m_FrontWeaponGrip.position);

        //    m_Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
        //    m_Animator.SetIKRotation(AvatarIKGoal.RightHand, m_FrontWeaponGrip.rotation);
        //}

        if (layerIndex == 2)
        {
            m_Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            m_Animator.SetIKPosition(AvatarIKGoal.LeftHand, m_BackWeaponGrip.position);

            m_Animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            m_Animator.SetIKRotation(AvatarIKGoal.LeftHand, m_BackWeaponGrip.rotation);
        }
    }

    public void OnDeath()
    {
        m_WeaponPickup.enabled = true;
        m_WeaponPickup.gameObject.transform.parent = null;
        m_WeaponPickup.Drop(transform.forward.Copy() * 500.0f, null);
    }
}
