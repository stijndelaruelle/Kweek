using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieBehaviour : EnemyBehaviour
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

    public override void Setup(List<Collider> ownerColliders)
    {
        base.Setup(ownerColliders);
        m_Weapon.Setup(ownerColliders, null);
    }
}
