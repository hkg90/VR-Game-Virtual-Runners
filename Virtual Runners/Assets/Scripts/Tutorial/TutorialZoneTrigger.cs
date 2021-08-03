using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialZoneTrigger : MonoBehaviour
{
    private bool triggerCalled = false;

    
    // Called upon start
    private void Start()
    {
        GameEvents.current.onTutorialStart += ResetFlag;
    }
    
    // Resets flag
    private void ResetFlag()
    {
        triggerCalled = false;
    }

    // Handles test subject collision ("Player") hitting triggers to progress 
    // game play tutorial part
    void OnTriggerExit(Collider collision)
    {
        // Ensure event trigger is only called once when test subject hits it
        if (!triggerCalled && collision.tag == "Player")
        {
            // Call event to trigger progression of game play tutorial onces test
            // subject has hit trigger
            GameEvents.current.ProgressGamePlayTutorial();
            triggerCalled = true;
        }
    }

    // When destroyed, unsubscribe from event manager
    private void OnDestroy()
    {
        GameEvents.current.onTutorialStart -= ResetFlag;
    }
}
