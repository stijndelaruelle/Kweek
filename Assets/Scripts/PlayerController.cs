using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, MoveableObject
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

    //temp
    private Vector3 m_PreviousPosition;

    private void Awake()
    {
        m_NumberOfJumps = m_MaxNumberOfJumps;

        m_CharacterTargetRot = transform.localRotation;
        m_CameraTargetRot = m_Camera.localRotation;

        m_PreviousPosition = transform.position.Copy();
    }

    private void Update()
    {
        //Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Movement
        CalculateHorizontalMovement();
        CalculateVerticalMovement();

        UpdateRotation();
        HandleShooting();
    }

    private void LateUpdate()
    {
        UpdateMovement();

        //Collision
        GroundCheck();
        RoofCheck();
        DirectionalCheck();
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
        m_PreviousPosition = transform.position.Copy();
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
        //temp
        float width = 0.5f;
        float height = 2;

        Vector3[] rayPositions = new Vector3[4];
        rayPositions[0] = transform.position - (transform.right * width) + (transform.forward * width);
        rayPositions[1] = transform.position + (transform.right * width) + (transform.forward * width);
        rayPositions[2] = transform.position + (transform.right * width) - (transform.forward * width);
        rayPositions[3] = transform.position - (transform.right * width) - (transform.forward * width);

        for (int i = 0; i < rayPositions.Length; ++i)
        {
            rayPositions[i].y += m_SkinWidth;
        }

        List<RaycastHit> hits = CastRayGrid(rayPositions, Vector3.down, length, 0);

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
        //temp
        float width = 0.5f;
        float height = 2;

        Vector3[] rayPositions = new Vector3[4];
        rayPositions[0] = transform.position - (transform.right * width) + (transform.forward * width);
        rayPositions[1] = transform.position + (transform.right * width) + (transform.forward * width);
        rayPositions[2] = transform.position + (transform.right * width) - (transform.forward * width);
        rayPositions[3] = transform.position - (transform.right * width) - (transform.forward * width);

        for (int i = 0; i < rayPositions.Length; ++i)
        {
            rayPositions[i].y += height - m_SkinWidth;
        }

        List<RaycastHit> hits = CastRayGrid(rayPositions, Vector3.up, length, 0);

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
                transform.position = new Vector3(transform.position.x, closestHit.point.y - height, transform.position.z);
                m_CurrentVelocity.y = 0.0f;
            }
        }
    }

    private void DirectionalCheck()
    {
        Vector3 normDir = m_CurrentVelocity.normalized;
        normDir.y = 0.0f;

        if (normDir == Vector3.zero) normDir = transform.forward;
        
        Vector3 right = new Vector3(-normDir.z, 0.0f, normDir.x); //perpendicular to "forward"

        //---------------------------------------
        // Generate corners for the grid
        //---------------------------------------

        //temp
        float width = 1;
        float height = 2;

        //Bottom Left
        Vector3 bottomLeft = m_PreviousPosition;
        bottomLeft -= right * ((width * 0.5f) - 0.1f);
        bottomLeft.y += 0.1f;

        //Bottom Right
        Vector3 bottomRight = m_PreviousPosition;
        bottomRight += right * ((width * 0.5f) - 0.1f);
        bottomRight.y += 0.1f;

        //Top Left
        Vector3 topLeft = bottomLeft;
        topLeft.y += height - 0.2f;

        //Top Right
        Vector3 topRight = bottomRight;
        topRight.y += height - 0.2f;

        Vector3[] rayPositions = new Vector3[4];
        rayPositions[0] = topLeft;
        rayPositions[1] = topRight;
        rayPositions[2] = bottomLeft;
        rayPositions[3] = bottomRight;

        //---------------------------------------
        // Cast the grid
        //---------------------------------------
        float length = 0.5f; //used to be m_CurrentSpeed * deltaTime. Don't use deltaTime it sometimes spikes randomly. Creating weird wallhugging issues

        if (length <= 0.0f)
            return;

        List<RaycastHit> hits = CastRayGrid(rayPositions, normDir, length, 2);

        //---------------------------------------
        // What happens when we have collision
        //---------------------------------------
        if (hits.Count > 0)
        {
            //Determine the furthest hit
            RaycastHit closestHit = hits[0];
            float closestDistance = (hits[0].point - m_PreviousPosition).sqrMagnitude;

            List<Vector3> uniqueNormals = new List<Vector3>();
            uniqueNormals.Add(hits[0].normal);

            foreach (RaycastHit hit in hits)
            {
                //if (hit.distance < closestHit.distance) { closestHit = hit; }
                float distance = (hit.point - m_PreviousPosition).sqrMagnitude;
                if (distance < closestDistance)
                {
                    closestHit = hit;
                    closestDistance = distance;
                }

                if (uniqueNormals.Contains(hit.normal) == false)
                {
                    uniqueNormals.Add(hit.normal);
                }
            }

            float lengthInWall = (length - closestHit.distance);

            bool doSlide = true;
            if (uniqueNormals.Count > 2) doSlide = false;

            //determine if it's a "stompe" or "sharp" corner.
            //In case of a "stompe" we stop moving. In case of a sharp We slide along the center ray
            if (uniqueNormals.Count == 2)
            {
                float dotNormals = Vector3.Dot(uniqueNormals[0], uniqueNormals[1]);

                //Stompe hoek
                if (dotNormals > 0.0f) { doSlide = false; }
            }

            //Either slide along the perpendicular or back off
            if (doSlide)
            {
                Vector3 perpendicular = new Vector3(-closestHit.normal.z, 0.0f, closestHit.normal.x);
                perpendicular.Normalize();

                //Inverse the perpendicular to make sure we always move forward
                float dot = Vector3.Dot(normDir, perpendicular);
                if (dot < 0.0f) { perpendicular *= -1; }

                //Move backwards a bit  
                transform.position -= (normDir * lengthInWall);

                //Move towards the perpendicular
                //transform.position += perpendicular * (lengthInWall * Mathf.Abs(dot)); //scale from 0 to 1 on how fast we move. Heading straight to a wall is a full stop

                Debug.DrawRay(closestHit.point, normDir * lengthInWall, Color.red, 5.0f);
            }
            else
            {
                //Move backwards a bit
                transform.position -= (normDir * lengthInWall);
            }
        }
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
                        Debug.DrawRay(rayPositions[i], direction * length, new Color(1.0f, 0.0f, 1.0f));

                        returnValues.Add(raycastHits[j]);
                    }
                }
            }

            Debug.DrawRay(rayPositions[i], direction * length, Color.gray);
        }

        return returnValues;
    }

    //MoveableObject
    public void AddVelocity(Vector3 velocity)
    {
        m_ExternalAddedVelocity += velocity;
    }
}
