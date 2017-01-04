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
        //Horizontal follows later

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

        //Check for collision and alter the added velocity if required
        accelerationDir = DirectionalCheck(accelerationDir, acceleration);

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

        List<RaycastHit> hits = CastRayGrid(rayPositions, Vector3.down, length, 1);

        if (hits.Count > 0)
        {
            //Determine the closest hit
            RaycastHit closestHit = hits[0];

            foreach (RaycastHit hit in hits)
            {
                if (hit.distance < closestHit.distance) { closestHit = hit; }
            }

            if (closestHit.collider != null)
            {
                m_IsGrounded = true;

                if (m_IsGrounded)
                {
                    m_CurrentVelocity.y = 0.0f;
                    transform.position = new Vector3(transform.position.x, closestHit.point.y, transform.position.z);
                    m_NumberOfJumps = m_MaxNumberOfJumps;
                }
            }
        }
        else
        {
            m_IsGrounded = false;
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

        List<RaycastHit> hits = CastRayGrid(rayPositions, Vector3.up, length, 1);

        if (hits.Count > 0)
        {
            //Determine the closest hit
            RaycastHit closestHit = hits[0];

            foreach (RaycastHit hit in hits)
            {
                if (hit.distance < closestHit.distance) { closestHit = hit; }
            }

            if (closestHit.collider != null)
            {
                transform.position = new Vector3(transform.position.x, closestHit.point.y - m_BoxCollider.bounds.size.y, transform.position.z);
                m_CurrentVelocity.y = 0.0f;
            }
        }
    }

    private Vector3 DirectionalCheck(Vector3 accelerationDir, float acceleration)
    {
        Vector3 normDir = accelerationDir;
        normDir.y = 0.0f;

        if (normDir == Vector3.zero) normDir = transform.forward;
        
        Vector3 right = new Vector3(-normDir.z, 0.0f, normDir.x); //perpendicular to "forward"

        //---------------------------------------
        // Generate corners for the grid
        //---------------------------------------

        //Bottom Left
        Vector3 bottomLeft = transform.position.Copy();
        bottomLeft -= right * m_BoxCollider.size.x * 0.5f;
        bottomLeft.y += 0.1f;

        //Bottom Right
        Vector3 bottomRight = bottomLeft;
        bottomLeft += right * m_BoxCollider.size.x;

        //Top Left
        Vector3 topLeft = bottomLeft;
        topLeft.y += m_BoxCollider.size.y - 0.2f;

        //Top Right
        Vector3 topRight = bottomRight;
        topRight.y += m_BoxCollider.size.y;

        Vector3[] rayPositions = new Vector3[4];
        rayPositions[0] = topLeft;
        rayPositions[1] = topRight;
        rayPositions[2] = bottomLeft;
        rayPositions[3] = bottomRight;

        //---------------------------------------
        // Cast the grid
        //---------------------------------------

        Vector3 totalVelocity = m_CurrentVelocity + (accelerationDir * acceleration) + m_AddedVelocity;
        float totalSpeed = totalVelocity.magnitude;

        float length = (totalSpeed * Time.deltaTime);

        if (length <= 0.0f)
            return accelerationDir;

        List<RaycastHit> hits = CastRayGrid(rayPositions, normDir, length, 2);

        //---------------------------------------
        // What happens when we have collision
        //---------------------------------------

        if (hits.Count > 0)
        {
            //Determine the closest hit
            RaycastHit furthestHit = hits[0];

            //Determine the average (unique) normal.
            //Could probably be done more efficient.
            Vector3 averageNormal = Vector3.zero;
            averageNormal += hits[0].normal;

            List<Vector3> uniqueNormals = new List<Vector3>();
            uniqueNormals.Add(hits[0].normal);

            bool doSlide = true; //don't slide if not all the normals are the same

            foreach (RaycastHit hit in hits)
            {
                if (hit.distance > furthestHit.distance) { furthestHit = hit; }

                if (uniqueNormals.Contains(hit.normal) == false)
                {
                    doSlide = false;

                    uniqueNormals.Add(hit.normal);
                    averageNormal += hit.normal;
                }
            }

            averageNormal /= uniqueNormals.Count;

            //Either slide along the perpendicular or back off
            //Not sliding solves corner issues.
            if (doSlide)
            {
                //Move backwards a bit
                transform.position -= normDir * (length - furthestHit.distance);

                Vector3 perpendicular = new Vector3(-averageNormal.z, 0.0f, averageNormal.x);
                perpendicular.Normalize();

                //Inverse the perpendicular to make sure we always move forward
                float dot = Vector3.Dot(transform.forward, perpendicular);
                if (dot < 0.0f) { perpendicular *= -1; }

                //Rotate the current velocity
                m_CurrentVelocity = perpendicular * m_CurrentVelocity.magnitude;

                Debug.DrawRay(furthestHit.point, perpendicular * length, Color.red, 5.0f);

                return perpendicular;
            }
            else
            {
                //Move backwards a bit
                transform.position -= averageNormal * (length - furthestHit.distance);

                m_CurrentVelocity.x = 0.0f;
                m_CurrentVelocity.z = 0.0f;

                Debug.DrawRay(furthestHit.point, averageNormal * length, Color.red, 5.0f);
                return Vector3.zero;
            }
        }

        return accelerationDir;
    }

    private List<RaycastHit> CastRayGrid(Vector3[] corners, Vector3 direction, float length, int divisions = 0)
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
        List<RaycastHit> returnValues = new List<RaycastHit>();

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
                        //Debug.DrawRay(rayPositions[i], direction * length, new Color(1.0f, 0.0f, 1.0f));

                        returnValues.Add(raycastHits[j]);
                    }
                }
            }

           // Debug.DrawRay(rayPositions[i], direction * length, Color.gray);
        }

        return returnValues;
    }



    //MoveableObject
    public void AddVelocity(Vector3 velocity)
    {
        m_ExternalAddedVelocity += velocity;
    }
}
