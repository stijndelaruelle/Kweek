using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Base Movement Settings")]
    [SerializeField]
    private float m_MovementSpeed;

    [SerializeField]
    private float m_RotationSpeed;

    [Header("Movement Options")]
    [SerializeField]
    private bool m_BackAndForth = true;

    [Header("Required references")]
    [SerializeField]
    private Transform m_TargetTransform;

    private Vector3 m_TargetPosition;
    private Vector3 m_StartPosition;
    private Vector3 m_Direction;

    private List<MoveableObject> m_MoveableObjects;

    private void Awake()
    {
        m_StartPosition = transform.position.Copy();
        m_TargetPosition = m_TargetTransform.position.Copy();

        CalculateDirection();

        m_MoveableObjects = new List<MoveableObject>();
    }

    private void Update()
    {
        //Update passengers first, otherwise there is a snap when changing directions
        UpdatePassengers();
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        //Move
        transform.position += m_Direction * m_MovementSpeed * Time.deltaTime;

        //Check if we reached our destination
        float distance = (m_TargetPosition - transform.position).sqrMagnitude;

        if (distance < 0.01f)
        {
            //Doesn't work well when there are objects on the platform (fix later)
            //transform.position = m_TargetPosition;

            if (m_BackAndForth)
            {
                Vector3 temp = m_TargetPosition.Copy();
                m_TargetPosition = m_StartPosition;
                m_StartPosition = temp;

                CalculateDirection();
            }
        }
    }

    private void UpdatePassengers()
    {
        foreach (MoveableObject passenger in m_MoveableObjects)
        {
            passenger.AddVelocity(m_Direction * m_MovementSpeed);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        MoveableObject moveableObject = collision.gameObject.GetComponent<MoveableObject>();

        if (moveableObject != null)
        {
            if (m_MoveableObjects.Contains(moveableObject) == false)
            {
                Debug.Log("Started transporting: " + collision.gameObject.name);
                m_MoveableObjects.Add(moveableObject);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        MoveableObject moveableObject = collision.gameObject.GetComponent<MoveableObject>();

        if (moveableObject != null)
        {
            if (m_MoveableObjects.Contains(moveableObject) == true)
            {
                Debug.Log("Stopped transporting: " + collision.gameObject.name);
                m_MoveableObjects.Remove(moveableObject);
            }
        }
    }

    private void CalculateDirection()
    {
        m_Direction = (m_TargetPosition - transform.position).normalized;
    }
}
