using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootstepEffect : MonoBehaviour
{
    [SerializeField]
    private SurfaceTypeDefinition m_DefaultSurfaceType;

    [SerializeField]
    private float m_TimeUntilFirstStep;

    [SerializeField]
    private float m_StepFrequency;
    private float m_StepTimer;

    [SerializeField]
    private AudioSource m_JumpAudioSource;

    [Space(10)]
    [Header("Required references")]
    [Space(5)]

    [SerializeField]
    private PlayerMovementController m_Player;

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
        if (m_AudioSource == null || m_DefaultSurfaceType == null)
            return;

        float playerSpeed = m_Player.Velocity.magnitude;
        if (m_Player.IsGrounded() == false || playerSpeed <= m_Player.MaxDuckSpeed)
        {
            m_StepTimer = m_TimeUntilFirstStep;
            return;
        }

        m_StepTimer -= Time.deltaTime;

        if (m_StepTimer <= 0.0f && playerSpeed > 0)
        {
            PlayFootstepSound();
            m_StepTimer += (m_StepFrequency / playerSpeed);
        }
    }

    private void PlayFootstepSound()
    {
        List<AudioClip> footstepSounds = m_DefaultSurfaceType.FootstepSounds;

        //Determine the current underground
        SurfaceType surfaceType = GetSurfaceType();
        if (surfaceType != null)
        {
            if (surfaceType.FootstepSounds != null)
                footstepSounds = surfaceType.FootstepSounds;
        }

        //Take a random footstep sound
        int randSound = 0;
        if (footstepSounds.Count > 1) randSound = UnityEngine.Random.Range(0, footstepSounds.Count - 1);

        //Play the land sound
        m_AudioSource.clip = footstepSounds[randSound];
        m_AudioSource.Play();
    }

    //Events
    private void OnPlayerJump()
    {
        if (m_AudioSource == null || m_JumpAudioSource == null)
            return;

        m_JumpAudioSource.Play();
    }

    private void OnPlayerLand()
    {
        if (m_AudioSource == null || m_DefaultSurfaceType == null)
            return;

        //Determine the current underground
        SurfaceType surfaceType = GetSurfaceType();
        if (surfaceType != null)
        {
            surfaceType.SpawnCharacterImpactEffect(m_Player.transform.position, Vector3.up);
        }
    }

    private SurfaceType GetSurfaceType()
    {
        RaycastHit hitInfo;
        bool success = Physics.Raycast(transform.position, Vector3.down, out hitInfo, 10.0f);

        if (success)
        {
            return hitInfo.collider.gameObject.GetComponent<SurfaceType>();
        }

        return null;
    }
}
