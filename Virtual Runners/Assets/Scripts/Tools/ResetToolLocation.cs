using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

// This script focuses on tool placement onto the tool table, belt and floor. Depending on the tool type, 
// the tool is either reset on the tool table, left on the belt or destroyed. 
public class ResetToolLocation : MonoBehaviour
{    
    // Store tool's original location on table and tool's belt status
    private Vector3 position; 
    public bool onToolTable = true;  
    public bool toolBeingHeld = false;
    public bool gamePaused = false;
    private Rigidbody rb = null;
    private LayerMask allowGrabMask = -1; // Layer mask value: Everything
    private LayerMask disallowGrabMask = 0; // Layer mask value: Nothing
    private XRGrabInteractable grabScript = null;


    // Start is called before the first frame update
    void Start()
    {
        // Find components relevant to tool
        position = transform.position;
        // Ignore collisions with test subject
        var tempTestSub = GameObject.FindGameObjectWithTag("Player");
        if(tempTestSub != null)
        {
            Physics.IgnoreCollision(tempTestSub.GetComponent<Collider>(), gameObject.GetComponent<Collider>(), true);
        }
                
        grabScript = gameObject.GetComponent<XRGrabInteractable>();
        rb = gameObject.GetComponent<Rigidbody>();

        // Subscribe to events
        GameEvents.current.onToolGrabbed += DisableToolGrabbable;
        GameEvents.current.onToolReleased += AllowToolGrabbable;
        GameEvents.current.onGamePause += DisableToolGrabbable;
        GameEvents.current.onGameResume += AllowToolGrabbable;
        GameEvents.current.onGamePause += PauseFlag;
        GameEvents.current.onGameResume += ResumeFlag;
    }

    // FixedUpdate is called every fixed framerate frame
    void FixedUpdate()
    {
        if(gamePaused && gameObject.tag != "conveyor" && (rb.isKinematic == false || grabScript.interactionLayerMask.value != 0))
        {
            DisableToolGrabbable();
        }
    }
    
    // Handles continuous collisions ecountered by the tool
    private void OnCollisionStay(Collision collision)
    {
        // If tool moved to different position on tool table, reset position if 
        // it was moved too far from its original spawn location
        if (collision.gameObject.tag == "ToolTable") 
        {      
            // Reposition tool if it's moved too far form it's original position
            if ((transform.position - position).magnitude > 0.2f)
            {
                transform.position = position;
            }
        }
    }
    
    // Handles collisons encountered by the tool
    private void OnCollisionEnter(Collision collision)
    {        
        // Actions for tool currently being held by player
        if (onToolTable == false)
        {          
            // If tool marked dirty released by player onto tool table, reset flag
            if (collision.gameObject.tag == "ToolTable" && !toolBeingHeld) 
            {
                onToolTable = true;
            }            
            // If tool held by player is placed onto conveyor belt
            else if (collision.gameObject.tag == "conveyor")
            {
                // Update game object tag to 'conveyor'
                gameObject.tag = "conveyor";
                // Disable tool from being grabbed
                grabScript.interactionLayerMask = disallowGrabMask;
                // Update position
                position = transform.position;

                // Un-ignore collisions with test subject
                Physics.IgnoreCollision(GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>(), gameObject.GetComponent<Collider>(), false);
                
                // Find conveyor section tool landed on
                var conveyorParent = collision.gameObject.transform.parent;
                while(true)
                {
                    if(conveyorParent.name.Contains("ConveyorSection"))
                    {
                        break;
                    }
                    conveyorParent = conveyorParent.gameObject.transform.parent;
                }
                // Update tool's parent to conveyor section it landed on
                gameObject.transform.parent = conveyorParent.gameObject.transform;
                
                // If tool is wire cutter, destroy wire cutter tool to respawn
                if (gameObject.name.Contains("Wire Cutter"))
                {                    
                    GameEvents.current.WireCutterDestroy();
                    Destroy(gameObject);
                }        
            } 

            // If tool lands on anywhere else, destroy it           
            else if (collision.gameObject.tag == "Untagged" || collision.gameObject.tag == "Floor")
            {
                Destroy(gameObject);
            }            
        }        
    }

    // Function called through XR Grab Interactable's 'Interactable Events - Select' section
    public void SelectedToolGrabbed()
    {
        toolBeingHeld = true;
        onToolTable = false;
        GameEvents.current.ToolGrabbed();
    }

    // Function called through XR Grab Interactable's 'Interactable Events - Select Exited'section
    public void SelectedToolReleased()
    {
        toolBeingHeld = false;
        GameEvents.current.ToolReleased();
    }

    // When player is holding a tool or game is paused, tools not being held by player are disabled from being grabbable
    public void DisableToolGrabbable()    
    {
        if(onToolTable && !toolBeingHeld)
        {
            // If variables have not been updated, find components
            if(grabScript == null || rb == null)
            {
                grabScript = gameObject.GetComponent<XRGrabInteractable>();
                rb = gameObject.GetComponent<Rigidbody>();
            }
            // Change interaction layer mask to Nothing      
            grabScript.interactionLayerMask = disallowGrabMask;
            // Change kinematic setting of tool (prevent tool on table from being pushed around)
            rb.isKinematic = true;
        }   
    }

    // When player has released a tool they were holding, tools that were disabled from being 
    // grabbable are reset to be grabbable
    private void AllowToolGrabbable()
    {
        if(onToolTable && !toolBeingHeld)     
        {
            // Change interaction layer mask to Everything     
            grabScript.interactionLayerMask = allowGrabMask;
            // Change kinematic setting of tool (prevent tool on table from being pushed around)
            rb.isKinematic = false;
        }
    }

    // Updates game paused flag to determine if update to tool's grab state is needed on Game Pause
    private void PauseFlag()
    {
        gamePaused = true;
    }

    // Updates game paused flag to determine if update to tool's grab state is needed on Game Resume
    private void ResumeFlag()
    {
        gamePaused = false;
    }

    // When game object is destroyed, remove all subscriptions
    private void OnDestroy()
    {
        GameEvents.current.onToolGrabbed -= DisableToolGrabbable;
        GameEvents.current.onToolReleased -= AllowToolGrabbable;
        GameEvents.current.onGamePause -= DisableToolGrabbable;
        GameEvents.current.onGameResume -= AllowToolGrabbable;
        GameEvents.current.onGamePause -= PauseFlag;
        GameEvents.current.onGameResume -= ResumeFlag;
    }
}
