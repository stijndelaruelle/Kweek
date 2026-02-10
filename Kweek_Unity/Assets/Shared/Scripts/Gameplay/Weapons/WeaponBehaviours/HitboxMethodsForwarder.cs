using UnityEngine;

namespace Kweek
{
    public delegate void HitboxMethodDelegate();

    public class HitboxMethodsForwarder : MonoBehaviour
    {
        public event HitboxMethodDelegate HitboxEnableEvent = null;
        public event HitboxMethodDelegate HitboxDisableEvent = null;

        //More will follow if i end up using them
        private void EnableHitbox()
        {
            if (HitboxEnableEvent != null)
                HitboxEnableEvent();
        }

        private void DisableHitbox()
        {
            if (HitboxDisableEvent != null)
                HitboxDisableEvent();
        }
    }
}