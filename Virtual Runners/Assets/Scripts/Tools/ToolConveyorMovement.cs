using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ToolConveyorMovement : MonoBehaviour
{
    private bool frozen = false;
    private Rigidbody rb;


    // Start is called before the first frame update
    void Start()
    {      
        // Subscribe to onGamePause and onGameResume events
        GameEvents.current.onGamePause += FreezeToolsOnBelt;
        GameEvents.current.onGameResume += UnfreezeToolsOnBelt;

        rb = GetComponent<Rigidbody>();
    }

    // FixedUpdate is called every fixed framerate frame
    void FixedUpdate()
    {
        if (gameObject.tag == "conveyor" && frozen)
        {            
            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
        }
    }

    // On game pause, freeze tool on conveyor
    void FreezeToolsOnBelt()
    {
        frozen = true;    
    }

    // On game resume, unfreeze tool on conveyor
    void UnfreezeToolsOnBelt()
    {
        frozen = false;        
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.None;
        }
    }

    void OnDestroy()
    {
        GameEvents.current.onGamePause -= FreezeToolsOnBelt;
        GameEvents.current.onGameResume -= UnfreezeToolsOnBelt;
    }
}
