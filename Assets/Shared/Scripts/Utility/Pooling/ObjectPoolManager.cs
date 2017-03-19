using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    [SerializeField]
    private List<ObjectPoolDefinition> m_ObjectPoolDefinitions;
    private Dictionary<PoolableObject, ObjectPool> m_ObjectPools;

    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }
    
    private void Initialize()
    {
        m_ObjectPools = new Dictionary<PoolableObject, ObjectPool>();

        foreach (ObjectPoolDefinition definition in m_ObjectPoolDefinitions)
        {
            if (definition.ObjectType != null)
            {
                //If the pool already exists, notify the developer
                if (m_ObjectPools.ContainsKey(definition.ObjectType))
                {
                    Debug.LogWarning("Trying to create an already existing pool \"" + definition.ToString() + "\" please extend the original instead.");
                }
                else
                {
                    ObjectPool newPool = new GameObject("Pool " + definition.ObjectType.name).AddComponent<ObjectPool>();
                    newPool.transform.SetParent(transform); //Parent it to ourselves

                    newPool.Initialize(definition);

                    m_ObjectPools.Add(definition.ObjectType, newPool);
                }
            }
        }
    }

    public ObjectPool GetPool(PoolableObject poolableObject)
    {
        if (poolableObject == null)
            return null;

        if (m_ObjectPools.ContainsKey(poolableObject))
        {
            return m_ObjectPools[poolableObject];
        }

        return null;
    }
}
