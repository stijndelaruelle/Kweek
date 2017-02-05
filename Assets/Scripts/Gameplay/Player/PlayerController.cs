using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void JumpDelegate();
public delegate void DuckDelegate(bool isDucking);

public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        Running,
        Walking,
        Ducking,
        Airbourne
    }

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
    public string CurrentState
    {
        get { return m_CurrentState.ToString(); }
    }

    //List of all available states, so we don't have to recreate them all the time.
    private List<IState> m_States;

    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;

    //Events (only unsafe events because the states must be able to call it. Will change when the state machine gets refactored)
    private DuckDelegate m_DuckEvent;
    public DuckDelegate DuckEvent
    {
        get { return m_DuckEvent; }
        set { m_DuckEvent = value; }
    }

    private JumpDelegate m_JumpEvent;
    public JumpDelegate JumpEvent
    {
        get { return m_JumpEvent; }
        set { m_JumpEvent = value; }
    }

    private JumpDelegate m_LandEvent;
    public JumpDelegate LandEvent
    {
        get { return m_LandEvent; }
        set { m_LandEvent = value; }
    }

    private void Awake()
    {
        //Cache the states, otherwise I kept creating new ones for no reason.
        m_States = new List<IState>();
        m_States.Add(new RunState(this));
        m_States.Add(new WalkState(this));
        m_States.Add(new DuckState(this));
        m_States.Add(new AirborneState(this));
    }

    private void Start()
    {
        m_CharacterTargetRot = transform.localRotation;
        m_CameraTargetRot = m_Camera.localRotation;

        SwitchState(PlayerState.Running);
    }

    private void SuperUpdate()
    {
        //Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        UpdateRotation();

        if (m_CurrentState != null)
            m_CurrentState.StateUpdate();

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

    public IState SwitchState(PlayerState newState)
    {
        int stateID = (int)newState;
        if (stateID >= 0 && stateID < m_States.Count)
        {
            if (m_CurrentState != null)
                m_CurrentState.Exit();

            m_CurrentState = m_States[stateID];
            m_CurrentState.Enter();
        }
        else
        {
            Debug.LogError("INVALID STATE!");
        }

        return m_CurrentState;
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

    public bool IsGrounded()
    {
        return MaintainingGround();
    }

    //---------------------
    // STATES
    //---------------------

    //Ground states
    public class RunState : IState
    {
        private PlayerController m_Player;

        public RunState(PlayerController player)
        {
            m_Player = player;
        }

        public void Enter() {}
        public void Exit() {}

        public void StateUpdate()
        {
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
                AirborneState state = (AirborneState)m_Player.SwitchState(PlayerState.Airbourne);
                state.Jump();
                return;
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
        private PlayerController m_Player;

        public WalkState(PlayerController player)
        {
            m_Player = player;
        }

        public void Enter() {}
        public void Exit() {}

        public void StateUpdate()
        {
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
                AirborneState state = (AirborneState)m_Player.SwitchState(PlayerState.Airbourne);
                state.Jump();
                return;
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
        private PlayerController m_Player;

        public DuckState(PlayerController player)
        {
            m_Player = player;
        }

        public void Enter()
        {
            if (m_Player.DuckEvent != null)
                m_Player.DuckEvent(true);

            m_Player.CharacterController.heightScale = 0.5f;
            m_Player.OwnCollider.height = m_Player.OwnCollider.height * 0.5f;
            m_Player.OwnCollider.center = new Vector3(m_Player.OwnCollider.center.x, m_Player.OwnCollider.center.y * 0.5f, m_Player.OwnCollider.center.z);
        }

        public void Exit()
        {
            if (m_Player.DuckEvent != null)
                m_Player.DuckEvent(false);

            m_Player.CharacterController.heightScale = 1.0f;
            m_Player.OwnCollider.height = m_Player.OwnCollider.height * 2.0f;
            m_Player.OwnCollider.center = new Vector3(m_Player.OwnCollider.center.x, m_Player.OwnCollider.center.y * 2.0f, m_Player.OwnCollider.center.z);
        }

        public void StateUpdate()
        {
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
                AirborneState state = (AirborneState)m_Player.SwitchState(PlayerState.Airbourne);
                state.Jump();
                return;
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
                bool headCollision = m_Player.CharacterController.CheckHeadCollision(1.0f);

                if (headCollision == false)
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

        public override string ToString()
        {
            return "Ducking";
        }
    }

    //Airborne state
    public class AirborneState : IState
    {
        private PlayerController m_Player;
        private int m_NumberOfJumps = 2;

        public AirborneState(PlayerController player)
        {
            m_Player = player;
        }

        public void Enter()
        {
            m_Player.CharacterController.DisableClamping();
            m_Player.CharacterController.DisableSlopeLimit();

            m_NumberOfJumps = m_Player.MaxNumberOfJumps;
        }

        public void Exit()
        {
            m_Player.CharacterController.EnableClamping();
            m_Player.CharacterController.EnableSlopeLimit();
        }

        public void StateUpdate()
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

            if (m_Player.JumpEvent != null)
                m_Player.JumpEvent();
        }

        public override string ToString()
        {
            return "Airborne";
        }
    }

}
