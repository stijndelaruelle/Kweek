using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoldierBehaviour : EnemyBehaviour
{
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
    private Transform m_BackWeaponGrip;

    protected override void Start()
    {
        base.Start();
        m_Weapon.UpdateAmmoEvent += OnUpdateWeaponAmmo;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (m_Weapon != null)
            m_Weapon.UpdateAmmoEvent -= OnUpdateWeaponAmmo;
    }

    public override void Setup(List<Collider> ownerColliders)
    {
        base.Setup(ownerColliders);

        m_Weapon.Setup(ownerColliders, null);

        if (m_WeaponPickup != null)
        {
            m_WeaponPickup.enabled = false;
            m_WeaponPickup.IgnoreColliders(ownerColliders); //Make sure we don't freak out when becomming a ragdoll
        }
    }

    public override void OnDeath()
    {
        base.OnDeath();

        if (m_WeaponPickup != null)
        {
            //Throw the weapon
            m_WeaponPickup.enabled = true;
            m_WeaponPickup.gameObject.transform.parent = null;
            m_WeaponPickup.Drop(transform.forward.Copy() * 500.0f, null);
        }
    }

    private void OnUpdateWeaponAmmo(int ammoInClip, int reserveAmmo)
    {
        if (m_WeaponPickup != null)
            m_WeaponPickup.Ammo = ammoInClip;
    }

    protected override void OnAnimatorIK(int layerIndex)
    {
        base.OnAnimatorIK(layerIndex);

        ////Set the left hand to the weapongrip(layer 1 so we get new positions!)
        //if (layerIndex == 1)
        //{
        //    m_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        //    m_Animator.SetIKPosition(AvatarIKGoal.RightHand, m_FrontWeaponGrip.position);

        //    m_Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
        //    m_Animator.SetIKRotation(AvatarIKGoal.RightHand, m_FrontWeaponGrip.rotation);
        //}

        if (layerIndex == 2 && m_BackWeaponGrip != null)
        {
            m_Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            m_Animator.SetIKPosition(AvatarIKGoal.LeftHand, m_BackWeaponGrip.position);

            m_Animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            m_Animator.SetIKRotation(AvatarIKGoal.LeftHand, m_BackWeaponGrip.rotation);
        }
    }
}
