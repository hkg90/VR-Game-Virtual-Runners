using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles ray cast hit on tool objects to trigger highlight when hovering over object to select 
public class RayCastControllerHit : MonoBehaviour
{
    // FixedUpdate is called every fixed frame-rate frame
    void FixedUpdate()
    {
        RaycastHit hit;        

        // If the ray cast intersects any object
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit))
        {
           var highlightScriptCheck = hit.transform.gameObject.GetComponent<HighlightObject>();
           // Check if hit object has High Light Object script
           if (highlightScriptCheck != null){
               // Call script's OnRayHit() function
               highlightScriptCheck.OnRayHit();               
           }
        }
    }
}
