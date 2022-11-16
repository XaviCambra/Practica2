using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimation : MonoBehaviour
{
    private float m_doorSize = 1;
    private bool m_isOpen;
    public Transform m_door;

    // Update is called once per frame
    void Update()
    {
        if(m_isOpen && m_doorSize > 0)
        {
            m_doorSize -= Time.deltaTime;
            if (m_doorSize < 0)
                m_doorSize = 0;

            m_door.localScale = new Vector3(1, m_doorSize, 1);
        }
        else if(m_isOpen == false && m_doorSize < 1)
        {
            m_doorSize += Time.deltaTime;
            if (m_doorSize > 1)
                m_doorSize = 1;

            m_door.localScale = new Vector3(1, m_doorSize, 1);
        }

    }

    public void SetIsOpen(bool setter)
    {
        m_isOpen = setter;
    }
}
