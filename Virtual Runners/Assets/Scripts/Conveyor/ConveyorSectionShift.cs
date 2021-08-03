// Script Written By Jay Pittenger


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorSectionShift : MonoBehaviour
{
    private float initialY;
    private float initialZ;
    private float initialX;
    public float startingX = 1.5f;
    public float endingX = -2.5f;

    void Start()
    {
        // get initial y and z for section shift
        initialY = gameObject.transform.position.y;
        initialZ = gameObject.transform.position.z;
        initialX = gameObject.transform.position.x;
        // event subscription
        GameEvents.current.onTestSubjectRespawn += ResetSectionPosition;
        GameEvents.current.onTutorialStart += ResetSectionPosition;
    }


    void FixedUpdate()
    {
        //Debug.Log("Conveyor Section Position: " + transform.position.x + ", " + transform.position.y + ", " + transform.position.z);
        // check if section is at end postion, if so, move to beginning position
        if (gameObject.transform.position.x <= endingX)
        {
            GameEvents.current.LoopConveyorSection(gameObject);
            gameObject.transform.position = new Vector3(startingX, initialY, initialZ);
        }
    }


    // function resets conveyor section back to its original position for new run
    private void ResetSectionPosition() {
        gameObject.transform.position = new Vector3(initialX, initialY, initialZ);
    }


    // Remove event subscriptions on destroy
    private void OnDestroy()
    {
        GameEvents.current.onTestSubjectRespawn -= ResetSectionPosition;
        GameEvents.current.onTutorialStart -= ResetSectionPosition;
    }
}
