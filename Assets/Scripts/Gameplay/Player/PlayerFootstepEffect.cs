using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootstepEffect : MonoBehaviour
{
    [SerializeField]
    private SurfaceTypeDefinition m_DefaultSurfaceType;

    [SerializeField]
    private float m_StepFrequency;

    [SerializeField]
    private AudioClip m_JumpSound;

    [Space(10)]
    [Header("Required references")]
    [Space(5)]

    [SerializeField]
    private Player m_Player;

    [SerializeField]
    private AudioSource m_AudioSource;

    private void Start()
    {
        m_Player.JumpEvent += OnPlayerJump;
        m_Player.LandEvent += OnPlayerLand;
    }

    private void OnDestroy()
    {
        if (m_Player == null)
            return;

        m_Player.JumpEvent -= OnPlayerJump;
        m_Player.LandEvent -= OnPlayerLand;
    }

    private void Update()
    {
        if (m_AudioSource == null || m_Player.IsGrounded() == false)
            return;

        //Determine the underground

        //Determine the veloctiy
    }

    private void OnPlayerJump()
    {
        if (m_AudioSource == null || m_JumpSound == null)
            return;

        m_AudioSource.clip = m_JumpSound;
        m_AudioSource.Play();
    }

    private void OnPlayerLand()
    {
        if (m_AudioSource == null || m_DefaultSurfaceType == null)
            return;

        AudioClip landSound = m_DefaultSurfaceType.LandSound;

        //Determine the current underground
        RaycastHit hitInfo;
        bool success = Physics.Raycast(transform.position, Vector3.down, out hitInfo, 1.0f);

        if (success)
        {
            SurfaceType surfaceType = hitInfo.collider.gameObject.GetComponent<SurfaceType>();
        
            if (surfaceType != null)
            {
                if (surfaceType.LandSound != null)
                    landSound = surfaceType.LandSound;
            }
        }

        //Play the land sound
        m_AudioSource.clip = landSound;
        m_AudioSource.Play();
    }
}
