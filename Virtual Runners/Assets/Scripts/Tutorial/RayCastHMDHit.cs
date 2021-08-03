using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastHMDHit : MonoBehaviour
{
    public LayerMask hitLayerMask;
    private bool canvasUIOpen = false;
    private float sphereRadius = 0.25f;
    private Vector3 origin;
    private Vector3 direction;
    private float maxDistance = 20f;
    private float currentHitDistance;
    private GameObject canvasUI;
    
    
    // Start is called before the first frame update
    void Start()
    {
        canvasUI = GameObject.Find("UI_Canvas");
    }

    // FixedUpdate is called every fixed framerate frame
    void FixedUpdate()
    {
        // Determine if UI is open
        // If TRUE - will not toggle DisplayCues.cs script
        // If FALSE - will toggle DisplayCues.cs script
        if(canvasUI != null)
        {
            canvasUIOpen = canvasUI.GetComponent<MenuManager>().menuOpen;
        }
        RaycastHit hit;  
        origin = transform.position;
        direction = transform.TransformDirection(Vector3.forward);
        
        // If the sphere cast intersects any object
        if ((Physics.SphereCast(origin, sphereRadius, direction, out hit, maxDistance, hitLayerMask)) && !canvasUIOpen)
        {
            var cuesScriptCheck = hit.transform.gameObject.GetComponent<DisplayCues>();
           // Check if hit object is the currently playing cue and if it has DisplayCues.cs script
           //  and hasn't been called yet. If yes, Call OnRayHit() function
           if (cuesScriptCheck != null && !(cuesScriptCheck.viewedByPlayer) && cuesScriptCheck.currentCue)
           {
               cuesScriptCheck.OnRayHit();               
           }     
        }
    }
}
