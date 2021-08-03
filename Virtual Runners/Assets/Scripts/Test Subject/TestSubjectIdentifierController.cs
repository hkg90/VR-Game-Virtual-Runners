// Script Written by Jay Pittenger


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/******************************************************************************************************
 * TestSubjectIdentifierController is a class that controls the movement and visibility of the identifier
 ******************************************************************************************************/



public class TestSubjectIdentifierController : MonoBehaviour
{
    private float xPos;
    private float yPos;
    private float zPos;
    private bool positiveDirection = true;

    public float averageYPosition = 6.5f;


    void FixedUpdate()
    {
        // get identifier position
        xPos = gameObject.transform.localPosition.x;
        yPos = gameObject.transform.localPosition.y;
        zPos = gameObject.transform.localPosition.z;

        // perform identifier movement
        if (positiveDirection)
        {
            gameObject.transform.localPosition = new Vector3(xPos, (yPos + 0.015f), zPos);
        }  
        else
        {
            gameObject.transform.localPosition = new Vector3(xPos, (yPos - 0.015f), zPos);
        }


        // change direction if past bounds of y position
        if (yPos > (averageYPosition + 0.35f))
        {
            positiveDirection = false;
        }
        
        else if(yPos < (averageYPosition - 0.35f))
        {
            positiveDirection = true;
        }
    }
}
