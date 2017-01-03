using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, MoveableObject
{
    private enum ColliderPosition
    {
        BottomFrontLeft = 0,
        BottomFrontRight = 1,
        BottomBackRight = 2,
        BottomBackLeft = 3,
        TopFrontLeft = 4,
        TopFrontRight = 5,
        TopBackRight = 6,
        TopBackLeft = 7
    }

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
    private BoxCollider m_BoxCollider;
    private Vector3[] m_ColliderVertices;

    [SerializeField]
    private Gun m_Gun;

    //Exposed to debug
    private Vector3 m_AddedVelocity;
    private Vector3 m_ExternalAddedVelocity;
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

        CalculcateColliderVertices();

        //Collision
        GroundCheck();
        RoofCheck();
        ForwardCheck();
        SideCheck();

        //Movement
        CalculateHorizontalMovement();
        CalculateVerticalMovement();

        UpdateRotation();
        HandleShooting();
    }

    private void LateUpdate()
    {
        UpdateMovement();
    }

    private void CalculcateColliderVertices()
    {
        BoxCollider b = m_BoxCollider;
        m_ColliderVertices = new Vector3[8];

        m_ColliderVertices[(int)ColliderPosition.BottomFrontLeft] = gameObject.transform.TransformPoint(b.center + new Vector3(-b.size.x, -b.size.y, b.size.z) * 0.5f);  //Links voor (onder)
        m_ColliderVertices[(int)ColliderPosition.BottomFrontRight] = gameObject.transform.TransformPoint(b.center + new Vector3(b.size.x, -b.size.y, b.size.z) * 0.5f);  //Rechts voor (onder)
        m_ColliderVertices[(int)ColliderPosition.BottomBackRight] = gameObject.transform.TransformPoint(b.center + new Vector3(b.size.x, -b.size.y, -b.size.z) * 0.5f);  //Rechts achter (onder)
        m_ColliderVertices[(int)ColliderPosition.BottomBackLeft] = gameObject.transform.TransformPoint(b.center + new Vector3(-b.size.x, -b.size.y, -b.size.z) * 0.5f);  //Links achter (onder)

        m_ColliderVertices[(int)ColliderPosition.TopFrontLeft] = gameObject.transform.TransformPoint(b.center + new Vector3(-b.size.x, b.size.y, b.size.z) * 0.5f);      //Links voor (boven)
        m_ColliderVertices[(int)ColliderPosition.TopFrontRight] = gameObject.transform.TransformPoint(b.center + new Vector3(b.size.x, b.size.y, b.size.z) * 0.5f);      //Rechts voor (boven)
        m_ColliderVertices[(int)ColliderPosition.TopBackRight] = gameObject.transform.TransformPoint(b.center + new Vector3(b.size.x, b.size.y, -b.size.z) * 0.5f);      //Rechts achter (boven)
        m_ColliderVertices[(int)ColliderPosition.TopBackLeft] = gameObject.transform.TransformPoint(b.center + new Vector3(-b.size.x, b.size.y, -b.size.z) * 0.5f);      //Links achter (boven)
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
        if (m_CurrentVelocity.magnitude < 0.75f)
        {
            m_CurrentVelocity.x = 0.0f;
            m_CurrentVelocity.z = 0.0f;
        }

        //Vertical Speed limits
        if (Mathf.Abs(m_CurrentVelocity.y) > (m_MaxAirialSpeed - 1))
        {
            m_CurrentVelocity.y = m_CurrentVelocity.normalized.y * (m_MaxAirialSpeed - 1);
        }

        //Actually move
        transform.position += (m_CurrentVelocity + m_ExternalAddedVelocity) * Time.deltaTime;
        m_ExternalAddedVelocity = Vector3.zero;

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

    //Collision
    private void GroundCheck()
    {
        //Determine length
        float length = m_SkinWidth + Time.deltaTime;

        if (m_CurrentVelocity.y < 0.0f)
            length = (Mathf.Abs(m_CurrentVelocity.y) * Time.deltaTime) + m_SkinWidth;

        //Determine starting pos
        Vector3[] rayPositions = new Vector3[4];
        rayPositions[0] = m_ColliderVertices[(int)ColliderPosition.BottomFrontLeft];
        rayPositions[1] = m_ColliderVertices[(int)ColliderPosition.BottomFrontRight];
        rayPositions[2] = m_ColliderVertices[(int)ColliderPosition.BottomBackRight];
        rayPositions[3] = m_ColliderVertices[(int)ColliderPosition.BottomBackLeft];

        for (int i = 0; i < rayPositions.Length; ++i)
        {
            rayPositions[i].y += m_SkinWidth;
        }

        RaycastHit hit = CastRayGrid(rayPositions, Vector3.down, length, 1);

        if (hit.collider != null)
        {
            m_IsGrounded = true;

            if (m_IsGrounded)
            {
                m_CurrentVelocity.y = 0.0f;
                transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
                m_NumberOfJumps = m_MaxNumberOfJumps;
            }
        }
        else
        {
            m_IsGrounded = false;
        }
    }

    private void GroundCheckOld()
    {
        //Determine length
        float length = m_SkinWidth + Time.deltaTime;

        if (m_CurrentVelocity.y < 0.0f)
            length = (Mathf.Abs(m_CurrentVelocity.y) * Time.deltaTime) + m_SkinWidth;

        //Determine starting pos
        Vector3[] rayPositions = new Vector3[9];
        rayPositions[0] = m_ColliderVertices[(int)ColliderPosition.BottomFrontLeft];
        rayPositions[1] = m_ColliderVertices[(int)ColliderPosition.BottomFrontRight];
        rayPositions[2] = m_ColliderVertices[(int)ColliderPosition.BottomBackRight];
        rayPositions[3] = m_ColliderVertices[(int)ColliderPosition.BottomBackLeft];

        rayPositions[4] = (rayPositions[0] + rayPositions[1]) * 0.5f;
        rayPositions[5] = (rayPositions[1] + rayPositions[2]) * 0.5f;
        rayPositions[6] = (rayPositions[2] + rayPositions[3]) * 0.5f;
        rayPositions[7] = (rayPositions[3] + rayPositions[0]) * 0.5f;

        rayPositions[8] = (rayPositions[0] + rayPositions[2]) * 0.5f;

        for (int i = 0; i < rayPositions.Length; ++i)
        {
            rayPositions[i].y += m_SkinWidth;

            //Cast the ray
            Ray ray = new Ray(rayPositions[i], Vector3.down);
            RaycastHit[] raycastHits = Physics.RaycastAll(ray, length);

            //If we hit the ground
            if (raycastHits.Length > 0)
            {
                for (int j = 0; j < raycastHits.Length; ++j)
                {
                    if (raycastHits[j].collider.gameObject != gameObject)
                    {
                        m_IsGrounded = true;

                        if (m_IsGrounded)
                        {
                            m_CurrentVelocity.y = 0.0f;
                            transform.position = new Vector3(transform.position.x, raycastHits[j].point.y, transform.position.z);
                            m_NumberOfJumps = m_MaxNumberOfJumps;

                            Debug.DrawRay(rayPositions[i], Vector3.down * length, Color.red);

                            return;
                        }
                    }
                }
            }

            m_IsGrounded = false;
            Debug.DrawRay(rayPositions[i], Vector3.down * length, Color.gray);
        }
    }

    private void RoofCheck()
    {
        //Determine length
        float length = m_SkinWidth + Time.deltaTime;

        if (m_CurrentVelocity.y > 0.0f)
            length = (Mathf.Abs(m_CurrentVelocity.y) * Time.deltaTime) + m_SkinWidth;

        //Determine starting pos
        Vector3[] rayPositions = new Vector3[9];
        rayPositions[0] = m_ColliderVertices[(int)ColliderPosition.TopFrontLeft];
        rayPositions[1] = m_ColliderVertices[(int)ColliderPosition.TopFrontRight];
        rayPositions[2] = m_ColliderVertices[(int)ColliderPosition.TopBackRight];
        rayPositions[3] = m_ColliderVertices[(int)ColliderPosition.TopBackLeft];

        for (int i = 0; i < rayPositions.Length; ++i)
        {
            rayPositions[i].y -= m_SkinWidth;
        }

        RaycastHit hit = CastRayGrid(rayPositions, Vector3.up, length, 1);

        if (hit.collider != null)
        {
            transform.position = new Vector3(transform.position.x, hit.point.y - m_BoxCollider.bounds.size.y, transform.position.z);
            m_CurrentVelocity.y = 0.0f;
        }

    }

    private void RoofCheckOld()
    {
        //Determine length
        float length = m_SkinWidth + Time.deltaTime;

        if (m_CurrentVelocity.y > 0.0f)
            length = (Mathf.Abs(m_CurrentVelocity.y) * Time.deltaTime) + m_SkinWidth;

        //Determine starting pos
        Vector3[] rayPositions = new Vector3[9];
        rayPositions[0] = m_ColliderVertices[(int)ColliderPosition.TopFrontLeft];
        rayPositions[1] = m_ColliderVertices[(int)ColliderPosition.TopFrontRight];
        rayPositions[2] = m_ColliderVertices[(int)ColliderPosition.TopBackRight];
        rayPositions[3] = m_ColliderVertices[(int)ColliderPosition.TopBackLeft];

        rayPositions[4] = (rayPositions[0] + rayPositions[1]) * 0.5f;
        rayPositions[5] = (rayPositions[1] + rayPositions[2]) * 0.5f;
        rayPositions[6] = (rayPositions[2] + rayPositions[3]) * 0.5f;
        rayPositions[7] = (rayPositions[3] + rayPositions[0]) * 0.5f;

        rayPositions[8] = (rayPositions[0] + rayPositions[2]) * 0.5f;

        for (int i = 0; i < rayPositions.Length; ++i)
        {
            rayPositions[i].y -= m_SkinWidth;

            //Cast the ray
            Ray ray = new Ray(rayPositions[i], Vector3.up);
            RaycastHit[] raycastHits = Physics.RaycastAll(ray, length);

            //If we hit the ceiling
            if (raycastHits.Length > 0)
            {
                for (int j = 0; j < raycastHits.Length; ++j)
                {
                    if (raycastHits[j].collider.gameObject != gameObject)
                    {
                        transform.position = new Vector3(transform.position.x, raycastHits[j].point.y - m_BoxCollider.bounds.size.y, transform.position.z);
                        m_CurrentVelocity.y = 0.0f;

                        Debug.DrawRay(rayPositions[i], Vector3.up * length, Color.blue);
                        return;
                    }
                }
            }

            Debug.DrawRay(rayPositions[i], Vector3.up * length, Color.gray);
        }
    }

    private void ForwardCheck()
    {
        Vector3[] rayPositions = new Vector3[4];

        //Determine if we are moving forward or backward
        float dot = Vector3.Dot(transform.forward, m_CurrentVelocity);

        if (dot >= 0.0f)
        {
            rayPositions[0] = m_ColliderVertices[(int)ColliderPosition.TopFrontLeft];
            rayPositions[1] = m_ColliderVertices[(int)ColliderPosition.TopFrontRight];
            rayPositions[2] = m_ColliderVertices[(int)ColliderPosition.BottomFrontLeft];
            rayPositions[3] = m_ColliderVertices[(int)ColliderPosition.BottomFrontRight];

            //make sure we don't interact with the side rays
            rayPositions[0] += transform.right * 0.1f;
            rayPositions[1] -= transform.right * 0.1f;
            rayPositions[2] += transform.right * 0.1f;
            rayPositions[3] -= transform.right * 0.1f;
        }
        else
        {
            rayPositions[0] = m_ColliderVertices[(int)ColliderPosition.TopBackLeft];
            rayPositions[1] = m_ColliderVertices[(int)ColliderPosition.TopBackRight];
            rayPositions[2] = m_ColliderVertices[(int)ColliderPosition.BottomBackLeft];
            rayPositions[3] = m_ColliderVertices[(int)ColliderPosition.BottomBackRight];

            //make sure we don't interact with the side rays
            rayPositions[0] -= transform.right * 0.1f;
            rayPositions[1] += transform.right * 0.1f;
            rayPositions[2] -= transform.right * 0.1f;
            rayPositions[3] += transform.right * 0.1f;
        }

        HorizontalCheck(rayPositions, transform.forward);
    }

    private void SideCheck()
    {
        Vector3[] rayPositions = new Vector3[4];

        //Determine if we are moving forward or backward
        float dot = Vector3.Dot(transform.right, m_CurrentVelocity);

        if (dot >= 0.0f)
        {
            rayPositions[0] = m_ColliderVertices[(int)ColliderPosition.TopBackRight];
            rayPositions[1] = m_ColliderVertices[(int)ColliderPosition.TopFrontRight];
            rayPositions[2] = m_ColliderVertices[(int)ColliderPosition.BottomBackRight];
            rayPositions[3] = m_ColliderVertices[(int)ColliderPosition.BottomFrontRight];
        }
        else
        {
            rayPositions[0] = m_ColliderVertices[(int)ColliderPosition.TopFrontLeft];
            rayPositions[1] = m_ColliderVertices[(int)ColliderPosition.TopBackLeft];
            rayPositions[2] = m_ColliderVertices[(int)ColliderPosition.BottomFrontLeft];
            rayPositions[3] = m_ColliderVertices[(int)ColliderPosition.BottomBackLeft];
        }

        //make sure we don't interact with the forward rays
        rayPositions[0] += transform.forward * 0.1f;
        rayPositions[1] -= transform.forward * 0.1f;
        rayPositions[2] += transform.forward * 0.1f;
        rayPositions[3] -= transform.forward * 0.1f;

        HorizontalCheck(rayPositions, transform.right);
    }

    private void HorizontalCheck(Vector3[] rayPositions, Vector3 direction)
    {
        //Determine length
        float length = m_SkinWidth + Time.deltaTime;

        if (m_CurrentSpeed > 0.0f)
            length = (m_CurrentSpeed * Time.deltaTime) + m_SkinWidth;

        //Determine if we are moving forward or backward
        float dot = Vector3.Dot(direction, m_CurrentVelocity);
        float backwardMod = Mathf.Sign(dot);

        //Adjust the positions slightly (so they don't interfere with the above & below rays)
        rayPositions[0].y -= 0.1f;
        rayPositions[1].y -= 0.1f;
        rayPositions[2].y += 0.1f;
        rayPositions[3].y += 0.1f;

        for (int i = 0; i < rayPositions.Length; ++i)
        {
            rayPositions[i] -= backwardMod * direction * m_SkinWidth;
        }

        Vector3 hitRay;
        RaycastHit hit = CastRayGrid(rayPositions, backwardMod * direction, length, out hitRay, 2);

        if (hit.collider != null)
        {
            //Alter the position slightly, so we are not in the wall anymore
            transform.position -= (hitRay + (backwardMod * direction) * length) - hit.point;

            //Alter the rotation of our character so he can move along the wall
            Vector3 normal = hit.normal;
            Vector3 perpendicular = new Vector3(-normal.z, 0.0f, normal.x);

            //Inverse the perpendicular to make sure we always move forward
            dot = Vector3.Dot(direction, perpendicular);

            Debug.Log(dot);
            if (dot < 0.0f) { perpendicular *= -1; }

            transform.localRotation = Quaternion.LookRotation(perpendicular, Vector3.up);

            Debug.Log(hit.collider.gameObject.name);
        }
    }

    private void ForwardCheckOld()
    {
        //Determine length
        float length = m_SkinWidth + Time.deltaTime;

        if (m_CurrentSpeed > 0.0f)
            length = (m_CurrentSpeed * Time.deltaTime) + m_SkinWidth;

        //Determine starting pos
        Vector3[] rayPositions = new Vector3[15];
        rayPositions[0] = m_ColliderVertices[(int)ColliderPosition.TopFrontLeft];
        rayPositions[1] = m_ColliderVertices[(int)ColliderPosition.TopFrontRight];
        rayPositions[2] = m_ColliderVertices[(int)ColliderPosition.BottomFrontLeft];
        rayPositions[3] = m_ColliderVertices[(int)ColliderPosition.BottomFrontRight];

        //Adjust the positions slightly (so they don't interfere with the above & below rays)
        rayPositions[0].y -= 0.1f;
        rayPositions[1].y -= 0.1f;
        rayPositions[2].y += 0.1f;
        rayPositions[3].y += 0.1f;
  
        rayPositions[4] = (rayPositions[0] + rayPositions[1]) * 0.5f;   //Top center
        rayPositions[5] = (rayPositions[2] + rayPositions[3]) * 0.5f;   //Bottom center
        rayPositions[6] = (rayPositions[0] + rayPositions[2]) * 0.5f;   //Left center
        rayPositions[7] = (rayPositions[1] + rayPositions[3]) * 0.5f;   //Right center

        rayPositions[8] = (rayPositions[6] + rayPositions[7]) * 0.5f;   //center

        //Extra left
        rayPositions[9] = (rayPositions[0] + rayPositions[6]) * 0.5f;
        rayPositions[10] = (rayPositions[2] + rayPositions[6]) * 0.5f;

        //Extra right
        rayPositions[11] = (rayPositions[1] + rayPositions[7]) * 0.5f;
        rayPositions[12] = (rayPositions[3] + rayPositions[7]) * 0.5f;

        //Extra center rays
        rayPositions[13] = (rayPositions[9] + rayPositions[11]) * 0.5f;
        rayPositions[14] = (rayPositions[10] + rayPositions[12]) * 0.5f;

        for (int i = 0; i < rayPositions.Length; ++i)
        {
            rayPositions[i] -= transform.forward * m_SkinWidth;

            //Cast the ray
            Ray ray = new Ray(rayPositions[i], transform.forward);
            RaycastHit[] raycastHits = Physics.RaycastAll(ray, length);

            //If we hit the ceiling
            if (raycastHits.Length > 0)
            {
                for (int j = 0; j < raycastHits.Length; ++j)
                {
                    if (raycastHits[j].collider.gameObject != gameObject)
                    {
                        //Alter the position slightly, so we are not in the wall anymore
                        transform.position -= (rayPositions[i] + transform.forward * length) - raycastHits[j].point;

                        //Alter the rotation of our character so he can move along the wall
                        Vector3 normal = raycastHits[j].normal;
                        Vector3 perpendicular = new Vector3(-normal.z, 0.0f, normal.x);

                        //Inverse the perpendicular to make sure we always move forward
                        float dot = Vector3.Dot(transform.forward, perpendicular);
                        if (dot < 0.0f) { perpendicular *= -1; }

                        transform.localRotation = Quaternion.LookRotation(perpendicular, Vector3.up);

                        Debug.Log(raycastHits[j].collider.gameObject.name);
                        Debug.DrawRay(rayPositions[i], transform.forward * length, new Color(1.0f, 0.0f, 1.0f));
                        return;
                    }
                }
            }

            Debug.DrawRay(rayPositions[i], transform.forward * length, new Color(1.0f, 0.0f, 1.0f));
        }
    }


    private RaycastHit CastRayGrid(Vector3[] corners, Vector3 direction, float length, int divisions = 0)
    {
        Vector3 hitRay;
        return CastRayGrid(corners, direction, length, out hitRay);
    }

    private RaycastHit CastRayGrid(Vector3[] corners, Vector3 direction, float length, out Vector3 hitRay, int divisions = 0)
    {
        List<Vector3> rayPositions = new List<Vector3>();

        //Add the corners
        rayPositions.Add(corners[0]); //0
        rayPositions.Add(corners[1]); //1
        rayPositions.Add(corners[2]); //2
        rayPositions.Add(corners[3]); //3

        //TODO make this generic so it works for any kind of subdivisions. For now this is overkill

        //Subdivision 1
        if (divisions > 0)
        {
            rayPositions.Add((rayPositions[0] + rayPositions[1]) * 0.5f);   //Top center (4)
            rayPositions.Add((rayPositions[2] + rayPositions[3]) * 0.5f);   //Bottom center (5)
            rayPositions.Add((rayPositions[0] + rayPositions[2]) * 0.5f);   //Left center (6)
            rayPositions.Add((rayPositions[1] + rayPositions[3]) * 0.5f);   //Right center (7)

            rayPositions.Add((rayPositions[6] + rayPositions[7]) * 0.5f);   //Center (8)
        }

        if (divisions > 1)
        {
            //Extra left
            rayPositions.Add((rayPositions[0] + rayPositions[6]) * 0.5f);    //9
            rayPositions.Add((rayPositions[2] + rayPositions[6]) * 0.5f);   //10

            //Extra right
            rayPositions.Add((rayPositions[1] + rayPositions[7]) * 0.5f);   //11
            rayPositions.Add((rayPositions[3] + rayPositions[7]) * 0.5f);   //12

            //Extra center rays
            rayPositions.Add((rayPositions[9] + rayPositions[11]) * 0.5f);  //13
            rayPositions.Add((rayPositions[10] + rayPositions[12]) * 0.5f); //14
        }

        //Actually cast those rays
        for (int i = 0; i < rayPositions.Count; ++i)
        {
            //Cast the ray
            Ray ray = new Ray(rayPositions[i], direction);
            RaycastHit[] raycastHits = Physics.RaycastAll(ray, length);

            //If we hit the ceiling
            if (raycastHits.Length > 0)
            {
                for (int j = 0; j < raycastHits.Length; ++j)
                {
                    if (raycastHits[j].collider.gameObject != gameObject)
                    {
                        Debug.DrawRay(rayPositions[i], direction * length, new Color(1.0f, 0.0f, 1.0f));

                        hitRay = rayPositions[i];
                        return raycastHits[j];
                    }
                }
            }

            Debug.DrawRay(rayPositions[i], direction * length, Color.gray);
        }

        hitRay = Vector3.zero;
        return new RaycastHit();
    }

    public void AddVelocity(Vector3 velocity)
    {
        m_ExternalAddedVelocity += velocity;
    }
}
