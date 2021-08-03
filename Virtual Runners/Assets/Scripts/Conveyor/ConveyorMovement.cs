// Script Written By Jay Pittenger


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorMovement : MonoBehaviour
{
    public float FixedFrameMovement = 0.001f;
    public float MaxFixedFrameMovement = 0.004f;
    public float FixedFrameMovementFactor = 1.0f;
    private Vector3 direction;
    // start conveyor in off state
    private bool movementOn = false;
    // flag for determining if conveyor can be turned on/ off
    private bool gameOngoing = false;
    private bool rateIncrease = true;
    public float IncreaseRate = 0.00002f;


    void Start()
    {
        // Event subscription
        GameEvents.current.onKillTestSubject += SetMovementOff; 
        GameEvents.current.onKillTestSubject += ResetMovementOnDeath;
        GameEvents.current.onTestSubjectRespawn += SetMovementOn;
        GameEvents.current.onGamePause += SetMovementOff;
        GameEvents.current.onGameResume += SetMovementOn;
        GameEvents.current.onTutorialStart += SetFlagOn;
        GameEvents.current.onTutorialReset += SetFlagOff;
        GameEvents.current.onTutorialStart += DecreaseSpeed;
        GameEvents.current.onTutorialComplete += IncreaseSpeed;
        GameEvents.current.onIncreaseConveyor += IncreaseConveyorRate;
    }


    void FixedUpdate()
    {
        if (movementOn && gameOngoing)
        {
            // subtract FixedFrameMovement from x position every frame
            gameObject.transform.position = new Vector3((float)(gameObject.transform.position.x - (FixedFrameMovement * FixedFrameMovementFactor)), gameObject.transform.position.y, gameObject.transform.position.z);
        }
        
    }


    void OnCollisionStay(Collision otherThing)
    {
    // Get the direction of the conveyor belt 
    // (transform.forward is a built in Vector3 
    // which is used to get the forward facing direction)
    // * Remember Vector3's can used for position AND direction AND rotation
    direction = transform.forward;
    direction = direction*FixedFrameMovement;

    // Add a WORLD force to the other objects
    // Ignore the mass of the other objects so they all go the same speed (ForceMode.Acceleration)
    otherThing.rigidbody.AddForce(direction, ForceMode.Acceleration);
    }


    // Updates speed to MainScene initial speed
    private void IncreaseSpeed()
    {
        FixedFrameMovement = 0.001f;

        rateIncrease = true;
    }


    // Decreses speed to TutorialScene desired speed
    void DecreaseSpeed()
    {
        // Update with tutorial desired speed
        FixedFrameMovement = 0.0003f;

        //rate increase set to false
        rateIncrease = false;
    }


    // Sets movementOn to true
    private void SetMovementOn()
    {
        movementOn = true;
        SetFlagOn();
    }

    
    // Sets movementOn to false
    private void SetMovementOff()
    {
        movementOn = false;
        SetFlagOff();
    }


    // Sets game ongoing flag to on
    private void SetFlagOn()
    {
        gameOngoing = true;
    }


    // Sets game ongoing flag to on
    private void SetFlagOff()
    {
        gameOngoing = false;
    }


    // Method increases speed of conveyor over time as called by timer controller
    private void IncreaseConveyorRate()
    {
        if (rateIncrease && FixedFrameMovement < MaxFixedFrameMovement)
        {
            FixedFrameMovement += IncreaseRate;
        }
    }


    // resets rate to initial rate when test subject dies
    private void ResetMovementOnDeath()
    {
        if (rateIncrease)
        {
            FixedFrameMovement = 0.001f;
        }
        else
        {
            FixedFrameMovement = 0.0003f;
        }
    }


    // Remove event subscriptions on destroy
    private void OnDestroy()
    {
        GameEvents.current.onKillTestSubject -= SetMovementOff;
        GameEvents.current.onKillTestSubject -= ResetMovementOnDeath;
        GameEvents.current.onTestSubjectRespawn -= SetMovementOn;
        GameEvents.current.onGamePause -= SetMovementOff;
        GameEvents.current.onGameResume -= SetMovementOn;
        GameEvents.current.onTutorialStart -= SetFlagOn;
        GameEvents.current.onTutorialReset -= SetFlagOff;
        GameEvents.current.onTutorialStart -= DecreaseSpeed;
        GameEvents.current.onTutorialComplete -= IncreaseSpeed;
        GameEvents.current.onIncreaseConveyor -= IncreaseConveyorRate;
    }
}
