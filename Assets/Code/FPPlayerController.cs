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

    public Camera m_Camera;
    public float m_NormalMovementFOV = 60.0f;
    public float m_RunMovementFOV = 80.0f;

    public float m_VerticalSpeed = 0.0f;
    bool m_OnGround = true;
    public float m_JumpSpeed = 10.0f;
    bool m_Shooting = false;
    bool m_Reloading = false;

    [Header("Key controls")]
    public KeyCode m_LeftKeyCode;
    public KeyCode m_RightKeyCode;
    public KeyCode m_UpKeyCode;
    public KeyCode m_DownKeyCode;
    public KeyCode m_JumpKeyCode = KeyCode.Space;
    public KeyCode m_RunKeyCode = KeyCode.LeftShift;
    public KeyCode m_DebugLockAngleKeyCode = KeyCode.I;
    public KeyCode m_DebugLockKeyCode = KeyCode.O;
    public KeyCode m_ReloadKeyCode;
    bool m_AngleLocked = false;
    bool m_AimLocked = true;
    


    [Header("Shoot")]
    public float m_MaxShootDistance = 50.0f;
    public LayerMask m_ShootingLayerMask;

    float m_Life;
    float m_Shield;
    Vector3 m_StartPosition;
    Quaternion m_StartRotation;

    //[Header("Camera")]
    
    //[Header("Animations")]
    //public Animation m_Animation;
    //public AnimationClip m_IdleAnimationClip;
    //public AnimationClip m_ShootAnimationClip;


    void Start()
    {
        m_Yaw = transform.rotation.y;
        m_Pitch = m_PitchController.localRotation.x;
        Cursor.lockState = CursorLockMode.Locked;
        m_AimLocked = Cursor.lockState == CursorLockMode.Locked;
        //SetIdleWeaponAnimation();
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
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
        Vector3 l_Direction = Vector3.zero;
        float l_Speed = m_Speed;
        //SetIdleWeaponAnimation();
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
            l_Direction = l_ForwardDirection;
        }
        if (Input.GetKey(m_DownKeyCode))
        {
            l_Direction = -l_ForwardDirection;
        }
        if (Input.GetKey(m_RightKeyCode))
        {
            l_Direction += l_RightDirection;
        }
        if (Input.GetKey(m_LeftKeyCode))
        {
            l_Direction -= l_RightDirection;
        }
        if (Input.GetKey(m_JumpKeyCode) && m_OnGround)
        {
            m_VerticalSpeed = m_JumpSpeed;

        }

        
        l_Direction.Normalize();
        Vector3 l_Movement = l_Direction * m_Speed * Time.deltaTime;

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

        if (Input.GetMouseButtonDown(0) && CanShoot())
            Shoot();

    }

    
    bool CanShoot()
    {
        return !m_Shooting;
    }
    void Shoot()
    {
        Ray l_Ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        RaycastHit l_RaycastHit;
        if (Physics.Raycast(l_Ray, out l_RaycastHit, m_MaxShootDistance, m_ShootingLayerMask.value) && !m_Shooting)
        {
            //SetShootWeaponAnimation();
        }
    }

    //void SetIdleWeaponAnimation()
    //{
    //    m_Animation.CrossFade(m_IdleAnimationClip.name);
    //}


    //void SetShootWeaponAnimation()
    //{
    //    m_Animation.CrossFade(m_ShootAnimationClip.name, 0.1f);
    //    m_Animation.CrossFadeQueued(m_IdleAnimationClip.name, 0.1f);
    //    StartCoroutine(EndShoot());
    //}

    //public IEnumerator EndShoot()
    //{
    //    yield return new WaitForSeconds(m_ShootAnimationClip.length);
    //    m_Shooting = false;
    //}

    public float GetLife()
    {
        return m_Life;
    }

    public float GetShield()
    {
        return m_Shield;
    }

    public void AddLife(float Life)
    {
        m_Life = Mathf.Clamp(m_Life + Life, 0.0f, 1.0f);
    }

    public void AddShield(float shield)
    {
        m_Shield = Mathf.Clamp(m_Shield + shield, 0.0f, 1.0f);
    }

    void Kill()
    {
        m_Life = 0.0f;
    }

    public void RestartGame()
    {
        m_Life = 1.0f;
        m_CharacterController.enabled = false;
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        m_CharacterController.enabled = true;
    }

    public void Hit(float life)
    {
        Debug.Log(life * 0.7f);



        if (life * 0.7f <= m_Shield)
        {
            m_Shield = m_Shield - life * 0.7f;
            m_Life = m_Life - life * 0.3f;
        }
        else
        {
            m_Life -= (life - m_Shield);
            m_Shield = 0;
        }
    }
}
