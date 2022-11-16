using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTrigger : MonoBehaviour
{
   public DoorAnimationController doorController;

    public void ActivateDoor()
    {
            doorController.ActivateDoors(true);
    }
}
