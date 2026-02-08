using UnityEngine;

namespace Kweek
{
    public interface IMoveableObject
    {
        void AddVelocity(Vector3 velocity);
        Vector3 GetCenterOfMass();
    }
}