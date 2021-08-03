using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************************************************************************************
 * RedZoneTrigger is a class that calls events on Objects as they enter and exit the RedZone Collider
 * To add a new event, simply check if 'other' object has the script and pass the id to with the event
 ******************************************************************************************************/
public class RedZoneTrigger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        // Tell 'other' to do the burning effect event
        BurningEffect obj = other.GetComponent<BurningEffect>();
        if (obj)
        {
            GameEvents.current.RedZoneEnter(obj.GetInstanceID());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Tell 'other' to do the stop burning effect event
        BurningEffect obj = other.GetComponent<BurningEffect>();
        if (obj)
        {
            GameEvents.current.RedZoneExit(obj.GetInstanceID());
        }
    }
}
