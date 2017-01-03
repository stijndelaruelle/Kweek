using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;

public class DebugDrawValue : MonoBehaviour
{
    //Make this way more generic
    //UnityEvents don't support return values.
    public enum PlayerValue
    {
        CurrentSpeed,
        CurrentVelocity,
        IsGrounded
    }

    [SerializeField]
    private Text m_Text;

    [SerializeField]
    private PlayerController m_PlayerController;

    [SerializeField]
    private PlayerValue m_Value;

    private void Update()
    {
        if (m_PlayerController == null)
            return;

        m_Text.text = m_Value.ToString() + ": ";

        switch (m_Value)
        {
            case PlayerValue.CurrentSpeed:
                m_Text.text += m_PlayerController.CurrentSpeed;
                break;

            case PlayerValue.CurrentVelocity:
                m_Text.text += m_PlayerController.CurrentVelocity.x + " " + m_PlayerController.CurrentVelocity.y + " " + m_PlayerController.CurrentVelocity.z;
                break;

            case PlayerValue.IsGrounded:
                m_Text.text += m_PlayerController.IsGrounded;
                break;

            default:
                break;
        }
    }
}