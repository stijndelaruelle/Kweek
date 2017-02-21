using UnityEngine;
using System.Collections;

public class RidigBodyPlayerController : MonoBehaviour
{
    private Rigidbody m_Rigidbody;

    [SerializeField]
    private Transform m_Camera;

    //Movement
    [SerializeField]
    private float m_Acceleration = 1.0f;

    [SerializeField]
    private float m_Friction = 1.0f;

    [SerializeField]
    private float m_MaxSpeed = 6.0f;

    [SerializeField]
    private float m_JumpSpeed = 8.0f;

    private float m_CurrentSpeed;
    public float CurrentSpeed
    {
        get { return m_CurrentSpeed; }
    }

    private Vector3 m_LastDirection;

    //[SerializeField]
    //private float m_Gravity = 20.0f;

    [SerializeField]
    //private Gun m_Gun;

    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();

        m_CharacterTargetRot = transform.localRotation;
        m_CameraTargetRot = m_Camera.localRotation;

        Cursor.visible = false;
    }

    private void Update()
    {
        //Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Rotate player towards mouse
        UpdateRotation();
        HandleShooting();
    }

    private void FixedUpdate()
    {
        HandleMoving();
    }

    private void UpdateRotation()
    {
        float yRot = Input.GetAxis("Mouse X");// *10;
        float xRot = Input.GetAxis("Mouse Y");// *10;

        m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
        m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

        transform.localRotation = m_CharacterTargetRot;
        m_Camera.localRotation = m_CameraTargetRot;
    }

    private void HandleMoving()
    {
        //Gather input
        float horizInput = Input.GetAxisRaw("Horizontal");
        float vertInput = Input.GetAxisRaw("Vertical");
        bool isJumping = Input.GetKeyDown(KeyCode.Space);

        //Calculate the movement direction
        Vector3 accelerationDir = vertInput * transform.forward;
        accelerationDir += horizInput * transform.right;
        accelerationDir.Normalize();

        //Accelerate
        Vector3 currentVelocity = new Vector3();
        currentVelocity.x += accelerationDir.x * m_Acceleration * Time.deltaTime;
        currentVelocity.z += accelerationDir.z * m_Acceleration * Time.deltaTime;

        //Friction
        currentVelocity.x -= m_Rigidbody.velocity.normalized.x * m_Friction * Time.deltaTime;
        currentVelocity.z -= m_Rigidbody.velocity.normalized.z * m_Friction * Time.deltaTime;

        //Actually move
        m_Rigidbody.AddForce(currentVelocity);

        if (isJumping)
        {
            m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpSpeed, m_Rigidbody.velocity.z);
        }

        //Speed limit
        if (m_Rigidbody.velocity.magnitude > m_MaxSpeed)
        {
            float x = m_Rigidbody.velocity.normalized.x * m_MaxSpeed;
            float z = m_Rigidbody.velocity.normalized.z * m_MaxSpeed;

            m_Rigidbody.velocity = new Vector3(x, m_Rigidbody.velocity.y, z);
        }

        m_CurrentSpeed = m_Rigidbody.velocity.magnitude;
    }

    private void HandleShooting()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));

            //m_Gun.Fire(ray);
        }
    }

}
