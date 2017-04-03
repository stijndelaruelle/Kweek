using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeTrail : MonoBehaviour
{
    [SerializeField]
    private int m_NumberOfPoints = 10;
    private int m_CurrentNumberOfPoints = 0;

    [SerializeField]
    private float m_UpdateSpeed = 0.25f;
    private float m_UpdateTimer = 0.0f;

    [SerializeField]
    private float m_MaxSpeedOffset = 0.2f;

    [SerializeField]
    private float m_MinSpeed = 0.2f;

    [SerializeField]
    private float m_MaxSpeed = 0.2f;

    [SerializeField]
    private float m_MaxOffsetPerUpdate = 0.2f;

    [SerializeField]
    private float m_MaxCenterOffset = 0.5f;

    [SerializeField]
    private LineRenderer m_LineRenderer;
    private Material m_LineMaterial;

    private Vector3[] m_Positions;
    private Vector3[] m_Directions;

    private float lineSegment = 0.0f;

    private void Start()
    {
        m_LineMaterial = m_LineRenderer.material;

        lineSegment = 1.0f / m_NumberOfPoints;

        m_Positions = new Vector3[m_NumberOfPoints];
        m_Directions = new Vector3[m_NumberOfPoints];

        m_LineRenderer.positionCount = 0;
        m_UpdateTimer = m_UpdateSpeed;
    }

    private void Update()
    {
        m_UpdateTimer += Time.deltaTime;

        // It's time to add another point to the line
        if (m_UpdateTimer > m_UpdateSpeed)
        {
            m_UpdateTimer -= m_UpdateSpeed;

            //Add a new point if possible
            if (m_CurrentNumberOfPoints < m_NumberOfPoints)
            {
                m_CurrentNumberOfPoints++;
                m_LineRenderer.positionCount = m_CurrentNumberOfPoints;
            }

            //Move all the points up in the chain
            for (int i = m_CurrentNumberOfPoints - 1; i > 0; i--)
            {
                m_Positions[i] = m_Positions[i - 1];
                m_Directions[i] = m_Directions[i - 1];
            }

            //Set the new point
            m_Positions[0] = transform.position;

            Vector3 prevDir = GetRandomSmokeDirection(m_Directions[1]);
            if (m_CurrentNumberOfPoints == 1) prevDir = Vector3.up * ((m_MaxSpeed - m_MinSpeed) + m_MinSpeed); //Average

            m_Directions[0] = prevDir;
            m_LineRenderer.SetPosition(0, m_Positions[0]);
        }

        //Move all the points upwards
        for (int i = 1; i < m_CurrentNumberOfPoints; i++)
        {
            m_Positions[i] += m_Directions[i] * Time.deltaTime;
            m_LineRenderer.SetPosition(i, m_Positions[i]);
        }

        m_Positions[0] = transform.position; // 0th point is a special case, always follows the transform directly.
        m_LineRenderer.SetPosition(0, transform.position);

        // If we're at the maximum number of points, tweak the offset so that the last line segment is "invisible" (i.e. off the top of the texture) when it disappears.
        // Makes the change less jarring and ensures the texture doesn't jump.
        if (m_CurrentNumberOfPoints < m_NumberOfPoints)
        {
            m_LineMaterial.mainTextureOffset = new Vector2(lineSegment * (m_UpdateTimer / m_UpdateSpeed), 0.0f);
        }
    }

    // Give a random upwards vector.
    private Vector3 GetRandomSmokeDirection(Vector3 previousDir)
    {
        Vector3 smokeVec;

        float xLeftLimit = previousDir.x - m_MaxOffsetPerUpdate;
        if (xLeftLimit < -m_MaxCenterOffset) xLeftLimit = -m_MaxCenterOffset;

        float xRightLimit = previousDir.x + m_MaxOffsetPerUpdate;
        if (xRightLimit > m_MaxCenterOffset) xRightLimit = m_MaxCenterOffset;

        smokeVec.x = UnityEngine.Random.Range(xLeftLimit, xRightLimit);


        float zLeftLimit = previousDir.z - m_MaxOffsetPerUpdate;
        if (zLeftLimit < -m_MaxCenterOffset) zLeftLimit = -m_MaxCenterOffset;

        float zRightLimit = previousDir.z + m_MaxOffsetPerUpdate;
        if (zRightLimit > m_MaxCenterOffset) zRightLimit = m_MaxCenterOffset;

        smokeVec.z = UnityEngine.Random.Range(zLeftLimit, zRightLimit);


        float yLeftLimit = previousDir.y - m_MaxSpeedOffset;
        if (yLeftLimit < m_MinSpeed) yLeftLimit = m_MinSpeed;

        float yRightLimit = previousDir.y + m_MaxSpeedOffset;
        if (yRightLimit > m_MaxSpeed) yRightLimit = m_MaxSpeed;

        smokeVec.y = UnityEngine.Random.Range(yLeftLimit, yRightLimit);

        return smokeVec;
    }
}
