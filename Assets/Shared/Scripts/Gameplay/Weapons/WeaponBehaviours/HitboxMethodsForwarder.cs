using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void HitboxMethodDelegate();

public class HitboxMethodsForwarder : MonoBehaviour
{
    public event HitboxMethodDelegate HitboxEnableEvent;
    public event HitboxMethodDelegate HitboxDisableEvent;

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
