using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class CharacterFootstepEffect : MonoBehaviour
{
    [SerializeField]
    private SurfaceTypeDefinition m_DefaultSurfaceType;

    [SerializeField]
    private AudioSource m_AudioSource;

    public void PlayFootstep()
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
