using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IAIBehaviour : MonoBehaviour
{
    public abstract void Setup(List<Collider> ownerColliders);
    public abstract void OnDeath();
}
