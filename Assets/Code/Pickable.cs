using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickable : MonoBehaviour
{

    bool m_IsAttached = false;

    public void SetAttached(bool Attached)
    {
        m_IsAttached = Attached;
    }

    public bool GetAttached()
    {
        return m_IsAttached;
    }
}
