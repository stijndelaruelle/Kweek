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
    private float m_DrawSpeed;
    public float DrawSpeed
    {
        get { return m_DrawSpeed; }
    }

    [SerializeField]
    private float m_Speed;
    public float Speed
    {
        get { return m_Speed; }
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
    }

    private void Start()
    {
        SwitchState(SoldierState.Patrolling);

        m_Weapon.Setup(null);
        m_Weapon.UpdateAmmoEvent += OnUpdateWeaponAmmo;
        m_Forwarder.AnimatorIKEvent += OnAnimatorIK;
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
        m_Soldier.AnimatorIKEvent += OnAnimatorIK;
        m_Soldier.NavMeshAgent.Resume();
    }

    public void Exit()
    {
        if (m_Soldier == null)
            return;

        m_Soldier.TriggerStayEvent -= OnTriggerStay;
        m_Soldier.AnimatorIKEvent -= OnAnimatorIK;
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

        //Animation
        float normSpeed = (agent.velocity.magnitude / agent.speed) * m_Soldier.Speed;
        Vector3 normVelocity = agent.velocity.normalized * normSpeed;

        animator.SetFloat("VelocityX", 0.0f);
        animator.SetFloat("VelocityZ", 0.5f);
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

    private void OnAnimatorIK(int layedIndex)
    {
        m_Soldier.Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        m_Soldier.Animator.SetIKPosition(AvatarIKGoal.LeftHand, m_Soldier.WeaponGrip.position);
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
    private float m_DrawWeaponTimer = 0.0f;

    private bool m_IsSwitchingOut = false;

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

        m_Soldier.Animator.SetFloat("VelocityX", 0.0f);
        m_Soldier.Animator.SetFloat("VelocityZ", 0.0f);

        m_DrawWeaponTimer = m_Soldier.DrawSpeed;
        m_IsSwitchingOut = false;
    }

    public void Exit()
    {
        m_Target = null;

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

    private void HandleSwitchIn()
    {
        if (m_IsSwitchingOut)
            return;
            
        if (m_DrawWeaponTimer > 0.0f)
            m_DrawWeaponTimer -= Time.deltaTime;
    }

    private void HandleSwitchOut()
    {
        if (!m_IsSwitchingOut)
            return;

        m_DrawWeaponTimer += Time.deltaTime;
        if (m_DrawWeaponTimer >= m_Soldier.DrawSpeed)
        {
            m_Soldier.SwitchState(SoldierBehaviour.SoldierState.Patrolling);
        }
    }

    private void HandleLineOfSight()
    {
        if (m_Target == null)
            return;

        //Fire a ray in the direction of the target, if we don't hit him first.. we lost line of sight!
        Ray ray = new Ray(m_Soldier.Weapon.transform.position, (m_Target.bounds.center - m_Soldier.Weapon.transform.position));

        RaycastHit hitInfo;
        bool success = Physics.Raycast(ray, out hitInfo);

        bool switchOut = (!success || hitInfo.collider != m_Target);

        //if (switchOut)
        //    SwitchOut();
    }

    private void HandleShooting()
    {
        if (m_IsSwitchingOut || m_DrawWeaponTimer > 0.0f)
            return;

        Vector3 weaponPos = m_Soldier.WeaponPickup.transform.position;
        //Vector3 diff = m_Target.bounds.center - weaponPos;
        Ray fireRay = new Ray(weaponPos, m_Soldier.WeaponPickup.transform.forward);

        m_Soldier.Weapon.Fire(fireRay);
    }

    private void SwitchOut()
    {
        if (m_IsSwitchingOut)
            return;

        m_Soldier.Animator.SetTrigger("MovementTrigger");
        m_DrawWeaponTimer = 0.0f;

        m_IsSwitchingOut = true;
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

            bool switchOut = (degAngle > 170.0f);

            //if (switchOut)
            //    SwitchOut();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other == m_Target)
        {
            //Change to the chasing state
            //SwitchOut();
        }
    }

    public void SetTarget(Collider target)
    {
        m_Target = target;
    }

    private void OnAnimatorIK(int layedIndex)
    {
        float normTimer = (m_Soldier.DrawSpeed - m_DrawWeaponTimer) / m_Soldier.DrawSpeed;

        //Physically look at the target
        m_Soldier.Animator.SetLookAtWeight(normTimer * 2.0f);
        m_Soldier.Animator.SetLookAtPosition(m_Target.bounds.center);

        //Rotate the chest
        Vector3 direction = (m_Target.bounds.center - m_Soldier.Animator.GetBoneTransform(HumanBodyBones.Chest).position).normalized;
        Quaternion desiredRotation = Quaternion.LookRotation(direction);
        Vector3 euler = desiredRotation.eulerAngles;

        //Add the transform of the soldier, otherwise things get weird.
        euler.z = 360.0f - euler.x + (m_Soldier.transform.rotation.eulerAngles.x);
        euler.x = 360.0f - euler.y + (m_Soldier.transform.rotation.eulerAngles.y) - 45.0f; //-45 so the gun points towards you
        euler.y = 0.0f;

        desiredRotation = Quaternion.Euler(euler);

        //Quaternion currentRotation = Quaternion.Slerp(m_Soldier.Animator.GetBoneTransform(HumanBodyBones.Chest).localRotation, desiredRotation, normTimer);
        m_Soldier.Animator.SetBoneLocalRotation(HumanBodyBones.Chest, desiredRotation);
    }

    public override string ToString()
    {
        return "Firing";
    }
}