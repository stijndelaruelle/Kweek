using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;

public class DebugDrawValue : MonoBehaviour
{
    //Make this way more generic
    //UnityEvents doesn't support return values.
    public enum PlayerValue
    {
        CurrentVelocity,
        CurrentSpeed,
        CurrentState
    }

    [SerializeField]
    private Text m_Text;

    [SerializeField]
    private PlayerController m_Player;

    [SerializeField]
    private PlayerValue m_Value;

    private void Update()
    {
        if (m_Player == null)
            return;

        m_Text.text = m_Value.ToString() + ": ";

        switch (m_Value)
        {
            case PlayerValue.CurrentVelocity:
                m_Text.text += m_Player.Velocity;
                break;

            case PlayerValue.CurrentSpeed:
                m_Text.text += m_Player.Velocity.magnitude;
                break;

            case PlayerValue.CurrentState:
                m_Text.text += m_Player.CurrentState;
                break;

            default:
                break;
        }
    }
}