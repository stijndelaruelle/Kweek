using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Doesn't really have to be a MonoBehaviour as it only exists for 1 frame.
//We'll change this later, but for the sake of simplicity let's make it work the same way as the physical projectiles
public class HitScanProjectile : MonoBehaviour, IProjectile
{
    [SerializeField]
    private int m_Damage;

    [SerializeField]
    private AnimationCurve m_DamageFalloff;

    //[SerializeField]
    //private bool m_Piercing;
    //Piercing falloff is determined by the material we've hit (think counter strike)


    public void Fire(Vector3 baseVelocity, Ray ray, float range)
    {
        //Fire a single ray (get only the first target)
        RaycastHit hitInfo;
        bool succes = Physics.Raycast(ray, out hitInfo, range);

        if (!succes)
        {
            //Debug.DrawRay(ray.origin, ray.direction * range, Color.red, 5.0f);
            return;
        }

        GameObject go = hitInfo.collider.gameObject;
        //Debug.DrawLine(ray.origin, hitInfo.point, Color.green, 5.0f);

        //Did we hit a damageableobject?
        IDamageableObject damageableObject = go.GetComponent<IDamageableObject>();

        if (damageableObject != null)
        {
            //Damage calculation
            int damage = CalculateDamage(ray.origin, hitInfo.point, range);

            damageableObject.Damage(damage);
            Destroy(gameObject);
            return;
        }

        //Ddi we hit a surface?
        SurfaceType surfaceType = go.GetComponent<SurfaceType>();
        if (surfaceType != null)
        {
            Vector3 decalPosition = hitInfo.point + (hitInfo.normal * 0.01f); //Offset the decal a bit from the wall
            Quaternion decalRotation = Quaternion.LookRotation(hitInfo.normal, Vector3.up);

            GameObject.Instantiate(surfaceType.Decal, decalPosition, decalRotation);
        }
    }

    private List<IDamageableObject> FirePiercingRay(Ray ray, out List<RaycastHit> hitInfo)
    {
        //------------------
        // TODO: Piercing damage (RaycastAll)
        //------------------
        hitInfo = new List<RaycastHit>();
        return null;



        //Fire a single ray (get all the targets)
        //RaycastHit[] hitInfoArr = Physics.RaycastAll(centerRay, m_Range);

        //if (hitInfoArr.Length > 0)
        //{
        //for (int i = 0; i < hitInfoArr.Length; ++i)
        //{
        //Debug.Log("COULD HAVE HIT: " + hitInfoArr[i].collider.gameObject.name);

        //        IDamageableObject damageableObject = hitInfo[i].collider.gameObject.GetComponent<IDamageableObject>();

        //        if (damageableObject != null)
        //        {
        //            damageableObject.Damage(m_Damage);
        //        }
        //}
        //}
    }

    private int CalculateDamage(Vector3 start, Vector3 end, float range)
    {
        float distance = (end - start).magnitude;
        float normDistance = distance / range;
        float damagePercentage = m_DamageFalloff.Evaluate(normDistance);

        Debug.Log("Damage percentage: " + damagePercentage + " = " + (m_Damage * damagePercentage));
        return (int)(m_Damage * damagePercentage);
    }
}
