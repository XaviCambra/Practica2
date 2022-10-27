using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Portal : MonoBehaviour
{
    public Camera m_Camera;
    public Transform m_OtherPortalTransform;
    public Portal m_MirrorPortal;
    public FPPlayerController m_Player;
    public float m_OffsetNearPlane;
    public List<Transform> m_ValidPoints;
    public float m_MinValidDistance = 0.2f;
    public float m_MaxValidDistance = 0.3f;
    public float m_MinDotValidAngle;

    private void LateUpdate()
    {
        Vector3 l_WorldPosition = m_Player.m_Camera.transform.position;
        Vector3 l_LocalPosition = transform.InverseTransformPoint(l_WorldPosition);
        m_MirrorPortal.m_Camera.transform.position = m_MirrorPortal.transform.TransformPoint(l_LocalPosition);

        Vector3 l_WorldDirection = m_Player.m_Camera.transform.forward;
        Vector3 l_LocalDirection = m_OtherPortalTransform.InverseTransformDirection(l_WorldDirection);
        m_MirrorPortal.m_Camera.transform.forward = m_MirrorPortal.transform.TransformDirection(l_LocalDirection);

        float l_Distance = Vector3.Distance(m_MirrorPortal.m_Camera.transform.position, m_MirrorPortal.transform.position);
        m_MirrorPortal.m_Camera.nearClipPlane = l_Distance + m_OffsetNearPlane;
    }

    public bool isValidPosition(Vector3 StartPosition, Vector3 Forward, float MaxDistance, LayerMask PortalLayerMask)
    {
        Ray l_Ray=new Ray(StartPosition,Forward);
        RaycastHit l_RaycastHit;
        if(Physics.Raycast(l_Ray, out l_RaycastHit, MaxDistance, PortalLayerMask.value))
        {
            if (l_RaycastHit.collider.tag == "DrawableWall")
            {
                Vector3 l_Normal = l_RaycastHit.normal;
                Vector3 l_Position = l_RaycastHit.point;

                for(int i = 0; i < m_ValidPoints.Count; i++)
                {
                    Vector3 l_Direction = m_ValidPoints[i].position - StartPosition;
                    l_Direction.Normalize();
                    l_Ray = new Ray(StartPosition, l_Direction);

                    if (Physics.Raycast(l_Ray, out l_RaycastHit, MaxDistance, PortalLayerMask.value))
                    {
                        if (l_RaycastHit.collider.tag == "DrawableWall")
                        {
                            float l_Distance = Vector3.Distance(l_Position, l_RaycastHit.point);
                            float l_DotAngle = Vector3.Dot(l_Normal, l_RaycastHit.normal);
                            if (l_Distance >= m_MinValidDistance && l_DotAngle > m_MinDotValidAngle)
                            {

                            }

                        }
                    }
                }
            }
        }
        
        return false;
    }
}
