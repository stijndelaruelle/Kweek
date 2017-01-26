using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Base Movement Settings")]
    [SerializeField]
    private float m_Acceleration = 1.0f;
    public float Acceleration
    {
        get { return m_Acceleration; }
    }

    [SerializeField]
    private float m_AirAcceleration = 1.0f;
    public float AirAcceleration
    {
        get { return m_AirAcceleration; }
    }

    [SerializeField]
    private float m_Friction = 1.0f;
    public float Friction
    {
        get { return m_Friction; }
    }

    [SerializeField]
    private float m_AirFriction = 1.0f;
    public float AirFriction
    {
        get { return m_AirFriction; }
    }

    [SerializeField]
    private float m_MaxGroundSpeed = 20.0f;
    public float MaxGroundSpeed
    {
        get { return m_MaxGroundSpeed; }
    }

    [SerializeField]
    private float m_MaxAirialSpeed = 30.0f;
    public float MaxAirialSpeed
    {
        get { return m_MaxAirialSpeed; }
    }

    [SerializeField]
    private float m_JumpHeight = 8.0f;
    public float JumpHeight
    {
        get { return m_JumpHeight; }
    }

    [SerializeField]
    private float m_Gravity = 1.0f;
    public float Gravity
    {
        get { return m_Gravity; }
    }

    [Header("Movement Options")]
    [SerializeField]
    private int m_MaxNumberOfJumps = 2;
    public int MaxNumberOfJumps
    {
        get { return m_MaxNumberOfJumps; }
    }

    [Space(10)]
    [Header ("Required references")]
    [Space(5)]

    [SerializeField]
    private Transform m_Camera;

    [SerializeField]
    private SuperCharacterController m_CharacterController;
    public SuperCharacterController CharacterController
    {
        get { return m_CharacterController; }
    }

    [SerializeField]
    private IDamageableObject m_DamageableObject;
    public IDamageableObject DamageableObject
    {
        get { return m_DamageableObject; }
    }

    [SerializeField]
    private WeaponArsenal m_WeaponArsenal;
    public WeaponArsenal WeaponArsenal
    {
        get { return m_WeaponArsenal; }
    }


    private Vector3 m_Velocity;
    public Vector3 Velocity
    {
        get { return m_Velocity; }
        set { m_Velocity = value; }
    }

    private IState m_CurrentState;
    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;

    private void Start()
    {
        m_DamageableObject.DeathEvent += OnDeath;

        m_CharacterTargetRot = transform.localRotation;
        m_CameraTargetRot = m_Camera.localRotation;

        SwitchState(new IdleState(this));
    }

    private void OnDestroy()
    {
        if (m_DamageableObject != null)
            m_DamageableObject.DeathEvent -= OnDeath;
    }

    private void Update()
    {
        //Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        UpdateRotation();

        if (m_CurrentState != null)
            m_CurrentState.Update();

        //Move the player by our velocity every frame
        transform.position += m_Velocity * Time.deltaTime; //m_CharacterController.deltaTime;
    }

    private void UpdateRotation()
    {
        //Rotate player towards mouse

        float yRot = Input.GetAxis("Mouse X");// *10;
        float xRot = Input.GetAxis("Mouse Y");// *10;

        //if (m_LockVerticalRotation)
        //    xRot = 0.0f;

        //if (m_LockHorizontalRotation)
        //    yRot = 0.0f;

        m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
        transform.localRotation = m_CharacterTargetRot;

        m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);
        m_Camera.localRotation = m_CameraTargetRot;

        //Lock vertical camera rotation
        float dot = Vector3.Dot(transform.forward, m_Camera.transform.forward);
        if (dot < 0.0f)
        {
            m_CameraTargetRot = Quaternion.Euler(90.0f * Mathf.Sign(m_CameraTargetRot.x), 0f, 0f);
            m_Camera.localRotation = m_CameraTargetRot;
        }
    }

    private void OnDeath()
    {
        Debug.Log("THE PLAYER DIED!");
    }

    public void SwitchState(IState newState)
    {
        if (m_CurrentState != null)
            m_CurrentState.Exit();

        m_CurrentState = newState;

        m_CurrentState.Enter();
    }


    //Character controller
    private bool AcquiringGround()
    {
        return m_CharacterController.currentGround.IsGrounded(false, 0.01f);
    }

    private bool MaintainingGround()
    {
        return m_CharacterController.currentGround.IsGrounded(true, 0.5f);
    }



    //---------------------
    // STATES
    //---------------------
    public class IdleState : IState
    {
        private Player m_Player;

        public IdleState(Player player)
        {
            m_Player = player;
        }

        public void Enter()
        {
            Debug.Log("Entered IDLE state");

            m_Player.CharacterController.EnableSlopeLimit();
            m_Player.CharacterController.EnableClamping();
        }

        public void Exit()
        {

        }

        public void Update()
        {
            // Run every frame we are in the idle state
            float horizInput = Input.GetAxisRaw("Horizontal");
            float vertInput = Input.GetAxisRaw("Vertical");
            bool isJumping = Input.GetKeyDown(KeyCode.Space);

            if (isJumping)
            {
                AirborneState state = new AirborneState(m_Player);
                m_Player.SwitchState(state);
                state.Jump();
                return;
            }

            if (!m_Player.MaintainingGround())
            {
                m_Player.SwitchState(new AirborneState(m_Player));
                return;
            }

            if (horizInput != 0.0f || vertInput != 0.0f)
            {
                m_Player.SwitchState(new WalkState(m_Player));
                return;
            }

            // Apply friction to slow us to a halt
            m_Player.Velocity = Vector3.MoveTowards(m_Player.Velocity,
                                                    Vector3.zero,
                                                    m_Player.Friction * Time.deltaTime); //controller.deltaTime
        }
    }

    public class WalkState : IState
    {
        private Player m_Player;

        public WalkState(Player player)
        {
            m_Player = player;
        }

        public void Enter()
        {
            Debug.Log("Entered WALK state");
        }

        public void Exit()
        {

        }

        public void Update()
        {
            float horizInput = Input.GetAxisRaw("Horizontal");
            float vertInput = Input.GetAxisRaw("Vertical");
            bool isJumping = Input.GetKeyDown(KeyCode.Space);

            if (isJumping)
            {
                AirborneState state = new AirborneState(m_Player);
                m_Player.SwitchState(state);
                state.Jump();
                return;
            }

            if (!m_Player.MaintainingGround())
            {
                m_Player.SwitchState(new AirborneState(m_Player));
                return;
            }

            if (horizInput == 0.0f && vertInput == 0.0f)
            {
                m_Player.SwitchState(new IdleState(m_Player));
                return;
            }

            HandleHorizontalMovement(horizInput, vertInput);
        }

        private void HandleHorizontalMovement(float horizInput, float vertInput)
        {
            //Calculate the movement direction
            Vector3 accelerationDir = vertInput * m_Player.gameObject.transform.forward;
            accelerationDir += horizInput * m_Player.gameObject.transform.right;
            accelerationDir.Normalize();

            //Horizontal Acceleration
            m_Player.Velocity = Vector3.MoveTowards(m_Player.Velocity,
                                                    accelerationDir * m_Player.MaxGroundSpeed,
                                                    m_Player.Acceleration * Time.deltaTime); //controller.deltaTime
        }
    }

    public class AirborneState : IState
    {
        private Player m_Player;
        private int m_NumberOfJumps = 2;

        public AirborneState(Player player)
        {
            m_Player = player;
        }

        public void Enter()
        {
            Debug.Log("Entered AIRBORNE state");
            m_Player.CharacterController.DisableClamping();
            m_Player.CharacterController.DisableSlopeLimit();

            m_NumberOfJumps = m_Player.MaxNumberOfJumps;
        }

        public void Exit()
        {

        }

        public void Update()
        {
            float horizInput = Input.GetAxisRaw("Horizontal");
            float vertInput = Input.GetAxisRaw("Vertical");
            bool isJumping = Input.GetKeyDown(KeyCode.Space);

            if (isJumping && m_NumberOfJumps > 0)
            {
                Jump();
            }

            Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(m_Player.CharacterController.up, m_Player.Velocity);
            Vector3 verticalMoveDirection = m_Player.Velocity - planarMoveDirection;

            //if (Vector3.Angle(verticalMoveDirection, m_Player.CharacterController.up) > 90 && m_Player.AcquiringGround())
            //{
            if (m_Player.AcquiringGround())
            {
                m_Player.Velocity = planarMoveDirection;
                m_Player.SwitchState(new IdleState(m_Player));
                return;
            }

            //Calculate the movement direction
            Vector3 accelerationDir = vertInput * m_Player.gameObject.transform.forward;
            accelerationDir += horizInput * m_Player.gameObject.transform.right;
            accelerationDir.Normalize();


            planarMoveDirection = Vector3.MoveTowards(planarMoveDirection, accelerationDir * m_Player.MaxAirialSpeed, m_Player.AirAcceleration * Time.deltaTime);//controller.deltaTime);
            verticalMoveDirection -= m_Player.CharacterController.up * m_Player.Gravity * Time.deltaTime; //controller.deltaTime;

            m_Player.Velocity = planarMoveDirection + verticalMoveDirection;
        }

        public void Jump()
        {
            if (m_NumberOfJumps <= 0)
                return;

            //Calculate jump height
            float height = Mathf.Sqrt(2 * m_Player.JumpHeight * m_Player.Gravity);

            //Reset all the Y velocity & jump!
            m_Player.Velocity = new Vector3(m_Player.Velocity.x, height, m_Player.Velocity.z);

            m_NumberOfJumps -= 1;
        }
    }
}
