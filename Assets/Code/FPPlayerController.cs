using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FPPlayerController : MonoBehaviour
{
    float m_Yaw;
    float m_Pitch;
    public float m_YawRotationalSpeed;
    public float m_PitchRotationalSpeed;
    
    public float m_MinPitch;
    public float m_MaxPitch;

    public Transform m_PitchController;
    public bool m_UseYawInverted;
    public bool m_UsePitchInverted;

    public CharacterController m_CharacterController;
    public float m_Speed;
    public float m_FastSpeedMultiplier = 1.5f;

    public Portal m_BluePortal;
    public Portal m_OrangePortal;

    [Header("Key controls")]
    public KeyCode m_LeftKeyCode;
    public KeyCode m_RightKeyCode;
    public KeyCode m_UpKeyCode;
    public KeyCode m_DownKeyCode;
    public KeyCode m_JumpKeyCode = KeyCode.Space;
    public KeyCode m_RunKeyCode = KeyCode.LeftShift;
    public KeyCode m_DebugLockAngleKeyCode = KeyCode.I;
    public KeyCode m_DebugLockKeyCode = KeyCode.O;
    public KeyCode m_AttachObjectKeyCode;
    bool m_AngleLocked = false;
    bool m_AimLocked = true;

    [Header("Shoot")]
    public float m_MaxShootDistance = 50.0f;
    public LayerMask m_ShootingLayerMask;

    float m_Life;
    float m_Shield;
    Vector3 m_StartPosition;
    Quaternion m_StartRotation;

    public Sprite[] m_activePortalsImages;
    public Image m_hudPortalsImage;

    [Header("Camera")]
    public Camera m_Camera;
    public float m_NormalMovementFOV = 60.0f;
    public float m_RunMovementFOV= 80.0f;

    public float m_VerticalSpeed = 0.0f;
    bool m_OnGround = true;

    public float m_JumpSpeed = 10.0f;
    bool m_Shooting = false;

    public float m_OffsetPortal = 1.5f;

    public Vector3 m_Direction;
    [Range (0.0f, 60.0f)]
    public float m_AngleToEnterPortalInDegrees;


    [Header ("AttachingObject")]
    public Transform m_AttachingPosition;
    Rigidbody m_ObjectAttached;
    bool m_AttachingObject = false;
    public float m_AttachingObjectSpeed = 3.0f;
    Quaternion m_AttachingObjectStartRotation;
    public float m_MaxDistanceToAttachObject = 10.0f;
    public LayerMask m_AttachingObjectLayerMask;
    public float m_AttachedObjectThrowForce = 75.0f;

    void Start()
    {
        m_Yaw = transform.rotation.y;
        m_Pitch = m_PitchController.localRotation.x;
        Cursor.lockState = CursorLockMode.Locked;
        m_AimLocked = Cursor.lockState == CursorLockMode.Locked;
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
        m_BluePortal.gameObject.SetActive(false);
        m_OrangePortal.gameObject.SetActive(false);
    }

    #if UNITY_EDITOR
    void UpdateInputDebug()
    {
        if (Input.GetKeyDown(m_DebugLockAngleKeyCode))
            m_AngleLocked = !m_AngleLocked;
        if (Input.GetKeyDown(m_DebugLockKeyCode))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
            m_AimLocked = Cursor.lockState == CursorLockMode.Locked;
        }
        
    }
#endif

    void Update()
    {

#if UNITY_EDITOR
    UpdateInputDebug();
#endif
        Vector3 l_RightDirection = transform.right;
        Vector3 l_ForwardDirection = transform.forward;
        m_Direction = Vector3.zero;
        float l_Speed = m_Speed;
        float l_MouseX = Input.GetAxis("Mouse X");
        float l_MouseY = Input.GetAxis("Mouse Y");
#if UNITY_EDITOR
        if (m_AngleLocked)
        {
            l_MouseX = 0.0f;
            l_MouseY = 0.0f;
        }
#endif

        if (Input.GetKey(m_UpKeyCode))
        {
            m_Direction = l_ForwardDirection;
        }
        if (Input.GetKey(m_DownKeyCode))
        {
            m_Direction = -l_ForwardDirection;
        }
        if (Input.GetKey(m_RightKeyCode))
        {
            m_Direction += l_RightDirection;
        }
        if (Input.GetKey(m_LeftKeyCode))
        {
            m_Direction -= l_RightDirection;
        }
        if (Input.GetKey(m_JumpKeyCode) && m_OnGround)
        {
            m_VerticalSpeed = m_JumpSpeed;

        }
        float l_FOV = m_NormalMovementFOV;
        if (Input.GetKey(m_RunKeyCode))
        {
            l_Speed = m_Speed*m_FastSpeedMultiplier;
            l_FOV = m_RunMovementFOV;

        }
        m_Camera.fieldOfView = l_FOV;

        
        
        m_Direction.Normalize();
        Vector3 l_Movement = m_Direction * m_Speed * Time.deltaTime;

        m_Yaw = m_Yaw + l_MouseX * m_YawRotationalSpeed*Time.deltaTime*(m_UseYawInverted ? -1.0f : 1.0f);
        m_Pitch = m_Pitch + l_MouseY * m_PitchRotationalSpeed * Time.deltaTime * (m_UsePitchInverted ? -1.0f : 1.0f);
        m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch);

        transform.rotation = Quaternion.Euler(0.0f, m_Yaw, 0.0f);
        m_PitchController.localRotation = Quaternion.Euler(m_Pitch, 0.0f, 0.0f);
        

        m_VerticalSpeed = m_VerticalSpeed + Physics.gravity.y * Time.deltaTime;
        l_Movement.y = m_VerticalSpeed * Time.deltaTime;


        CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Movement);

        if ((l_CollisionFlags & CollisionFlags.Above) != 0 && m_VerticalSpeed>0.0f)
        {
            m_VerticalSpeed = 0.0f;
        }
        if ((l_CollisionFlags & CollisionFlags.Below)!=0)
        {
            m_VerticalSpeed = 0.0f;
            m_OnGround = true;
        }
        else
        {
            m_OnGround = false;
        }

        if (Input.GetKeyDown(m_AttachObjectKeyCode) && CanAttachObject())
        {
            AttachObject();
        }
        if (m_ObjectAttached)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ThrowAttachedObject(m_AttachedObjectThrowForce);
            }
            if (Input.GetMouseButtonDown(1))
            {
                ThrowAttachedObject(0.0f);
            }
        }
        else if(!m_AttachingObject)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Shoot(m_BluePortal);
            }
            if (Input.GetMouseButtonDown(1))
            {
                Shoot(m_OrangePortal);
            }
        }
        

        if (m_AttachingObject)
        {
            UpdateAttachedObject();
        }
    }

    void Shoot(Portal _Portal)
    {
        Vector3 l_Position;
        Vector3 l_Normal;
        if(_Portal.isValidPosition(m_Camera.transform.position, m_Camera.transform.forward, m_MaxShootDistance, m_ShootingLayerMask, out l_Position, out l_Normal))
        {
            _Portal.gameObject.SetActive(true);
        }
        else
        {
            _Portal.gameObject.SetActive(false);
        }

        if (m_BluePortal.gameObject.activeSelf)
        {
            if (m_OrangePortal.gameObject.activeSelf)
            {
                m_hudPortalsImage.sprite = m_activePortalsImages[0];
            }
            else
            {
                m_hudPortalsImage.sprite = m_activePortalsImages[1];
            }
        }
        else
        {
            if (m_OrangePortal.gameObject.activeSelf)
            {
                m_hudPortalsImage.sprite = m_activePortalsImages[2];
            }
            else
            {
                m_hudPortalsImage.sprite = m_activePortalsImages[3];
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Portal")
        {
            Portal l_Portal = other.GetComponent<Portal>();
            if(Vector3.Dot(l_Portal.transform.forward, -m_Direction) > Mathf.Cos(m_AngleToEnterPortalInDegrees * Mathf.Deg2Rad))
                Teleport(l_Portal);
        }
    }

    void Teleport(Portal _Portal)
    {
        Vector3 l_LocalPosition = _Portal.m_OtherPortalTransform.InverseTransformPoint(transform.position);
        Vector3 l_LocalDirection = _Portal.m_OtherPortalTransform.transform.InverseTransformDirection(transform.forward);
        Vector3 l_LocalDirectionMovement = _Portal.m_OtherPortalTransform.transform.InverseTransformDirection(m_Direction);
        Vector3 l_WorldDirectionMovement = _Portal.m_MirrorPortal.transform.TransformDirection(l_LocalDirectionMovement);


        m_CharacterController.enabled = false;
        transform.forward=_Portal.m_MirrorPortal.transform.TransformDirection(l_LocalDirection);
        m_Yaw=transform.rotation.eulerAngles.y;
        transform.position = _Portal.m_MirrorPortal.transform.TransformPoint(l_LocalPosition)+l_WorldDirectionMovement*m_OffsetPortal;
        m_CharacterController.enabled = true;
    }

    void ThrowAttachedObject(float Force)
    {
        if (m_ObjectAttached == null)
            return;


        m_ObjectAttached.transform.SetParent(null);
        m_ObjectAttached.isKinematic = false;
        m_ObjectAttached.AddForce(m_PitchController.forward * Force);
        m_ObjectAttached.GetComponent<Pickable>().SetAttached(false);
        m_ObjectAttached = null;
    }

    void UpdateAttachedObject()
    {
        Vector3 l_EulerAngles = m_AttachingPosition.rotation.eulerAngles;
        Vector3 l_Direction = m_AttachingPosition.transform.position - m_ObjectAttached.transform.position;
        float l_Distance = l_Direction.magnitude;
        float l_Movement = m_AttachingObjectSpeed * Time.deltaTime;
        if (l_Movement >= l_Distance)
        {
            m_AttachingObject = false;
            m_ObjectAttached.transform.SetParent(m_AttachingPosition);
            m_ObjectAttached.transform.localPosition = Vector3.zero;
            m_ObjectAttached.transform.localRotation=Quaternion.identity;
        }
        else
        {
            l_Direction /= l_Distance;
            m_ObjectAttached.MovePosition(m_ObjectAttached.transform.position + l_Direction * l_Movement);
            m_ObjectAttached.MoveRotation(Quaternion.Lerp(m_AttachingObjectStartRotation, Quaternion.Euler(0.0f, l_EulerAngles.y, l_EulerAngles.z), 1.0f - Mathf.Min(l_Distance / 1.5f, 1.0f)));
        }
    }

    bool CanAttachObject()
    {
        return m_ObjectAttached == null;
    }

    void AttachObject()
    {
        Ray l_Ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit l_Raycasthit;
        if (Physics.Raycast(l_Ray, out l_Raycasthit, m_MaxDistanceToAttachObject, m_AttachingObjectLayerMask.value))
        {
            m_AttachingObject = true;
            m_ObjectAttached = l_Raycasthit.collider.GetComponent<Rigidbody>() ? l_Raycasthit.collider.GetComponent<Rigidbody>() : m_ObjectAttached = null;
            m_ObjectAttached.GetComponent<Pickable>().SetAttached(true);
            m_ObjectAttached.isKinematic = true;
            m_AttachingObjectStartRotation = l_Raycasthit.collider.transform.rotation;
        }

    }

    public float GetLife()
    {
        return m_Life;
    }
   public void RestartGame()
   {
        m_Life = 1.0f;
        m_CharacterController.enabled = false;
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        m_CharacterController.enabled = true;
    }

    public bool Hit()
    {
        return true;
    }

    
}
