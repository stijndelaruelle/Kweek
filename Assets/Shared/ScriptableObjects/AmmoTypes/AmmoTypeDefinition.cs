using UnityEngine;

namespace Kweek
{
    [CreateAssetMenu(fileName = "AmmoTypeDefinition", menuName = "Kweek/Ammo Type Definition")]
    public class AmmoTypeDefinition : ScriptableObject
    {
        [Tooltip("Maximum amount of ammo we can have of this ammo type")]
        [SerializeField]
        private int m_MaxAmmo = 0;
        public int MaxAmmo
        {
            get { return m_MaxAmmo; }
        }
    }
}