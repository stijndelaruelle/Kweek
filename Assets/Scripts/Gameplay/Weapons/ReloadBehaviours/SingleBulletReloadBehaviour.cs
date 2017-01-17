using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Even tough I normally don't derive just for code reuse, in this case it was fairly extreme.
//Code readability was prioritsed. (this can change, depending on further IAmmoUseBehaviour implementations)
public class SingleBulletReloadBehaviour : FullClipReloadBehaviour
{
    protected override void EndReload()
    {
        m_AmmoArsenal.ChangeAmmo(m_AmmoType, -1);
        m_AmmoInClip += 1;

        FireUpdateAmmoEvent();

        m_ReloadTimer = 0.0f;

        //Keep reloading if required
        if (m_AmmoInClip < m_MaxAmmoInClip)
        {
            StartReload();
        }
    }

    public override bool CanUse()
    {
        return (m_AmmoInClip > 0);
    }
}
