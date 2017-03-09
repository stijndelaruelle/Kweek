using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//http://www.grapefruitgames.com/blog/2013/11/a-min-max-range-for-unity/

public class MinMaxRangeAttribute : PropertyAttribute
{
    private float m_MinLimit;
    public float MinLimit
    {
        get { return m_MinLimit; }
    }

    private float m_MaxLimit;
    public float MaxLimit
    {
        get { return m_MaxLimit; }
    }

    public MinMaxRangeAttribute(float minLimit, float maxLimit)
    {
        m_MinLimit = minLimit;
        m_MaxLimit = maxLimit;
    }
}

[System.Serializable]
public class MinMaxRange
{
    public float m_Min;
    public float Min
    {
        get { return m_Min; }
        set { m_Min = value; }
    }

    public float m_Max;
    public float Max
    {
        get { return m_Max; }
        set { m_Max = value; }
    }

    public float GetRandomValue()
    {
        return Random.Range(m_Min, m_Max);
    }
}