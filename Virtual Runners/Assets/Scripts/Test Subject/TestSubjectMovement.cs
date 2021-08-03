// Script Written By Jay Pittenger


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/******************************************************************************************************
 * TestSubjectMovement is a class that performs movement of test subject based on OVR controller inputs
 ******************************************************************************************************/


public class TestSubjectMovement : MonoBehaviour
{
    private Vector2 thumbstick;
    private Vector2 rotationStick;
    private bool jump;
    private bool jumped;
    private bool inAir;
    private bool blockerSide;
    private bool blockerOverride;
    private bool onConveyor;
    public bool freeze;
    private Rigidbody rb;
    private float conveyorFrameRate;
    private float conveyorRate;
    public float movementFactor = 3.0f;
    public float jumpImpulse = 2.0f;
    public float limitDistance = 1.0f;
    private bool initialFall = true;
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip jumpClip;
    private float soundVolume = 1.0f;
    [Header("Object to obtain conveyor fixed frame (conveyor belt game object)")]
    public GameObject conveyorBelt;
    private List<Collider> activeColliders;
    private List<Collider> blockerColliders;
    private List<Collider> blockerOverrideColliders;
    private List<Collision> blockerOverrideCollisions;


    void Start()
    {
        // get conveyor movement from conveyorBelt game object
        conveyorRate = conveyorBelt.GetComponent<ConveyorMovement>().FixedFrameMovement;
        // assign rigid body variable 
        rb = GetComponent<Rigidbody>();
        // set initial thumbstic position and conveyor speed position change
        thumbstick = new Vector2(0, 0);
        rotationStick = new Vector2(0, 0);
        conveyorFrameRate = 0;

        // Game event to freeze motion 
        GameEvents.current.onTestSubjectFloorEnter += FreezeMovement;
        GameEvents.current.onGamePause += OnGamePauseFreeze;
        GameEvents.current.onTestSubjectRespawn += StartMovement;
        GameEvents.current.onGameResume += StartMovement;
        GameEvents.current.onKillTestSubject += ClearConveyorCollision;
        GameEvents.current.onSFXVolumeChange += UpdateVolumeLevel;

        // at start of game, freeze test subject in duct
        freeze = true;
        // freeze test subject in space
        rb.constraints = RigidbodyConstraints.FreezeAll;

        audioSource = GetComponent<AudioSource>();

        //initialize Collider lists
        activeColliders = new List<Collider>();
        blockerColliders = new List<Collider>();
        blockerOverrideColliders = new List<Collider>();
        blockerOverrideCollisions = new List<Collision>();

    }


    void FixedUpdate()
    {
        conveyorRate = conveyorBelt.GetComponent<ConveyorMovement>().FixedFrameMovement;
        // call update for ovr input refresh
        OVRInput.Update();
        if (!freeze && !initialFall)
        {
            // call method to perform jump based on LIndexTrigger input
            PerformJump(OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger));

            //  call method to perform Test subject movement
            PerformMovement();
        }
    }


    // NOTE: ALL OBJECTS ON CONVEYOR MUST HAVE TAG SET TO CONVEYOR
    // mark if test subject colliding with conveyor using collisions with objects
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("conveyor"))
        {
            conveyorFrameRate = conveyorRate;
            onConveyor = true;
        }
        // object in other collision
        else
        {
            conveyorFrameRate = 0;
            onConveyor = false;
        }

        // check if colliding with blocker override, if so activate override and add to collision list
        if(collision.gameObject.layer == 8)
        {
            blockerOverrideCollisions.Add(collision);
            blockerOverride = true;
        }

    }


    // Clear override trigger collisions if needed
    private void OnCollisionExit(Collision collision)
    {
        // if override collision was lost, remove from list
        if (collision.gameObject.layer == 8)
        {
            blockerOverrideCollisions.Remove(collision);
        }

        // check if both collision and trigger collider lists for overrrides are empty, if so, deactivate override
        if (blockerOverrideColliders.Count == 0 && blockerOverrideCollisions.Count == 0)
        {
            blockerOverride = false;
        }
    }


    // determine if test subject can jump 
    private void OnTriggerEnter(Collider other)
    {
        // ensure that object in collision is not in the "NoJump" layer
        if (other.gameObject.layer != 6)
        {
            // if blockerside layer, add collison to blocker colliders
            if (other.gameObject.layer == 7)
            {
                blockerColliders.Add(other);
                blockerSide = true;

                // add downward impulse when contacting blocker wall side collider to prevent high jump
                rb.AddForce(0, -.2f, 0, ForceMode.Impulse);

            }
            // add to override and active list if override layer
            else if (other.gameObject.layer ==  8)
            {
                blockerOverrideColliders.Add(other);
                activeColliders.Add(other);
                blockerOverride = true;
            }  
            else
            {
                activeColliders.Add(other);
            }
            inAir = false;
            initialFall = false;
        }
    }


    // check if collider is not colliding with anything. If this is the case, test subject is in the air
    private void OnTriggerExit(Collider other)
    {
        // remove from proper collider list based on collision
        if(other.gameObject.layer == 7)
        {
            blockerColliders.Remove(other);
        // remove from override and active list if override layer
        } 
        else if(other.gameObject.layer == 8)
        {
            blockerOverrideColliders.Remove(other);
            activeColliders.Remove(other);
        } 
        else
        {
            activeColliders.Remove(other);
        }
        
        // test subject in air if active collider list is empty
        if(activeColliders.Count == 0)
        {
            inAir = true;
        }

        // test subject not contacting sides of blockers if list is empty
        if (blockerColliders.Count == 0)
        {
            blockerSide = false;
        }
        // check if both collision and trigger collider lists for overrrides are empty, if so, deactivate override
        if (blockerOverrideColliders.Count == 0 &&  blockerOverrideCollisions.Count == 0)
        {
            blockerOverride = false;
        }
    }


    // get method for inAir var
    public bool GetInAir()
    {
        return inAir;
    }


    // Function performs test subject jump based on current state of test subject
    void PerformJump(float triggerVal)
    {
        // if trigger pulled 50% or more, activate jump
        if (triggerVal >= 0.5f)
        {
            jump = true;
        }
        else
        {
            jump = false;
        }

        // perform jump
        if (jump)
        {
            // only perform jump if jump has not been performed since trigger was last compressed and test subject is not currently in the air
            // and either not contacting a blocker side, or contacting a blocker side but also contacting blocker override layer
            if (!jumped && !inAir  && (!blockerSide ||(blockerSide && blockerOverride)))
            {
                rb.AddForce(0, jumpImpulse, 0, ForceMode.Impulse);
                jumped = true;
                // Play the jump sounds
                audioSource.PlayOneShot(jumpClip, soundVolume);
            }
        }
        else
        {
            jumped = false;
        }
    }


    // Function performs Test Subject movement based on current state of test subject
    void PerformMovement()
    {
        // set object position based on thumbstick position. use last on ground thumb position when in air
        // if not currently in the air, set new thumbstick position
        // This makes it such that test subject cannot change direction while in the air
        if (!inAir)
        {
            // obtain thumb position
            thumbstick = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick);
        }
        // limit forward movement to limitDistance when on conveyor
        // progressively limit speed the closer the test subject gets to the limitDistance only when running in positive x direction
        if (onConveyor && thumbstick.x > 0)
        {
            // movement factor adjusts overall speed of test subject. In x-direction check difference between
            // current x position of test subject and limit distance and use this to apply progressive speed.
            // subtract conveyor frame rate from x postion to simulate conveyor movement
            gameObject.transform.position = new Vector3((float)(gameObject.transform.position.x +
                (((movementFactor * 0.001) + conveyorFrameRate) * thumbstick.x * (limitDistance - gameObject.transform.position.x)) - conveyorFrameRate),
                gameObject.transform.position.y,
                (float)(gameObject.transform.position.z + (movementFactor * 0.001 * thumbstick.y)));
        }
        else if(onConveyor)
        {
            gameObject.transform.position = new Vector3((float)(gameObject.transform.position.x + (((movementFactor * 0.001) + conveyorFrameRate) * thumbstick.x) - conveyorFrameRate), gameObject.transform.position.y, (float)(gameObject.transform.position.z + (movementFactor * 0.001 * thumbstick.y)));
        } 
        else
        {
            gameObject.transform.position = new Vector3((float)(gameObject.transform.position.x + (movementFactor * 0.001 * thumbstick.x) - conveyorFrameRate), gameObject.transform.position.y, (float)(gameObject.transform.position.z + (movementFactor * 0.001 * thumbstick.y)));

        }
        // obtain thumbstick position for test subject rotation
        rotationStick = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick);
        //only adjust test subject rotation if thumbstick position is currently not zero
        if (rotationStick.x != 0 || rotationStick.y != 0)
        {
            gameObject.transform.forward = new Vector3(-rotationStick.y, 0, rotationStick.x);
        }
    }


    // function freezes the ability for the test subject to move during respawn from floor
    private void FreezeMovement()
    {
        // only perform freeze if test subject is not on conveyor
        if (gameObject.transform.position.y < 0.6f)
        {
            freeze = true;
            // freeze test subject in space
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }


    private void OnGamePauseFreeze()
    {
        freeze = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }


    // function unfreezes the ability for the test subject to move at beginning of respawn
    private void StartMovement()
    {
        // unfreeze movement
        freeze = false;
        // unfreeze test subject in space
        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }


    // function clears collision list and resets conveyor rate for killed test subject
    private void ClearConveyorCollision()
    {
        // set intial fall so that movement is not performed until new test subject contacts conveyor
        initialFall = true;
        freeze = true;
        conveyorFrameRate = 0;
        inAir = true;

        //reassign new activeColliders list
        activeColliders = new List<Collider>();
        blockerColliders = new List<Collider>();
        blockerOverrideColliders = new List<Collider>();
        blockerOverrideCollisions = new List<Collision>();
    }
    

    // Updates audio clip's play sound volume per UI Settings page's value
    private void UpdateVolumeLevel(float newVolume)
    {
        soundVolume = newVolume;
    }


    // Remove event subscriptions on destroy
    private void OnDestroy()
    {
        GameEvents.current.onTestSubjectFloorEnter -= FreezeMovement;
        GameEvents.current.onGamePause -= OnGamePauseFreeze;
        GameEvents.current.onTestSubjectRespawn -= StartMovement;
        GameEvents.current.onGameResume -= StartMovement;
        GameEvents.current.onKillTestSubject -= ClearConveyorCollision;
        GameEvents.current.onSFXVolumeChange -= UpdateVolumeLevel;
    }
}
