using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Base Movement Settings")]
    [SerializeField]
    private float m_Acceleration = 1.0f;

    [SerializeField]
    private float m_AirAcceleration = 1.0f;

    [SerializeField]
    private float m_Friction = 1.0f;

    [SerializeField]
    private float m_AirFriction = 1.0f;

    [SerializeField]
    private float m_MaxGroundSpeed = 20.0f;

    [SerializeField]
    private float m_MaxAirialSpeed = 30.0f;

    [SerializeField]
    private float m_JumpSpeed = 8.0f;

    [SerializeField]
    private float m_Gravity = 1.0f;

    [Header("Movement Options")]
    [SerializeField]
    private int m_MaxNumberOfJumps = 2;
    private int m_NumberOfJumps = 2;

    [Header("Collision Settings")]
    [SerializeField]
    private float m_SkinWidth;

    [Header("Required references")]
    [SerializeField]
    private Transform m_Camera;

    [SerializeField]
    private Gun m_Gun;

    //Exposed to debug
    private Vector3 m_AddedVelocity;
    private Vector3 m_CurrentVelocity;
    public Vector3 CurrentVelocity
    {
        get { return m_CurrentVelocity; }
    }

    private bool m_IsGrounded;
    public bool IsGrounded
    {
        get { return m_IsGrounded; }
    }

    private float m_CurrentSpeed;
    public float CurrentSpeed
    {
        get { return m_CurrentSpeed; }
    }

    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;

    private void Awake()
    {
        m_NumberOfJumps = m_MaxNumberOfJumps;

        m_CharacterTargetRot = transform.localRotation;
        m_CameraTargetRot = m_Camera.localRotation;
    }

    private void Update()
    {
        //Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GroundCheck();
        CalculateHorizontalMovement();
        CalculateVerticalMovement();

        UpdateRotation();
        HandleShooting();
    }

    private void LateUpdate()
    {
        UpdateMovement();
    }

    private void CalculateHorizontalMovement()
    {
        float horizInput = Input.GetAxisRaw("Horizontal");
        float vertInput = Input.GetAxisRaw("Vertical");

        //Calculate the movement direction
        Vector3 accelerationDir = vertInput * transform.forward;
        accelerationDir += horizInput * transform.right;
        accelerationDir.Normalize();

        //Horizontal Acceleration
        float acceleration = m_Acceleration;
        if (!m_IsGrounded) acceleration = m_AirAcceleration;

        m_AddedVelocity.x += accelerationDir.x * acceleration;
        m_AddedVelocity.z += accelerationDir.z * acceleration;
        
        //Horizontal Friction
        float friction = m_Friction;
        if (!m_IsGrounded) friction = m_AirFriction;

        m_AddedVelocity.x -= m_CurrentVelocity.normalized.x * friction;
        m_AddedVelocity.z -= m_CurrentVelocity.normalized.z * friction;

    }

    private void CalculateVerticalMovement()
    {
        bool isJumping = Input.GetKeyDown(KeyCode.Space);

        //Vertical Acceleration (jumping)
        if (isJumping && m_NumberOfJumps > 0)
        {
            m_CurrentVelocity.y = 0.0f;
            m_AddedVelocity.y = m_JumpSpeed;

            m_NumberOfJumps -= 1;
        }

        //Vertical Friction (Gravity)
        if (m_IsGrounded) m_CurrentVelocity.y = 0.0f;
        if (!m_IsGrounded) m_AddedVelocity.y -= m_Gravity;
    }

    private void UpdateMovement()
    {
        m_CurrentVelocity += m_AddedVelocity;
        m_AddedVelocity = Vector3.zero; //ready for the next frame

        //Horizontal Speed limits
        Vector3 tempVelocity = m_CurrentVelocity.Copy();
        tempVelocity.y = 0.0f;

        float maxHorizontalSpeed = m_MaxGroundSpeed;
        if (!m_IsGrounded) maxHorizontalSpeed = m_MaxAirialSpeed;

        if (Mathf.Abs(tempVelocity.magnitude) > (maxHorizontalSpeed - 1))
        {
            tempVelocity = tempVelocity.normalized * (maxHorizontalSpeed - 1);

            m_CurrentVelocity.x = tempVelocity.x;
            m_CurrentVelocity.z = tempVelocity.z;
        }

        //Avoids "dancing in place"
        if (Mathf.Abs(m_CurrentVelocity.x * Time.deltaTime) < 0.01f) m_CurrentVelocity.x = 0.0f;
        if (Mathf.Abs(m_CurrentVelocity.z * Time.deltaTime) < 0.01f) m_CurrentVelocity.z = 0.0f;

        //Vertical Speed limits
        if (Mathf.Abs(m_CurrentVelocity.y) > (m_MaxAirialSpeed - 1))
        {
            m_CurrentVelocity.y = m_CurrentVelocity.normalized.y * (m_MaxAirialSpeed - 1);
        }

        //Actually move
        transform.position += m_CurrentVelocity * Time.deltaTime;

        m_CurrentSpeed = m_CurrentVelocity.magnitude;
    }

    private void UpdateRotation()
    {
        //Rotate player towards mouse

        float yRot = Input.GetAxis("Mouse X");// *10;
        float xRot = Input.GetAxis("Mouse Y");// *10;

        m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
        m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

        transform.localRotation = m_CharacterTargetRot;
        m_Camera.localRotation = m_CameraTargetRot;
    }

    private void HandleShooting()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));

            m_Gun.Fire(ray);
        }
    }

    private void GroundCheck()
    {
        //Determine length
        float length = m_SkinWidth + Time.deltaTime;

        if (m_CurrentVelocity.y < 0.0f)
            length = (Mathf.Abs(m_CurrentVelocity.y) * Time.deltaTime) + m_SkinWidth;

        //Determine starting pos
        Vector3 startPos = transform.position;
        startPos.y += m_SkinWidth;

        //Cast the ray
        RaycastHit raycastHit;
        Ray ray = new Ray(startPos, Vector3.down);
        bool success = Physics.Raycast(ray, out raycastHit, length);

        m_IsGrounded = success;

        if (m_IsGrounded)
        {
            transform.position = new Vector3(transform.position.x, raycastHit.point.y, transform.position.z);
            m_NumberOfJumps = m_MaxNumberOfJumps;

            //Debug.Log("Collided width : " + raycastHit.collider.gameObject.name);
        }


        Debug.DrawRay(startPos, Vector3.down * length, Color.red);
    }
}
