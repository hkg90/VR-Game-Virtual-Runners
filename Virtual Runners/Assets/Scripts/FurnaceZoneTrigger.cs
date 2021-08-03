// Script Written by Jay Pittenger


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/******************************************************************************************************
 * FurnaceZoneTrigger is a class that triggers test subject respawn when test subject passes into trigger
 ******************************************************************************************************/



public class FurnaceZoneTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // only call test subject kill if test subject passes through furnace zone
        if (other.tag == "Player")
        {
            GameEvents.current.KillTestSubject();
        }
    }
    
    
    private void OnTriggerExit(Collider other)
    {
        // Tell the GameObject it has left the FurnaceZone
        GameEvents.current.FurnaceZoneExit(other.gameObject);
    }
}
