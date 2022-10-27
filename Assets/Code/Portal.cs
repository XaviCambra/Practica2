using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Camera m_Camera;
    public Transform m_OtherPortalTransform;
    public Portal m_MirrorPortal;
    public FPPlayerController m_Player;

    private void LateUpdate()
    {
        Vector3 l_WorldPosition = m_Player.m_Camera.transform.position;
        Vector3 l_LocalPosition = transform.InverseTransformPoint(l_WorldPosition);
        m_MirrorPortal.m_Camera.transform.position = m_MirrorPortal.transform.TransformPoint(l_LocalPosition);

        Vector3 l_WorldDirection = m_Player.m_Camera.transform.forward;
        Vector3 l_LocalDirection = m_OtherPortalTransform.InverseTransformDirection(l_WorldDirection);
        m_MirrorPortal.m_Camera.transform.forward = m_MirrorPortal.transform.TransformDirection(l_LocalDirection);
    }
}
