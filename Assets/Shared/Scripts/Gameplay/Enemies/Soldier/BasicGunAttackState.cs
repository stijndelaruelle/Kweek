using System.Collections;
using UnityEngine;

namespace Kweek
{
    [RequireComponent(typeof(EnemyWeaponPickupBehaviour))]
    public class BasicGunAttackState : IAbstractTargetState
    {
        private EnemyWeaponPickupBehaviour m_Soldier = null;

        [Header("Fireing")]
        [Space(5)]
        [SerializeField]
        private Transform m_FireTransform = null;

        [SerializeField]
        private float m_FireDelay = 0.0f;
        private float m_FireDelayTimer = 0.0f;

        [SerializeField]
        private float m_BurstDistance = 5.0f;

        [SerializeField]
        private float m_SingleShotDistance = 15.0f;

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
        private IAbstractTargetState m_ChaseState = null;

        [SerializeField]
        private Transform m_ViewTransform = null;

        [SerializeField]
        private float m_ViewRadius = 0.0f;

        [SerializeField]
        private float m_ViewAngle = 0.0f;

        [SerializeField]
        private LayerMask m_ScanLayerMask = default(LayerMask);

        [Space(10)]
        [Header("Other States")]
        [Space(5)]
        [SerializeField]
        private IAbstractState m_DefaultState = null;

        private bool m_IsInFireStance = false;
        private bool m_IsSwitchingOut = false;

        private IDamageableObject m_Target = null;
        private Coroutine m_FireRoutine = null;
        private Quaternion m_LastChestLocalRotation = Quaternion.identity; //Chest bone rotation constatntly resets, cache it here.
        private Quaternion m_LastRightArmLocalRotation = Quaternion.identity;

        private void Awake()
        {
            //Assigning this manually clutters the inspector a LOT!
            //If we, at some point, want to detach state objects from their behaviour, revert this.
            m_Soldier = GetComponent<EnemyWeaponPickupBehaviour>();
        }

        public override void Enter()
        {
            //Debug.Log("Entered fire state!");

            m_Soldier.AnimatorIKEvent += OnStateAnimatorIK;

            //Stop the character from moving, both gamewise as visually
            m_Soldier.NavMeshAgent.isStopped = true;

            m_Target = null;
            m_FireDelayTimer = m_FireDelay;
            m_IsInFireStance = false;
            m_IsSwitchingOut = false;

            m_LastChestLocalRotation = Quaternion.identity;
        }

        public override void Exit()
        {
            if (m_Soldier == null)
                return;

            m_Soldier.AnimatorIKEvent -= OnStateAnimatorIK;
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
            if (m_IsSwitchingOut || m_FireDelayTimer > 0.0f)
                return;

            if (m_FireRoutine != null)
                return;

            //Check the distance between us and the target
            Vector3 diff = m_Target.GetPosition() - m_FireTransform.position;
            float distance = diff.magnitude;

            //Full auto
            if (distance < m_BurstDistance)
            {
                ShootOnce();
                return;
            }

            //Burstfire
            if (distance < m_SingleShotDistance)
            {
                m_FireRoutine = m_Soldier.StartCoroutine(FireBurstRoutine(3));
                return;
            }

            //Single shot
            m_FireRoutine = m_Soldier.StartCoroutine(FireBurstRoutine(1));
            return;
        }

        private IEnumerator FireBurstRoutine(int numberOfBullets)
        {
            int bulletsFired = 0;

            while (bulletsFired < numberOfBullets)
            {
                bool success = ShootOnce();
                if (success) { bulletsFired++; }

                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForSeconds(1.0f);
            m_FireRoutine = null;
        }

        private bool ShootOnce()
        {
            if (m_Target.IsDead())
            {
                SwitchOut();
                return false;
            }

            //We used to shoot in the direction of "m_FirePosition.forward", and it seemed to work with a bit of tweaking.
            //However when our character is on a slope it consistently misses shots.
            //Therefore the direction we shoot in and the visuals are now separate from eachother.
            Vector3 normDirection = (m_Target.GetPosition() - m_FireTransform.position).normalized;
            Ray fireRay = new Ray(m_FireTransform.position, normDirection);

            return m_Soldier.Weapon.Use(fireRay);
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
            if (m_FireDelayTimer >= m_FireDelay)
            {
                if (m_Target.IsDead())
                {
                    m_Soldier.SwitchState(m_DefaultState);
                }
                else
                {
                    m_ChaseState.SetTarget(m_Target);
                    m_Soldier.SwitchState(m_ChaseState);
                }
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

        private void HandleScanning()
        {
            Collider[] colliders = Physics.OverlapSphere(m_ViewTransform.position, m_ViewRadius, m_ScanLayerMask);

            //For all targets in my radius
            for (int i = 0; i < colliders.Length; ++i)
            {
                Collider other = colliders[i];

                IDamageableObject damageableObject = other.GetComponent<IDamageableObject>();
                if (damageableObject == null)
                    return;

                damageableObject = damageableObject.GetMainDamageableObject();

                //Check if it's my target
                if (damageableObject == m_Target)
                {
                    if (m_Target.IsDead())
                    {
                        SwitchOut();
                        return;
                    }

                    //If so check if he's within the specified angle
                    Vector3 diffPos = other.transform.position - m_Soldier.transform.position;
                    float dot = Vector3.Dot(m_Soldier.transform.forward, diffPos.normalized);
                    float degAngle = (Mathf.Acos(dot) * Mathf.Rad2Deg * 2.0f);

                    if (degAngle <= m_ViewAngle)
                    {
                        //If so, check line of sight
                        Ray ray = new Ray(m_ViewTransform.position, (damageableObject.GetPosition() - m_ViewTransform.position));
                        Debug.DrawRay(ray.origin, ray.direction * 100, Color.blue);
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

                    //Our target is no longer in our line of sight, start chasing
                    SwitchOut();
                    return;
                }
            }

            //Our target is no longer in our radius, start chasing
            SwitchOut();
        }

        private void OnStateAnimatorIK(int layerIndex)
        {
            if (layerIndex == 1)
            {
                //Rotate the head
                float normTimer = (m_FireDelay - m_FireDelayTimer) / m_FireDelay;
                m_Soldier.Animator.SetLookAtWeight(normTimer);

                m_Soldier.Animator.SetLookAtPosition(m_Target.GetPosition());

                //Rotate the chest
                if (m_ChestRotationSpeed > 0)
                {
                    Quaternion desiredChestRotation = CalculateLocalBoneRotation(HumanBodyBones.Chest, m_HorizontalChestRotationOffset, m_VerticalChestRotationOffset);

                    if (m_LastChestLocalRotation == Quaternion.identity)
                    {
                        m_LastChestLocalRotation = Quaternion.RotateTowards(m_Soldier.Animator.GetBoneTransform(HumanBodyBones.Chest).localRotation,
                                                                            desiredChestRotation,
                                                                            m_ChestRotationSpeed * Time.deltaTime);
                    }
                    else
                    {
                        m_LastChestLocalRotation = Quaternion.RotateTowards(m_LastChestLocalRotation,
                                                                            desiredChestRotation,
                                                                            m_ChestRotationSpeed * Time.deltaTime);
                    }

                    m_Soldier.Animator.SetBoneLocalRotation(HumanBodyBones.Chest, m_LastChestLocalRotation);
                }
            }
        }

        private Quaternion CalculateLocalBoneRotation(HumanBodyBones boneType, float horizOffset, float vertOffset)
        {
            Quaternion desiredRotation;

            if (m_IsSwitchingOut == false)
            {
                Vector3 direction = (m_Target.GetPosition() - m_Soldier.Animator.GetBoneTransform(boneType).position).normalized;
                desiredRotation = Quaternion.LookRotation(direction);
                Vector3 euler = desiredRotation.eulerAngles;

                //Add the transform of the character, otherwise things get weird.
                euler.z = 360.0f - euler.x + (m_Soldier.transform.rotation.eulerAngles.x) + vertOffset;
                euler.x = 360.0f - euler.y + (m_Soldier.transform.rotation.eulerAngles.y) + horizOffset;
                euler.y = 0.0f;

                desiredRotation = Quaternion.Euler(euler);
            }
            else
            {
                desiredRotation = m_Soldier.Animator.GetBoneTransform(boneType).localRotation;
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