using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimationController : MonoBehaviour
{
    public DoorAnimation[] doorList;

    public void ActivateDoors(bool setter)
    {
        foreach(DoorAnimation door in doorList)
        {
            door.SetIsOpen(setter);
        }
    }
}
