using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script Purpose: Detect when the tool has collided with a trigger collider attached to an obstacle with a ToolEnter.cs script component
//      If the CollidedObject does have a ToolEnter.cs script, an Event will be sent to the Subscribed Object 
public class ToolCollideWithObstacle : MonoBehaviour
{
    // Detect when the attached object has entered a trigger collider
    private void OnTriggerEnter(Collider other)
    {
        // Tell 'other' the tool has entered the collider
        ToolEnter obj = other.GetComponent<ToolEnter>();
        if (obj)
        {
            GameEvents.current.ToolEnter(obj.GetInstanceID(), gameObject, other);
        }
    }

    // Detect when the attached object has left a trigger collider
    private void OnTriggerExit(Collider other)
    {
        // Tell 'other' the test subject has left the collider
        ToolEnter obj = other.GetComponent<ToolEnter>();
        if (obj)
        {
            GameEvents.current.ToolExit(obj.GetInstanceID(), gameObject, other);
        }
    }
}
