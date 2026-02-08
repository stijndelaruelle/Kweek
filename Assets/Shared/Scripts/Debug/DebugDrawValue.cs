using UnityEngine;
using UnityEngine.UI;

namespace Kweek
{
    public class DebugDrawValue : MonoBehaviour
    {
        //Make this way more generic
        //UnityEvents doesn't support return values.
        public enum PlayerValue : int
        {
            None = 0,
            CurrentVelocity = 1,
            CurrentSpeed = 2,
            CurrentState = 3
        }

        [SerializeField]
        private Text m_Text;

        [SerializeField]
        private PlayerMovementController m_Player;

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
                    m_Text.text += m_Player.CurrentStateString;
                    break;

                case PlayerValue.None:
                default:
                    break;
            }
        }
    }
}