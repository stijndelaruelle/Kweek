using UnityEngine;

namespace Kweek
{
    public class MuzzleFlash : MonoBehaviour
    {
        [SerializeField]
        private Weapon m_Weapon = null;

        [SerializeField]
        private float m_MaxMeshAlpha = 0.0f;

        [SerializeField]
        private float m_MaxLightIntensity = 0.0f;

        [SerializeField]
        private MeshRenderer m_MeshRenderer = null;
        private Material m_Material = null;

        [SerializeField]
        private Light m_Light = null;

        [SerializeField]
        private float m_ActiveTime = 0.0f;
        private float m_Timer = 0.0f;

        private void Start()
        {
            if (m_Weapon != null)
                m_Weapon.WeaponUseEvent += OnWeaponFire;

            m_Material = m_MeshRenderer.material;

            ShowMuzzleFlash(false);
        }

        private void OnDestroy()
        {
            if (m_Weapon != null)
                m_Weapon.WeaponUseEvent -= OnWeaponFire;
        }

        private void Update()
        {
            if (m_Timer > 0.0f)
            {
                m_Timer -= Time.deltaTime;

                //0 -> 1 -> 0
                float normScale = 1.0f - ((Mathf.Abs((m_ActiveTime / 2) - m_Timer) * 2) / m_ActiveTime);

                //Set the material color
                Color color = m_Material.GetColor("_TintColor");
                color.a = normScale;
                m_Material.SetColor("_TintColor", color);

                //Set light intensity
                m_Light.intensity = normScale * m_MaxLightIntensity;

                if (m_Timer < 0.0f)
                {
                    ShowMuzzleFlash(false);
                    m_Timer = 0.0f;
                }
            }
        }

        private void OnWeaponFire(Vector3 direction)
        {
            m_Timer = m_ActiveTime;

            //Rotate the mesh randomly
            float randomAngle = UnityEngine.Random.Range(0.0f, 360.0f);
            m_MeshRenderer.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, randomAngle);

            ShowMuzzleFlash(true);
        }

        private void ShowMuzzleFlash(bool state)
        {
            m_MeshRenderer.enabled = state;
            m_Light.enabled = state;
        }
    }
}