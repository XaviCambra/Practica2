using UnityEngine;

public class RefractionCube : MonoBehaviour
{
    public LineRenderer m_Laser;
    public LayerMask m_LaserLayerMask;
    public float m_MaxLaserDistance = 250.0f;
    bool m_RefractionEnabled = false;

    Rigidbody m_Rigidbody;

    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        m_Laser.gameObject.SetActive(m_RefractionEnabled);
        m_RefractionEnabled=false;
    }
    public void CreateRefraction()
    {
        if (m_RefractionEnabled)
            return;
        m_RefractionEnabled=true;
        Ray l_Ray = new Ray(m_Laser.transform.position, m_Laser.transform.forward);
        float l_LaserDistance=m_MaxLaserDistance;
        RaycastHit l_RayCastHit;
        if(Physics.Raycast(l_Ray, out l_RayCastHit, m_MaxLaserDistance, m_LaserLayerMask.value))
        {
            l_LaserDistance = Vector3.Distance(m_Laser.transform.position, l_RayCastHit.point);
            if (l_RayCastHit.collider.tag == "RefractionCube")
                l_RayCastHit.collider.GetComponent<RefractionCube>().CreateRefraction();
        }
        m_Laser.SetPosition(1, new Vector3(0.0f, 0.0f, l_LaserDistance));
    }
}
