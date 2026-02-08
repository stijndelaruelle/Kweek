using UnityEngine;

//http://www.grapefruitgames.com/blog/2013/11/a-min-max-range-for-unity/

public class MinMaxRangeAttribute : PropertyAttribute
{
    private float m_MinLimit = 0.0f;
    public float MinLimit
    {
        get { return m_MinLimit; }
    }

    private float m_MaxLimit = 0.0f;
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
    public float m_Min = 0.0f;
    public float Min
    {
        get { return m_Min; }
        set { m_Min = value; }
    }

    public float m_Max = 0.0f;
    public float Max
    {
        get { return m_Max; }
        set { m_Max = value; }
    }

    public float GetValue(float t)
    {
        if (t <= 0.0f)
            return m_Min;

        if (t >= 1.0f)
            return m_Max;

        return Mathf.Lerp(m_Min, m_Max, t);
    }

    public float GetRandomValue()
    {
        return Random.Range(m_Min, m_Max);
    }
}