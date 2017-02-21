using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    private struct TransformData
    {
        private Vector3 m_LocalPosition;
        private Quaternion m_LocalRotation;
        private Vector3 m_LocalScale;

        public TransformData(Transform transform)
        {
            m_LocalPosition = transform.localPosition.Copy();
            m_LocalRotation = transform.localRotation.Copy();
            m_LocalScale = transform.localScale.Copy();
        }

        public void RestoreTransform(Transform transform)
        {
            transform.localPosition = m_LocalPosition;
            transform.localRotation = m_LocalRotation;
            transform.localScale = m_LocalScale;
        }
    }

    [SerializeField]
    private Animator m_Animator;

    //Get component instead of assigning, because assigning these manually is very error prone
    private RagdollPart[] m_RagdollParts;
    public RagdollPart[] RagdollParts
    {
        get { return m_RagdollParts; }
    }

    private Rigidbody[] m_Rigidbodies;

    private List<TransformData> m_RagdollTransformData;

    private void Awake()
    {
        m_RagdollParts = GetComponentsInChildren<RagdollPart>();
        m_Rigidbodies = GetComponentsInChildren<Rigidbody>();

        //Save all transform data
        m_RagdollTransformData = new List<TransformData>();
        SaveTransformData();
    }

    private void SaveTransformData()
    {
        m_RagdollTransformData.Clear();

        m_RagdollTransformData.Add(new TransformData(transform)); //Add the root
        foreach (RagdollPart ragdollPart in m_RagdollParts)
        {
            m_RagdollTransformData.Add(new TransformData(ragdollPart.transform));
        }
    }

    public void SetParent(Transform newTransform)
    {
        transform.parent = newTransform;
    }

    public void SetTransform(Transform newTransform)
    {
        transform.position = newTransform.position;
        transform.rotation = newTransform.rotation;
        transform.localScale = newTransform.localScale;
    }

    public void Reset()
    {
        m_RagdollTransformData[0].RestoreTransform(transform); //Root

        for (int i = 0; i < m_RagdollParts.Length; ++i)
        {
            m_RagdollTransformData[i + 1].RestoreTransform(m_RagdollParts[i].transform);
        }
    }


    public void SetKinematic(bool value)
    {
        for (int i = 0; i < m_Rigidbodies.Length; ++i)
        {
            m_Rigidbodies[i].isKinematic = value;
        }
    }

    public void SetActive(bool value)
    {
        if (m_Animator != null)
            m_Animator.enabled = (!value);
    }

    public bool IsRagdollEnabled()
    {
        if (m_Animator != null)
            return (!m_Animator.enabled);
        else
            return true;
    }
}
