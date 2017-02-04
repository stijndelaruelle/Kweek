using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SoldierBehaviour : MonoBehaviour
{
    public enum SoldierState
    {
        Patrolling,
        Firing,
        Chasing
    }

    [SerializeField]
    private Transform m_FirePosition;
    public Transform FirePosition
    {
        get { return m_FirePosition; }
    }

    [SerializeField]
    private float m_FireDelay;
    public float FireDelay
    {
        get { return m_FireDelay; }
    }

    [SerializeField]
    private float m_WalkSpeed;
    public float WalkSpeed
    {
        get { return m_WalkSpeed; }
    }

    [SerializeField]
    private float m_RunSpeed;
    public float RunSpeed
    {
        get { return m_RunSpeed; }
    }

    [SerializeField]
    private float m_BodyRotationSpeed;
    public float BodyRotationSpeed
    {
        get { return m_BodyRotationSpeed; }
    }

    [SerializeField]
    private Transform m_TargetTransform;

    [SerializeField]
    private float m_ViewAngle;
    public float ViewAngle
    {
        get { return m_ViewAngle; }
    }

    [Space(10)]
    [Header("Required references")]
    [Space(5)]
    [SerializeField]
    private Weapon m_Weapon;
    public Weapon Weapon
    {
        get { return m_Weapon; }
    }

    [SerializeField]
    private WeaponPickup m_WeaponPickup;
    public WeaponPickup WeaponPickup
    {
        get { return m_WeaponPickup; }
    }

    [SerializeField]
    private Transform m_WeaponGrip;
    public Transform WeaponGrip
    {
        get { return m_WeaponGrip; }
    }

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
    private List<IState> m_States;

    private float m_LastSpeed; //Sometines the velocity can spike, if so reset to this value

    //Events
    public event TriggerDelegate TriggerEnterEvent;
    public event TriggerDelegate TriggerStayEvent;
    public event TriggerDelegate TriggerExitEvent;

    public event AnimatorIKDelegate AnimatorIKEvent;

    private void Awake()
    {
        m_States = new List<IState>();

        Vector3 target = transform.position;
        if (m_TargetTransform != null) { target = m_TargetTransform.position; }

        m_States.Add(new PatrolState(this, transform.position, target));
        m_States.Add(new FireState(this));
        m_States.Add(new ChaseState(this, Vector3.zero));
    }

    private void Start()
    {
        m_Weapon.UpdateAmmoEvent += OnUpdateWeaponAmmo;
        m_Forwarder.AnimatorIKEvent += OnAnimatorIK;

        SwitchState(SoldierState.Patrolling);
    }

    public void Setup(List<Collider> ownerColliders)
    {
        m_Weapon.Setup(ownerColliders, null);
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
            m_CurrentState.Update();

        HandleMovementAnimation();
    }

    private void HandleMovementAnimation()
    {
        //Walking animation
        float currentSpeed = m_NavMeshAgent.velocity.magnitude;

        //Every once in a while the velocity will spike
        if (currentSpeed > m_NavMeshAgent.speed) { currentSpeed = m_LastSpeed; }
        m_LastSpeed = currentSpeed;

        m_Animator.SetFloat("VelocityX", 0.0f);
        m_Animator.SetFloat("VelocityZ", currentSpeed);
    }

    public void Pause()
    {
        m_NavMeshAgent.Stop();
    }

    public void Resume()
    {
        m_NavMeshAgent.Resume();
    }

    private void OnUpdateWeaponAmmo(int ammoInClip, int reserveAmmo)
    {
        m_WeaponPickup.Ammo = ammoInClip;
    }

    public IState SwitchState(SoldierState newState)
    {
        int stateID = (int)newState;
        if (stateID >= 0 && stateID < m_States.Count)
        {
            if (m_CurrentState != null)
                m_CurrentState.Exit();

            m_CurrentState = m_States[stateID];
            m_CurrentState.Enter();
        }
        else
        {
            Debug.LogError("INVALID STATE!");
        }

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
        if (layerIndex == 1)
        {
            m_Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            m_Animator.SetIKPosition(AvatarIKGoal.LeftHand, m_WeaponGrip.position);
        }
    }
}

public class PatrolState : IState
{
    private SoldierBehaviour m_Soldier;

    private Vector3 m_StartPosition;
    private Vector3 m_TargetPosition;

    public PatrolState(SoldierBehaviour soldier, Vector3 startPosition, Vector3 targetPosition)
    {
        m_Soldier = soldier;
        m_StartPosition = startPosition;
        m_TargetPosition = targetPosition;
    }

    public void Enter()
    {
        Debug.Log("Entered patrolling state!");

        if (m_StartPosition != m_TargetPosition)
        {
            m_Soldier.NavMeshAgent.destination = m_TargetPosition;
        }

        m_Soldier.TriggerStayEvent += OnTriggerStay;
        m_Soldier.NavMeshAgent.Resume();
        m_Soldier.NavMeshAgent.speed = m_Soldier.WalkSpeed;
        m_Soldier.Animator.SetTrigger("MovementTrigger");
    }

    public void Exit()
    {
        if (m_Soldier == null)
            return;

        m_Soldier.TriggerStayEvent -= OnTriggerStay;
    }

    public void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (m_StartPosition == m_TargetPosition)
            return;

        NavMeshAgent agent = m_Soldier.NavMeshAgent;
        Animator animator = m_Soldier.Animator;

        //Check if we reached our destination
        if (agent.remainingDistance <= 0.5f)
        {
            Vector3 temp = m_TargetPosition.Copy();
            m_TargetPosition = m_StartPosition;
            m_StartPosition = temp;

            agent.destination = m_TargetPosition;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //Check if it's the player
        if (other.tag == "Player")
        {
            //If so check if he's within the specified angle
            Vector3 diffPos = other.transform.position - m_Soldier.transform.position;
            float dot = Vector3.Dot(m_Soldier.transform.forward, diffPos.normalized);
            float degAngle = (Mathf.Acos(dot) * Mathf.Rad2Deg * 2.0f);

            if (degAngle <= m_Soldier.ViewAngle)
            {
                Vector3 middleTop = other.bounds.center;
                middleTop.y += other.bounds.extents.y * 0.5f;

                Ray ray = new Ray(m_Soldier.Weapon.transform.position, (middleTop - m_Soldier.Weapon.transform.position));

                RaycastHit hitInfo;
                bool success = Physics.Raycast(ray, out hitInfo);

                if (success && hitInfo.collider == other)
                {
                    //Change to the firing state
                    FireState fireState = (FireState)m_Soldier.SwitchState(SoldierBehaviour.SoldierState.Firing);
                    fireState.SetTarget(other);
                }
            }
        }
    }

    public override string ToString()
    {
        return "Patrolling";
    }
}

public class FireState : IState
{
    private SoldierBehaviour m_Soldier;
    private Collider m_Target;
    private float m_FireDelayTimer = 0.0f;
    private bool m_IsInFireStance = false;

    private Coroutine m_BurstFireRoutine;

    private bool m_IsSwitchingOut = false;

    private Vector3 m_LastHeadLookAtPosition;
    private Quaternion m_LastChestLocalRotation; //Chest bone rotation constatntly resets, cache it here.

    public FireState(SoldierBehaviour soldier)
    {
        m_Soldier = soldier;
    }

    public void Enter()
    {
        Debug.Log("Entered fire state!");

        m_Soldier.TriggerStayEvent += OnTriggerStay;
        m_Soldier.TriggerExitEvent += OnTriggerExit;
        m_Soldier.AnimatorIKEvent += OnAnimatorIK;

        //Stop the character from moving, both gamewise as visually
        m_Soldier.NavMeshAgent.Stop();
        m_Soldier.NavMeshAgent.updateRotation = false;
        m_Soldier.NavMeshAgent.speed = 0.0f;

        m_Target = null;
        m_FireDelayTimer = m_Soldier.FireDelay;
        m_IsInFireStance = false;
        m_IsSwitchingOut = false;

        m_LastHeadLookAtPosition = Vector3.zero;
        m_LastChestLocalRotation = Quaternion.identity;
    }

    public void Exit()
    {
        if (m_Soldier == null)
            return;

        m_Soldier.TriggerStayEvent -= OnTriggerStay;
        m_Soldier.TriggerExitEvent -= OnTriggerExit;
        m_Soldier.AnimatorIKEvent -= OnAnimatorIK;

        m_Soldier.NavMeshAgent.updateRotation = true;
        //m_Soldier.Animator.SetLookAtWeight(0);
    }

    public void Update()
    {
        HandleSwitchIn();
        HandleSwitchOut();

        HandleLineOfSight();
        HandleShooting();
    }

    //Shooting
    private void HandleShooting()
    {
        if (m_IsSwitchingOut || m_FireDelayTimer > 0.0f)
            return;

        if (m_BurstFireRoutine == null)
        {
            m_BurstFireRoutine = m_Soldier.StartCoroutine(FireBurstRoutine());
        }
    }

    private IEnumerator FireBurstRoutine()
    {
        int bulletsFired = 0;

        while (bulletsFired < 3)
        {
            bool success = ShootOnce();
            if (success) { bulletsFired++; }

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1.0f);
        m_BurstFireRoutine = null;
    }

    private bool ShootOnce()
    {
        Vector3 weaponPos = m_Soldier.FirePosition.position;
        Ray fireRay = new Ray(weaponPos, m_Soldier.FirePosition.forward);

        return m_Soldier.Weapon.Fire(fireRay);
    }


    //Switching
    private void HandleSwitchIn()
    {
        if (m_IsSwitchingOut)
            return;

        if (m_FireDelayTimer > 0.0f)
        {
            m_FireDelayTimer -= Time.deltaTime;

            //If we are almost ready to fire, call the animation already once. Otherwise we fire our first bullet into the ground
            if (m_IsInFireStance == false && m_FireDelayTimer <= 0.2f) //hardcoded value from the animator
            {
                m_Soldier.Animator.SetTrigger("ReadyFireTrigger");
                m_IsInFireStance = true;
            }
        }
    }

    private void HandleSwitchOut()
    {
        if (!m_IsSwitchingOut)
            return;

        m_FireDelayTimer += Time.deltaTime;
        if (m_FireDelayTimer >= m_Soldier.FireDelay)
        {
            ChaseState changeState = (ChaseState)m_Soldier.SwitchState(SoldierBehaviour.SoldierState.Chasing);
            changeState.SetTarget(m_Target.transform.position.Copy());
        }
    }

    private void SwitchOut()
    {
        if (m_IsSwitchingOut)
            return;

        m_FireDelayTimer = 0.0f;
        m_IsSwitchingOut = true;
        m_Soldier.Animator.SetTrigger("MovementTrigger");
    }


    private void HandleLineOfSight()
    {
        if (m_IsSwitchingOut)
            return;

        //Fire a ray in the direction of the target, if we don't hit him first.. we lost line of sight!
        Vector3 middleTop = m_Target.bounds.center;
        middleTop.y += m_Target.bounds.extents.y * 0.5f;

        Ray ray = new Ray(m_Soldier.Weapon.transform.position, (middleTop - m_Soldier.Weapon.transform.position));

        RaycastHit hitInfo;
        bool success = Physics.Raycast(ray, out hitInfo);

        if (!(success && hitInfo.collider == m_Target))
        {
            SwitchOut();
        }
    }


    private void OnTriggerStay(Collider other)
    {
        //Check if it's the player
        if (other.tag == "Player")
        {
            //If so check if he's within the specified angle
            Vector3 diffPos = other.transform.position - m_Soldier.transform.position;
            float dot = Vector3.Dot(m_Soldier.transform.forward, diffPos.normalized);
            float degAngle = (Mathf.Acos(dot) * Mathf.Rad2Deg * 2.0f);

            bool switchOut = (degAngle > (m_Soldier.ViewAngle + 5.0f)); //a little offset to avoid standing right on the edge

            if (switchOut)
                SwitchOut();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other == m_Target)
        {
            //Change to the chasing state
            SwitchOut();
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        //Rotate the head
        float normTimer = (m_Soldier.FireDelay - m_FireDelayTimer) / m_Soldier.FireDelay;
        m_Soldier.Animator.SetLookAtWeight(normTimer);
        m_Soldier.Animator.SetLookAtPosition(m_Target.bounds.center);

        //Rotate the chest
        Quaternion desiredChestRotation = CalculateLocalBoneRotation(HumanBodyBones.Chest);

        if (m_LastChestLocalRotation == Quaternion.identity)
        {
            m_LastChestLocalRotation = Quaternion.RotateTowards(m_Soldier.Animator.GetBoneTransform(HumanBodyBones.Chest).localRotation,
                                                                desiredChestRotation,
                                                                m_Soldier.BodyRotationSpeed * Time.deltaTime);
        }
        else
        {
            m_LastChestLocalRotation = Quaternion.RotateTowards(m_LastChestLocalRotation,
                                                                desiredChestRotation,
                                                                m_Soldier.BodyRotationSpeed * Time.deltaTime);
        }

        m_Soldier.Animator.SetBoneLocalRotation(HumanBodyBones.Chest, m_LastChestLocalRotation);
    }

    private Quaternion CalculateLocalBoneRotation(HumanBodyBones boneType)
    {
        Quaternion desiredRotation;

        if (m_IsSwitchingOut == false)
        {
            Vector3 direction = (m_Target.bounds.center - m_Soldier.Animator.GetBoneTransform(boneType).position).normalized;
            desiredRotation = Quaternion.LookRotation(direction);
            Vector3 euler = desiredRotation.eulerAngles;

            //Add the transform of the soldier, otherwise things get weird.
            euler.z = 360.0f - euler.x + (m_Soldier.transform.rotation.eulerAngles.x);
            euler.x = 360.0f - euler.y + (m_Soldier.transform.rotation.eulerAngles.y) - 40.0f; //-45 so the gun points towards you
            euler.y = 0.0f;

            desiredRotation = Quaternion.Euler(euler);
        }
        else
        {
            desiredRotation = m_Soldier.Animator.GetBoneTransform(boneType).localRotation;
        }

        return desiredRotation;
    }

    public void SetTarget(Collider target)
    {
        m_Target = target;
    }

    public override string ToString()
    {
        return "Firing";
    }
}

public class ChaseState : IState
{
    private SoldierBehaviour m_Soldier;
    private Vector3 m_TargetPosition;

    public ChaseState(SoldierBehaviour soldier, Vector3 targetPosition)
    {
        m_Soldier = soldier;
        m_TargetPosition = targetPosition;
    }

    public void Enter()
    {
        Debug.Log("Entered Chase state!");
        m_Soldier.NavMeshAgent.destination = m_TargetPosition;

        m_Soldier.TriggerStayEvent += OnTriggerStay;
        m_Soldier.NavMeshAgent.Resume();
        m_Soldier.NavMeshAgent.speed = m_Soldier.RunSpeed;

        m_Soldier.Animator.SetTrigger("MovementTrigger");
    }

    public void Exit()
    {
        if (m_Soldier == null)
            return;

        m_Soldier.TriggerStayEvent -= OnTriggerStay;
    }

    public void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        NavMeshAgent agent = m_Soldier.NavMeshAgent;
        Animator animator = m_Soldier.Animator;

        //Check if we reached our destination
        if (agent.remainingDistance <= 0.5f)
        {
            //If we still didn't switch to the fire state at this point, we lost the player. Go back to patrolling
            m_Soldier.SwitchState(SoldierBehaviour.SoldierState.Patrolling);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //Check if it's the player
        if (other.tag == "Player")
        {
            //If so check if he's within the specified angle
            Vector3 diffPos = other.transform.position - m_Soldier.transform.position;
            float dot = Vector3.Dot(m_Soldier.transform.forward, diffPos.normalized);
            float degAngle = (Mathf.Acos(dot) * Mathf.Rad2Deg * 2.0f);

            if (degAngle <= m_Soldier.ViewAngle)
            {
                //Check line of sight
                Ray ray = new Ray(m_Soldier.Weapon.transform.position, (other.bounds.center - m_Soldier.Weapon.transform.position));

                RaycastHit hitInfo;
                bool success = Physics.Raycast(ray, out hitInfo);

                if (success && hitInfo.collider == other)
                {
                    //Change to the firing state
                    FireState fireState = (FireState)m_Soldier.SwitchState(SoldierBehaviour.SoldierState.Firing);
                    fireState.SetTarget(other);
                }
            }
        }
    }

    public void SetTarget(Vector3 targetPosition)
    {
        m_TargetPosition = targetPosition;
        m_Soldier.NavMeshAgent.destination = m_TargetPosition;
    }

    public override string ToString()
    {
        return "Chasing";
    }
}