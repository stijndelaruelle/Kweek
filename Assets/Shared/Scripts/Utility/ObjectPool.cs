using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectPoolDefinition
{
    [SerializeField]
    private PoolableObject m_ObjectType;
    public PoolableObject ObjectType
    {
        get { return m_ObjectType; }
    }

    [SerializeField]
    private int m_Amount;
    public int Amount
    {
        get { return m_Amount; }
    }

    [SerializeField]
    private bool m_DynamicallyGrow;
    public bool DynamicallyGrow
    {
        get { return m_DynamicallyGrow; }
    }

    public ObjectPoolDefinition()
    {
        m_ObjectType = null;
        m_Amount = 0;
        m_DynamicallyGrow = false;
    }

    public ObjectPoolDefinition(PoolableObject objectType, int amount, bool dynamicallyGrow)
    {
        m_ObjectType = objectType;
        m_Amount = amount;
        m_DynamicallyGrow = dynamicallyGrow;
    }
}

public class ObjectPool : MonoBehaviour
{
    private ObjectPoolDefinition m_Definition;
    private List<PoolableObject> m_PooledObjects;
    private int m_LastActivatedID = 0;

    public void Initialize(ObjectPoolDefinition definition)
    {
        m_PooledObjects = new List<PoolableObject>();
        m_Definition = definition;

        Clear();
        AddPooledObjects(definition.Amount);
    }

    //Mutators
    private void AddPooledObjects(int count)
    {
        for (int i = 0; i < count; ++i)
        {
            AddPooledObject();
        }
    }

    private int Grow()
    {
        int firstNewIndex = m_PooledObjects.Count;
        AddPooledObjects(m_PooledObjects.Count);

        return firstNewIndex;
    }

    private void Clear()
    {
        for (int i = 0; i < m_PooledObjects.Count; ++i)
        {
            Destroy(m_PooledObjects[i].gameObject);
        }

        m_PooledObjects.Clear();
    }

    private PoolableObject AddPooledObject()
    {
        PoolableObject instance = Instantiate(m_Definition.ObjectType, Vector3.zero, Quaternion.identity, transform) as PoolableObject; //Parent it to ourselves

        if (instance == null)
        {
            Destroy(instance);
            throw new MissingComponentException("Component PoolableObject was not found on the prefab " + m_Definition.ObjectType.ToString());
        }
        else
        {
            instance.Initialize();
            instance.Deactivate();
            m_PooledObjects.Add(instance);
            return instance;
        }
    }

    //Accessors
    public PoolableObject ActivateAvailableObject()
    {
        PoolableObject pooledObject = GetAvailableObject();

        if (pooledObject != null)
            pooledObject.Activate();

        return pooledObject;
    }

    public PoolableObject ActivateAvailableObjectNonDisruptive()
    {
        PoolableObject pooledObject = GetAvailableObjectNonDisruptive();

        if (pooledObject != null)
            pooledObject.Activate();

        return pooledObject;
    }

    public PoolableObject GetAvailableObject()
    {
        //Using this function, the pool will never grow
        if (m_PooledObjects.Count <= 0)
            return null;

        if (m_LastActivatedID <= 0 || m_LastActivatedID >= m_PooledObjects.Count)
        {
            m_LastActivatedID = 0;
        }

        PoolableObject pooledObject = m_PooledObjects[m_LastActivatedID];
        ++m_LastActivatedID;

        return pooledObject;
    }

    public PoolableObject GetAvailableObjectNonDisruptive()
    {
        if (m_PooledObjects.Count <= 0)
            return null;

        if (m_LastActivatedID <= 0 || m_LastActivatedID >= m_PooledObjects.Count)
        {
            m_LastActivatedID = 0;
        }

        //Find the first object in the list that is available
        for (int i = m_LastActivatedID; i < m_PooledObjects.Count; ++i)
        {
            if (m_PooledObjects[i].IsAvailable())
            {
                m_LastActivatedID = i;
                return m_PooledObjects[i];
            } 
        }

        //Loop around
        for (int i = 0; i < m_LastActivatedID; ++i)
        {
            if (m_PooledObjects[i].IsAvailable())
            {
                m_LastActivatedID = i;
                return m_PooledObjects[i];
            }
        }

        //If we are allowed to grow, do just that
        if (m_Definition.DynamicallyGrow)
        {
            int firstNewIndex = Grow();
            m_LastActivatedID = firstNewIndex;
            return m_PooledObjects[firstNewIndex];
        }

        //If we aren't allowed to grow, there is nothing else we can do
        return null;
    }

    public void DeactivateAll()
    {
        for (int i = 0; i < m_PooledObjects.Count; ++i)
        {
            m_PooledObjects[i].Deactivate();
        }
    }

    //FIX
    //Workaround because generics cannot be MonoBehaviours
    public bool IsPoolType<T>()
    {
        return (m_Definition.ObjectType is T);
    }
}
