using System.Collections.Generic;
using UnityEngine;

namespace Kweek
{
    public class ZombieBehaviour : EnemyBehaviour
    {
        [Space(10)]
        [Header("Weapon")]
        [Space(5)]
        [SerializeField]
        private Weapon m_Weapon = null;
        public Weapon Weapon
        {
            get { return m_Weapon; }
        }

        public override void Setup(List<Collider> ownerColliders)
        {
            base.Setup(ownerColliders);

            if (m_Weapon != null)
                m_Weapon.Setup(ownerColliders, null);
        }
    }
}