using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTrigger : MonoBehaviour
{
    public Companion m_CompanionPrefab;
    public DoorAnimationController doorController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "CompanionCube")
        {
            Debug.Log("Cubito encima");
            doorController.ActivateDoors(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "CompanionCube")
        {
            Debug.Log("Cubito encima");
            doorController.ActivateDoors(false);
        }
    }
}
