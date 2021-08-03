using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireCutterClose : MonoBehaviour
{
    private Animation wireCutterAnimation;
    private GameObject TargetObstacle;
    private AudioSource audioSource;
    private float soundVolume = 1.0f;
    [SerializeField]
    private AudioClip wireCutterCloseClip;
    private bool currentlyHeld = false;

    private float trigger;
    private bool onTable; // Flag for if tool is being held by player
    
    private bool singleAnimation = false; // Flag for 'close' animation 


    // Start is called before the first frame update
    void Start()
    {
        wireCutterAnimation = GetComponent<Animation>();
        audioSource = GetComponent<AudioSource>();
        TargetObstacle = null;

        // Subscribe to the ToolEnterObstacle and ToolExitObstacle Events
        GameEvents.current.onToolEnter += OnToolEnter;
        GameEvents.current.onToolExit  += OnToolExit;
        GameEvents.current.onSFXVolumeChange += UpdateVolumeLevel;
    }

    // FixedUpdate is called every fixed framerate frame
    void FixedUpdate()
    {
        currentlyHeld = gameObject.GetComponent<ResetToolLocation>().toolBeingHeld;
        // Get update for OVR input 
        OVRInput.Update();
        trigger = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger);

        // if trigger pulled 50% or more and wire cutter is currently held, trigger close animation
        if (trigger >= 0.5f && currentlyHeld)
        {
            // Ensure 'close' animation only plays once per trigger squeeze
            if (singleAnimation == false)
            {
                singleAnimation = true;
                wireCutterAnimation.Play("LegacyWireCutterClose");
                // Play clipping sound
                audioSource.PlayOneShot(wireCutterCloseClip, soundVolume); 
                // Check if we're in contact with a wire
                if (TargetObstacle)
                {
                    // Trigger Wire Cut Event
                    GameEvents.current.ToolObstacleAction(TargetObstacle.GetInstanceID(), TargetObstacle);
                }
            }            
        }
        
        // Reset 'close' animation play flag
        else 
        {
            singleAnimation = false;
        }
    }

    // Event handler for when the Wire Cutter enters an Obstacle
    void OnToolEnter(int id, GameObject trigger, Collider collision)
    {
        // Check if we collided with a Wire
        if (collision.gameObject.name == "Wire")
        {
            // Update the Obstacle we're currently on
            TargetObstacle = collision.gameObject;
        }
    }

    // Event handler for when the Wire Cutter leaves an Obstacle
    void OnToolExit(int id, GameObject trigger, Collider collision)
    {
        // Check if we're leaving the collider of the TargetObstacle
        if (collision == TargetObstacle)
        {
            TargetObstacle = null;
        }
    }

    // Updates audio clip's play sound volume per UI Settings page's value
    private void UpdateVolumeLevel(float newVolume)
    {
        soundVolume = newVolume;
    }

    // If object is destroyed, unsubscribe to game events
    private void OnDestroy()
    {
        GameEvents.current.onToolEnter -= OnToolEnter;
        GameEvents.current.onToolExit -= OnToolExit;
        GameEvents.current.onSFXVolumeChange -= UpdateVolumeLevel;
    }
}
