using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTrigger : MonoBehaviour
{
    public Companion m_CompanionPrefab;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "CompanionCube")
        {
            Debug.Log("Cubito encima");
        }
    }
}
