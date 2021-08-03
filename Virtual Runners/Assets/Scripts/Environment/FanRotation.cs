// Script Written by Jay Pittenger


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/******************************************************************************************************
 * FanRotation is a class that performs rotation on the fan blades in the scene
 ******************************************************************************************************/



public class FanRotation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // rotate by 1 degree about y axis every fixed frame
        gameObject.transform.Rotate(0, 4.0f, 0, Space.Self);
    }
}
