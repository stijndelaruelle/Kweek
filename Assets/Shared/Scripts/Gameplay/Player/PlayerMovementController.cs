using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void CameraDelegate();
public delegate void JumpDelegate();
public delegate void DuckDelegate(bool isDucking);

public class PlayerMovementController : MonoBehaviour
{
    public enum PlayerState
    {
        Running,
        Walking,
        Ducking,
        Airbourne,
        Death
    }

    [SerializeField]
    private Player m_Player;

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
    private float m_MaxWalkSpeed = 20.0f;
    public float MaxWalkSpeed
    {
        get { return m_MaxWalkSpeed; }
    }

    [SerializeField]
    private float m_MaxRunSpeed = 20.0f;
    public float MaxRunSpeed
    {
        get { return m_MaxRunSpeed; }
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
    private bool m_WasGrounded = false;

    [Header("Control Options")]
    [SerializeField]
    private float m_DefaultMouseSensitivity = 60.0f;

    [Space(10)]
    [Header("Required references")]
    [Space(5)]
    [SerializeField]
    private Transform m_Camera;

    [SerializeField]
    private CharacterController m_CharacterController;
    public CharacterController CharacterController
    {
        get { return m_CharacterController; }
    }

    [SerializeField]
    private CapsuleCollider m_OwnCollider;
    public CapsuleCollider OwnCollider
    {
        get { return m_OwnCollider; }
    }

    private Vector3 m_Velocity;
    public Vector3 Velocity
    {
        get { return m_Velocity; }
        set { m_Velocity = value; }
    }

    private IState m_CurrentState;
    public string CurrentStateString
    {
        get { return m_CurrentState.ToString(); }
    }

    //List of all available states, so we don't have to recreate them all the time.
    private List<IState> m_States;

    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;

    //Events (states can access because they are internal classes)
    public event CameraDelegate UpdateCameraEvent;
    public event DuckDelegate DuckEvent;
    public event JumpDelegate JumpEvent;
    public event JumpDelegate LandEvent;

    private void Awake()
    {
        m_CharacterController.detectCollisions = false;

        //Cache the states, otherwise I kept creating new ones for no reason.
        m_States = new List<IState>();
        m_States.Add(new RunState(this));
        m_States.Add(new WalkState(this));
        m_States.Add(new DuckState(this));
        m_States.Add(new AirborneState(this));
        m_States.Add(new DeathState(this));
    }

    private void Start()
    {
        if (m_Player != null)
        {
            m_Player.DeathEvent += OnPlayerDeath;
            m_Player.RespawnEvent += OnPlayerRespawn;
        }

        m_CharacterTargetRot = transform.localRotation;
        m_CameraTargetRot = m_Camera.localRotation;

        SwitchState(PlayerState.Running);
    }

    private void OnDestroy()
    {
        if (m_Player != null)
        {
            m_Player.DeathEvent -= OnPlayerDeath;
            m_Player.RespawnEvent -= OnPlayerRespawn;
        }
    }

    private void Update()
    {
        if (Time.timeScale == 0.0f)
            return;

        if (m_CurrentState != null)
            m_CurrentState.StateUpdate();

        UpdateGroundedState();

        //Move the player by our velocity every frame
        m_CharacterController.Move(m_Velocity * Time.deltaTime);
    }

    private void UpdateRotation()
    {
        if (Time.timeScale == 0.0f)
            return;

        //Mouse sensitivity
        float mouseSensitivity = m_DefaultMouseSensitivity;
        if (OptionsManager.Instance != null)
        {
            bool success = OptionsManager.Instance.GetOptionAsFloat("MouseSensitivity", out mouseSensitivity);
            if (!success)
                mouseSensitivity = m_DefaultMouseSensitivity;
        }

        //Rotate player towards mouse
        float yRot = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float xRot = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
        transform.localRotation = m_CharacterTargetRot;

        Quaternion prevCameraRot = m_CameraTargetRot;
        m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

        //Lock Y axis (TODO: do this in a nicer way)
        if (m_CameraTargetRot.eulerAngles.z >= 90.0f)
            m_CameraTargetRot = prevCameraRot;

        m_Camera.localRotation = m_CameraTargetRot;

        //Lock vertical camera rotation
        //float dot = Vector3.Dot(transform.forward, m_Camera.transform.forward);
        //if (dot < 0.0f)
        //{
        //    m_CameraTargetRot = Quaternion.Euler(90.0f * Mathf.Sign(m_CameraTargetRot.x), 0f, 0f);
        //    m_Camera.localRotation = m_CameraTargetRot;
        //}

        if (UpdateCameraEvent != null)
            UpdateCameraEvent();
    }

    public IState SwitchState(PlayerState newState)
    {
        int stateID = (int)newState;
        if (stateID >= 0 && stateID < m_States.Count)
        {
            if (m_CurrentState != null)
                m_CurrentState.Exit();

            m_CurrentState = m_States[stateID];
            m_CurrentState.Enter();

            //Debug.Log(newState.ToString() + " STATE!");
        }
        else
        {
            Debug.LogError("INVALID STATE!");
        }

        return m_CurrentState;
    }

    private void OnPlayerDeath()
    {
        SwitchState(PlayerState.Death);
    }

    private void OnPlayerRespawn()
    {
        SwitchState(PlayerState.Running);
    }

    //Ground collision
    private bool AcquiringGround()
    {
        return (m_WasGrounded == false && IsGrounded() == true);
    }

    private bool MaintainingGround()
    {
        return IsGrounded();
    }

    public bool IsGrounded()
    {
        RaycastHit hitInfo;
        bool success = Physics.Raycast(new Ray(transform.position, -transform.up), out hitInfo, 0.2f);

        return success;
    }

    private void UpdateGroundedState()
    {
        m_WasGrounded = IsGrounded();
    }

    //Head collision
    public bool HasHeadCollision()
    {
        RaycastHit hitInfo;
        bool success = Physics.Raycast(new Ray(transform.position + (transform.up * m_CharacterController.height), transform.up), out hitInfo, 0.2f);

        return success;
    }

    //---------------------
    // STATES
    //---------------------

    //Ground states
    public class RunState : IState
    {
        private PlayerMovementController m_Player;

        public RunState(PlayerMovementController player)
        {
            m_Player = player;
        }

        public void Enter() { }
        public void Exit() { }

        public void StateUpdate()
        {
            m_Player.UpdateRotation();

            HandleSwitchingStates();
            HandleHorizontalMovement();
        }

        private void HandleSwitchingStates()
        {
            //Jumping
            bool isJumping = Input.GetKeyDown(KeyCode.Space);
            if (isJumping)
            {
                //On the edge of being dirty(?)
                if (m_Player.HasHeadCollision() == false)
                {
                    AirborneState state = (AirborneState)m_Player.SwitchState(PlayerState.Airbourne);
                    state.Jump();
                    return;
                }
            }

            //Falling
            if (!m_Player.MaintainingGround())
            {
                m_Player.SwitchState(PlayerState.Airbourne);
                return;
            }

            //Ducking
            bool isDucking = Input.GetKey(KeyCode.LeftAlt);
            if (isDucking)
            {
                m_Player.SwitchState(PlayerState.Ducking);
                return;
            }

            //Walking
            bool isWalking = Input.GetKey(KeyCode.LeftShift);
            if (isWalking)
            {
                m_Player.SwitchState(PlayerState.Walking);
                return;
            }
        }

        private void HandleHorizontalMovement()
        {
            float horizInput = Input.GetAxisRaw("Horizontal");
            float vertInput = Input.GetAxisRaw("Vertical");

            //Calculate the movement direction
            Vector3 accelerationDir = vertInput * m_Player.gameObject.transform.forward;
            accelerationDir += horizInput * m_Player.gameObject.transform.right;
            accelerationDir.Normalize();

            //Horizontal friction
            m_Player.Velocity = Vector3.MoveTowards(m_Player.Velocity,
                                        Vector3.zero,
                                        m_Player.Friction * Time.deltaTime); //controller.deltaTime

            //Horizontal Acceleration
            m_Player.Velocity = Vector3.MoveTowards(m_Player.Velocity,
                                                    accelerationDir * m_Player.MaxRunSpeed,
                                                    m_Player.Acceleration * Time.deltaTime); //controller.deltaTime
        }

        public override string ToString()
        {
            return "Running";
        }
    }

    public class WalkState : IState
    {
        private PlayerMovementController m_Player;

        public WalkState(PlayerMovementController player)
        {
            m_Player = player;
        }

        public void Enter() { }
        public void Exit() { }

        public void StateUpdate()
        {
            m_Player.UpdateRotation();

            HandleSwitchingStates();
            HandleHorizontalMovement();
        }

        private void HandleSwitchingStates()
        {
            //Jumping
            bool isJumping = Input.GetKeyDown(KeyCode.Space);
            if (isJumping)
            {
                //On the edge of being dirty(?)
                if (m_Player.HasHeadCollision() == false)
                {
                    AirborneState state = (AirborneState)m_Player.SwitchState(PlayerState.Airbourne);
                    state.Jump();
                    return;
                }
            }

            //Falling
            if (!m_Player.MaintainingGround())
            {
                m_Player.SwitchState(PlayerState.Airbourne);
                return;
            }

            //Ducking
            bool isDucking = Input.GetKey(KeyCode.LeftAlt);
            if (isDucking)
            {
                m_Player.SwitchState(PlayerState.Ducking);
                return;
            }

            //Running
            bool isWalking = Input.GetKey(KeyCode.LeftShift);
            if (isWalking == false)
            {
                m_Player.SwitchState(PlayerState.Running);
                return;
            }
        }

        private void HandleHorizontalMovement()
        {
            float horizInput = Input.GetAxisRaw("Horizontal");
            float vertInput = Input.GetAxisRaw("Vertical");

            //Calculate the movement direction
            Vector3 accelerationDir = vertInput * m_Player.gameObject.transform.forward;
            accelerationDir += horizInput * m_Player.gameObject.transform.right;
            accelerationDir.Normalize();

            //Horizontal friction
            m_Player.Velocity = Vector3.MoveTowards(m_Player.Velocity,
                                        Vector3.zero,
                                        m_Player.Friction * Time.deltaTime); //controller.deltaTime

            //Horizontal Acceleration
            m_Player.Velocity = Vector3.MoveTowards(m_Player.Velocity,
                                                    accelerationDir * m_Player.MaxWalkSpeed,
                                                    m_Player.Acceleration * Time.deltaTime); //controller.deltaTime
        }

        public override string ToString()
        {
            return "Walking";
        }
    }

    public class DuckState : IState
    {
        private PlayerMovementController m_Player;
        private float m_PrevHeight;

        public DuckState(PlayerMovementController player)
        {
            m_Player = player;
        }

        public void Enter()
        {
            if (m_Player.DuckEvent != null)
                m_Player.DuckEvent(true);

            m_PrevHeight = m_Player.CharacterController.height;
            m_Player.CharacterController.height = m_Player.CharacterController.height * 0.5f;
            m_Player.OwnCollider.height = m_Player.CharacterController.height + 0.1f;
            //m_Player.OwnCollider.center = new Vector3(m_Player.OwnCollider.center.x, m_Player.OwnCollider.center.y * 0.5f, m_Player.OwnCollider.center.z);
        }

        public void Exit()
        {
            if (m_Player.DuckEvent != null)
                m_Player.DuckEvent(false);

            m_Player.CharacterController.height = m_Player.CharacterController.height * 2.0f;
            m_Player.OwnCollider.height = m_Player.CharacterController.height + 0.1f;
            //m_Player.OwnCollider.center = new Vector3(m_Player.OwnCollider.center.x, m_Player.OwnCollider.center.y * 2.0f, m_Player.OwnCollider.center.z);
        }

        public void StateUpdate()
        {
            m_Player.UpdateRotation();

            HandleSwitchingStates();
            HandleHorizontalMovement();
        }

        private void HandleSwitchingStates()
        {
            //Jumping
            bool isJumping = Input.GetKeyDown(KeyCode.Space);
            if (isJumping)
            {
                //On the edge of being dirty(?)
                if (CanStopDucking())
                {
                    AirborneState state = (AirborneState)m_Player.SwitchState(PlayerState.Airbourne);
                    state.Jump();
                    return;
                }
            }

            //Falling
            if (!m_Player.MaintainingGround())
            {
                m_Player.SwitchState(PlayerState.Airbourne);
                return;
            }

            //Stop Ducking
            bool isDucking = Input.GetKey(KeyCode.LeftAlt);
            if (isDucking == false)
            {
                //Check if we are even allowed to leave the ducking state
                if (CanStopDucking())
                {
                    m_Player.SwitchState(PlayerState.Walking);
                    return;
                }
            }
        }

        private void HandleHorizontalMovement()
        {
            float horizInput = Input.GetAxisRaw("Horizontal");
            float vertInput = Input.GetAxisRaw("Vertical");

            //Calculate the movement direction
            Vector3 accelerationDir = vertInput * m_Player.gameObject.transform.forward;
            accelerationDir += horizInput * m_Player.gameObject.transform.right;
            accelerationDir.Normalize();

            //Horizontal friction
            m_Player.Velocity = Vector3.MoveTowards(m_Player.Velocity,
                                        Vector3.zero,
                                        m_Player.Friction * Time.deltaTime); //controller.deltaTime

            //Horizontal Acceleration
            m_Player.Velocity = Vector3.MoveTowards(m_Player.Velocity,
                                                    accelerationDir * m_Player.MaxWalkSpeed,
                                                    m_Player.Acceleration * Time.deltaTime); //controller.deltaTime
        }

        private bool CanStopDucking()
        {
            RaycastHit hitInfo;
            bool success = Physics.Raycast(new Ray(m_Player.transform.position + (m_Player.transform.up * m_PrevHeight), m_Player.transform.up), out hitInfo, 0.2f);

            return (!success);
        }

        public override string ToString()
        {
            return "Ducking";
        }
    }

    //Airborne state
    public class AirborneState : IState
    {
        private PlayerMovementController m_Player;
        private int m_NumberOfJumps = 2;

        public AirborneState(PlayerMovementController player)
        {
            m_Player = player;
        }

        public void Enter()
        {
            m_NumberOfJumps = m_Player.MaxNumberOfJumps;
        }

        public void Exit()
        {
        }

        public void StateUpdate()
        {
            m_Player.UpdateRotation();

            float horizInput = Input.GetAxisRaw("Horizontal");
            float vertInput = Input.GetAxisRaw("Vertical");
            bool isJumping = Input.GetKeyDown(KeyCode.Space);

            if (isJumping && m_NumberOfJumps > 0)
            {
                if (m_Player.HasHeadCollision() == false)
                {
                    Jump();
                }
            }

            Vector3 planarMoveDirection = new Vector3(m_Player.Velocity.x, 0.0f, m_Player.Velocity.z); // Math3d.ProjectVectorOnPlane(Vector3.up, m_Player.Velocity);
            Vector3 verticalMoveDirection = m_Player.Velocity - planarMoveDirection;

            if (m_Player.AcquiringGround())
            {
                m_Player.Velocity = planarMoveDirection;
                m_Player.SwitchState(PlayerState.Running);

                if (m_Player.LandEvent != null)
                    m_Player.LandEvent();

                return;
            }

            //Calculate the movement direction
            Vector3 accelerationDir = vertInput * m_Player.gameObject.transform.forward;
            accelerationDir += horizInput * m_Player.gameObject.transform.right;
            accelerationDir.Normalize();


            planarMoveDirection = Vector3.MoveTowards(planarMoveDirection, accelerationDir * m_Player.MaxAirialSpeed, m_Player.AirAcceleration * Time.deltaTime);//controller.deltaTime);
            verticalMoveDirection -= Vector3.up * m_Player.Gravity * Time.deltaTime;

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

            if (m_Player.JumpEvent != null)
                m_Player.JumpEvent();
        }

        public override string ToString()
        {
            return "Airborne";
        }
    }

    //Deathstate
    public class DeathState : IState
    {
        private PlayerMovementController m_Player;

        public DeathState(PlayerMovementController player)
        {
            m_Player = player;
        }

        public void Enter()
        {
            m_Player.OwnCollider.enabled = false;
        }

        public void Exit()
        {
            m_Player.OwnCollider.enabled = true;
        }

        //The player is dead, do absolutely nothing.
        public void StateUpdate() { }

        public override string ToString()
        {
            return "Dead";
        }
    }
}
