using UnityEngine;

public class Turret : MonoBehaviour
{
    public LineRenderer m_Laser;
    public LayerMask m_LaserLayerMask;
    public float m_MaxLaserDistance = 250.0f;
    public float m_AliveAngleInDegrees = 30.0f;
    public FPPlayerController m_PlayerController;

    Rigidbody m_Rigidbody;

    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        bool l_LaserAlive=Vector3.Dot(transform.up,Vector3.up)>Mathf.Cos(m_AliveAngleInDegrees*Mathf.Deg2Rad);
        m_Laser.gameObject.SetActive(l_LaserAlive);
        if (l_LaserAlive)
        {
            Ray l_Ray = new Ray(m_Laser.transform.position, m_Laser.transform.forward);
                float l_LaserDistance = m_MaxLaserDistance;
            RaycastHit l_RayCastHit;
            if (Physics.Raycast(l_Ray, out l_RayCastHit, m_MaxLaserDistance, m_LaserLayerMask.value))
            {
                l_LaserDistance = Vector3.Distance(m_Laser.transform.position, l_RayCastHit.point);
                if (l_RayCastHit.collider.tag == "RefractionCube")
                    l_RayCastHit.collider.GetComponent<RefractionCube>().CreateRefraction();
                if(l_RayCastHit.collider.tag == "Player")
                {
                    m_PlayerController.Kill();
                }
                else if (l_RayCastHit.collider.tag == "turret")
                {
                    l_RayCastHit.collider.gameObject.SetActive(false);
                }
                else if(l_RayCastHit.collider.tag == "LaserTrigger")
                {
                    Debug.Log("me siento atacado");
                    l_RayCastHit.collider.GetComponent<LaserTrigger>().ActivateDoor();
                }
            }
               
            m_Laser.SetPosition(1, new Vector3(0.0f, 0.0f, l_LaserDistance));
        }
       
    }
}
