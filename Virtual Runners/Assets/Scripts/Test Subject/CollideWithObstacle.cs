using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Disclaimer: Test script with minimal production application.
//     I just wanted something to demonstrate interaction between the object attached to this script and Obstacles

// Script Purpose: Detect when the attached Object has collided with a trigger collider attached to an Object with a TestSubjectEnter.cs script component
//      If the CollidedObject does have a TestSubjectEnter.cs script, an Event will be sent to the Subscribed Object that matches the ID of the trigger
public class CollideWithObstacle : MonoBehaviour
{
    // Detect when the attached object has entered a trigger collider
    private void OnTriggerEnter(Collider other)
    {
        // Tell 'other' the test subject has entered the collider
        TestSubjectEnter obj = other.GetComponent<TestSubjectEnter>();
        if (obj)
        {
            GameEvents.current.TestSubjectEnter(obj.GetInstanceID());
        }
    }

    // Detect when the attached object has left a trigger collider
    private void OnTriggerExit(Collider other)
    {
        // Tell 'other' the test subject has left the collider
        TestSubjectEnter obj = other.GetComponent<TestSubjectEnter>();
        if (obj)
        {
            GameEvents.current.TestSubjectExit(obj.GetInstanceID());
        }
    }
}
