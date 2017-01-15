﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Once collisions are enabled this script turns the character in a ragdoll once it hits a surface
//F.e.: The animator will do an animation until they hit a wall, then the ragdoll will take over

public class RagdollPart : MonoBehaviour
{
    [SerializeField]
    private Enemy m_Enemy;
    private List<GameObject> m_OtherParts;

    private void Start()
    {
        m_OtherParts = m_Enemy.RagdollParts;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (m_OtherParts.Contains(collision.gameObject))
            return;

        //Don't start ragdolls when we just hit a regular floor (not a slope!)
        if (collision.contacts[0].normal == Vector3.up)
            return;

        Debug.Log(gameObject.name + " enabled the ragdoll!", gameObject);
        m_Enemy.EnableRagdoll();
    }
}
