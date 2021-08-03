// Script Written by Jay Pittenger


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/******************************************************************************************************
 * TestSubjectRespawn is a class that handles the respawning of the Test Subject in game
 ******************************************************************************************************/


public class TestSubjectRespawn : MonoBehaviour
{

    // RESPAWN VARIABLES
    private Vector3 initialPos;
    private Quaternion initialRot;

    private Rigidbody rb;
    private bool hitFloor = false;

    // update color on meshobject
    Renderer Renderer;
    public GameObject MeshObject;


    // Start is called before the first frame update
    void Start()
    {
        // assign rigid body variable 
        rb = GetComponent<Rigidbody>();
        // GET INITIAL POSITION ROTATION FOR RESPAWN
        initialPos = gameObject.transform.position;
        initialRot = gameObject.transform.rotation;


        // event driven reset on kill
        GameEvents.current.onKillTestSubject += ResetTestSubject;
        // event driver tutorial start
        GameEvents.current.onTutorialStart += ResetTestSubject;


        Renderer = MeshObject.GetComponent<Renderer>();
    }


    private void OnTriggerEnter(Collider trigger)
    {
        // if collided with floor trigger, trigger event
        if(trigger.CompareTag("FloorTrigger"))
        {
            if (!hitFloor)
            {
                GameEvents.current.TestSubjectFloorEnter();
                hitFloor = true;
            }
        }
    }


    // Function performs reset of test subject for next respawn
    private void ResetTestSubject()
    {

        // freeze test subject in space
        rb.constraints = RigidbodyConstraints.FreezeAll;

        rb.velocity = new Vector3(0, 0, 0);
        rb.angularVelocity = new Vector3(0, 0, 0);
        gameObject.transform.rotation = initialRot;
        gameObject.transform.position = initialPos;
        hitFloor = false;

        // randomize color for new respawn
        Renderer.material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

    }


    // Remove event subscriptions on destroy
    private void OnDestroy()
    {
        GameEvents.current.onKillTestSubject -= ResetTestSubject;
        GameEvents.current.onTutorialStart -= ResetTestSubject;
    }
}


