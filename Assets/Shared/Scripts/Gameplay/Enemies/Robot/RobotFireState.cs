using System.Collections;
using UnityEngine;

namespace Kweek
{
    [RequireComponent(typeof(EnemyWeaponPickupBehaviour))]
    public class RobotFireState : IAbstractTargetState
    {
        private EnemyWeaponPickupBehaviour m_Behaviour = null;

        [Header("Firing")]
        [Space(5)]
        [SerializeField]
        private Transform m_FireTransform = null;

        [SerializeField]
        private float m_FireDelay = 0.0f;
        private float m_FireDelayTimer = 0.0f;

        [SerializeField]
        private float m_MaxShootRange = 5.0f;

        [SerializeField]
        private float m_ChargeTime = 1.0f;

        [Space(10)]
        [Header("Chest & Hand rotation")]
        [Space(5)]

        [Tooltip("Degrees per second")]
        [SerializeField]
        private float m_ChestRotationSpeed = 0.0f;

        [SerializeField]
        private float m_HorizontalChestRotationOffset = 0.0f;

        [SerializeField]
        private float m_VerticalChestRotationOffset = 0.0f;

        [Space(10)]
        [Header("Scanning")]
        [Space(5)]
        [SerializeField]
        private Transform m_ViewTransform = null;

        [SerializeField]
        private float m_ViewRadius = 0.0f;

        [SerializeField]
        private float m_ViewAngle = 0.0f;

        [SerializeField]
        private LayerMask m_ScanLayerMask = default(LayerMask);

        [SerializeField]
        private IAbstractState m_PatrolState = null;

        [SerializeField]
        private IAbstractTargetState m_ChaseState = null;

        private bool m_IsInFireStance = false;
        private bool m_IsSwitchingOut = false;

        private IDamageableObject m_Target = null;
        private Coroutine m_ChargeRoutine = null;
        private Quaternion m_LastChestLocalRotation = Quaternion.identity; //Chest bone rotation constatntly resets, cache it here.
        private Quaternion m_LastRightArmLocalRotation = Quaternion.identity;

        private void Awake()
        {
            //Assigning this manually clutters the inspector a LOT!
            //If we, at some point, want to detach state objects from their behaviour, revert this.
            m_Behaviour = gameObject.GetComponent<EnemyWeaponPickupBehaviour>();
        }

        public override void Enter()
        {
            //Debug.Log("Entered fire state!");
            if (m_Behaviour != null)
            {
                m_Behaviour.AnimatorIKEvent += OnStateAnimatorIK;

                //Stop the character from moving, both gamewise as visually
                if (m_Behaviour.NavMeshAgent != null)
                    m_Behaviour.NavMeshAgent.isStopped = true;
            }

            m_Target = null;
            m_FireDelayTimer = m_FireDelay;
            m_IsInFireStance = false;
            m_IsSwitchingOut = false;

            m_LastChestLocalRotation = Quaternion.identity;
        }

        public override void Exit()
        {
            if (m_Behaviour != null)
                m_Behaviour.AnimatorIKEvent -= OnStateAnimatorIK;
        }

        public override void StateUpdate()
        {
            HandleSwitchIn();
            HandleSwitchOut();

            HandleShooting();
            HandleScanning();
        }

        //Shooting
        private void HandleShooting()
        {
            if (m_IsSwitchingOut == true || m_FireDelayTimer > 0.0f)
                return;

            if (m_ChargeRoutine != null || m_FireTransform == null)
                return;

            //Check the distance between us and the target
            Vector3 diff = m_Target.GetPosition() - m_FireTransform.position;
            float distance = diff.magnitude;

            //If we are too far away, chase to the position!
            if (distance > m_MaxShootRange)
            {
                SwitchOut();
            }

            m_ChargeRoutine = StartCoroutine(ChargeRoutine(m_ChargeTime));
        }

        private IEnumerator ChargeRoutine(float chargeTime)
        {
            float timer = 0.0f;
            while (timer < chargeTime)
            {
                ChargeWeapon();
                timer += Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            //Shouldn't be endless, only reason this would fire more than once is if we try to fire before the minimum chargetime has been reached
            bool hasFired = false;
            while (hasFired == false)
            {
                hasFired = FireWeapon();
                yield return new WaitForEndOfFrame();
            }

            m_ChargeRoutine = null;
        }

        private void ChargeWeapon()
        {
            if (m_FireTransform == null)
                return;

            Ray fireRay = new Ray(m_FireTransform.position, m_FireTransform.forward);
            m_Behaviour.Weapon.Use(fireRay);
        }

        private bool FireWeapon()
        {
            if (m_Target == null || m_Target.IsDead())
            {
                SwitchOut();
                return false;
            }

            if (m_FireTransform == null || m_Behaviour.Weapon)
                return false;

            Vector3 dir = (m_Target.GetPosition() - m_FireTransform.position).normalized;
            Ray fireRay = new Ray(m_FireTransform.position, dir);

            return m_Behaviour.Weapon.StopUse(fireRay);
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
                    m_Behaviour.Animator.SetTrigger("ReadyFireTrigger");
                    m_IsInFireStance = true;
                }
            }
        }

        private void HandleSwitchOut()
        {
            if (!m_IsSwitchingOut)
                return;

            m_FireDelayTimer += Time.deltaTime;
            if (m_FireDelayTimer >= m_FireDelay)
            {
                if (m_Target == null || m_Target.IsDead())
                {
                    m_Behaviour.SwitchState(m_PatrolState);
                }
                else
                {
                    m_ChaseState.SetTarget(m_Target);
                    m_Behaviour.SwitchState(m_ChaseState);
                }
            }
        }

        private void SwitchOut()
        {
            if (m_IsSwitchingOut)
                return;

            m_FireDelayTimer = 0.0f;
            m_IsSwitchingOut = true;
            m_Behaviour.Animator.SetTrigger("MovementTrigger");

            if (m_ChargeRoutine != null)
            {
                FireWeapon();
                StopCoroutine(m_ChargeRoutine);
                m_ChargeRoutine = null;
            }
        }

        private void HandleScanning()
        {
            if (m_ViewTransform == null)
                return;

            Collider[] colliders = Physics.OverlapSphere(m_ViewTransform.position, m_ViewRadius, m_ScanLayerMask);

            //For all targets in my radius
            for (int i = 0; i < colliders.Length; ++i)
            {
                Collider other = colliders[i];

                IDamageableObject damageableObject = other.GetComponent<IDamageableObject>();
                if (damageableObject == null)
                    return;

                damageableObject = damageableObject.GetMainDamageableObject();

                if (damageableObject == m_Target)
                {
                    if (m_Target.IsDead())
                    {
                        SwitchOut();
                        return;
                    }

                    //If so check if he's within the specified angle
                    Vector3 diffPos = other.transform.position - m_Behaviour.transform.position;
                    float dot = Vector3.Dot(m_Behaviour.transform.forward, diffPos.normalized);
                    float degAngle = (Mathf.Acos(dot) * Mathf.Rad2Deg * 2.0f);

                    if (degAngle <= m_ViewAngle)
                    {
                        //If so, check line of sight
                        Ray ray = new Ray(m_ViewTransform.position, (damageableObject.GetPosition() - m_ViewTransform.position));

                        RaycastHit hitInfo;
                        bool success = Physics.Raycast(ray, out hitInfo);

                        if (success)
                        {
                            IDamageableObject foundDamageableObject = hitInfo.collider.GetComponent<IDamageableObject>();
                            if (foundDamageableObject != null)
                            {
                                foundDamageableObject = foundDamageableObject.GetMainDamageableObject();

                                if (damageableObject != foundDamageableObject)
                                {
                                    SwitchOut();
                                    return;
                                }
                                else
                                {
                                    m_IsSwitchingOut = false;
                                    return;
                                }
                            }
                        }
                    }

                    //If not, stop firing
                    SwitchOut();
                }
            }
        }

        public void OnStateTriggerExit(Collider other)
        {
            if (other == null)
                return;

            IDamageableObject damageableObject = other.GetComponent<IDamageableObject>();
            if (damageableObject == null)
                return;

            damageableObject = damageableObject.GetMainDamageableObject();

            if (damageableObject != null && damageableObject == m_Target)
            {
                //Change to the chasing state
                SwitchOut();
            }
        }

        private void OnStateAnimatorIK(int layerIndex)
        {
            if (layerIndex == 1)
            {
                //Rotate the head
                float normTimer = 0.0f;
                
                if (m_FireDelay != 0.0f)
                    normTimer = (m_FireDelay - m_FireDelayTimer) / m_FireDelay;

                m_Behaviour.Animator.SetLookAtWeight(normTimer);
                m_Behaviour.Animator.SetLookAtPosition(m_Target.GetPosition());

                //Rotate the chest
                if (m_ChestRotationSpeed > 0)
                {
                    Quaternion desiredChestRotation = CalculateLocalBoneRotation(HumanBodyBones.Chest, m_HorizontalChestRotationOffset, m_VerticalChestRotationOffset);

                    if (m_LastChestLocalRotation == Quaternion.identity)
                    {
                        m_LastChestLocalRotation = Quaternion.RotateTowards(m_Behaviour.Animator.GetBoneTransform(HumanBodyBones.Chest).localRotation,
                                                                            desiredChestRotation,
                                                                            m_ChestRotationSpeed * Time.deltaTime);
                    }
                    else
                    {
                        m_LastChestLocalRotation = Quaternion.RotateTowards(m_LastChestLocalRotation,
                                                                            desiredChestRotation,
                                                                            m_ChestRotationSpeed * Time.deltaTime);
                    }

                    m_Behaviour.Animator.SetBoneLocalRotation(HumanBodyBones.Chest, m_LastChestLocalRotation);
                }
            }
        }

        private Quaternion CalculateLocalBoneRotation(HumanBodyBones boneType, float horizOffset, float vertOffset)
        {
            Quaternion desiredRotation = Quaternion.identity;

            if (m_IsSwitchingOut == false)
            {
                Vector3 direction = (m_Target.GetPosition() - m_Behaviour.Animator.GetBoneTransform(boneType).position).normalized;
                desiredRotation = Quaternion.LookRotation(direction);
                Vector3 euler = desiredRotation.eulerAngles;

                //Add the transform of the character, otherwise things get weird.
                euler.z = 360.0f - euler.x + (m_Behaviour.transform.rotation.eulerAngles.x) + vertOffset;
                euler.x = 360.0f - euler.y + (m_Behaviour.transform.rotation.eulerAngles.y) + horizOffset;
                euler.y = 0.0f;

                desiredRotation = Quaternion.Euler(euler);
            }
            else
            {
                desiredRotation = m_Behaviour.Animator.GetBoneTransform(boneType).localRotation;
            }

            return desiredRotation;
        }

        public override void SetTarget(IDamageableObject target)
        {
            m_Target = target;
        }

        public override string ToString()
        {
            return "Firing";
        }
    }
}