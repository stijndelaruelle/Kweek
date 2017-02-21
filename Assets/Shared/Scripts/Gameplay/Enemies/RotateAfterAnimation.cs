using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAfterAnimation : MonoBehaviour
{
    [SerializeField]
    private Animator m_Animator;

    [SerializeField]
    private Vector3 m_EulerAngles;

    private void LateUpdate()
    {
        if (m_Animator.enabled == false)
            return;

        //Rotate ourselves to the target direction, our model is a bit offset so we can't let the navmeshagent handle it.
        transform.rotation *= Quaternion.Euler(m_EulerAngles);
    }
}
