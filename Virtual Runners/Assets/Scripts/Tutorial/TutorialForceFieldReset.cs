using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialForceFieldReset : MonoBehaviour
{
    private GameObject forcefield;
    private GameObject wire;
    
    
    // Start is called before the first frame update
    private void Start()
    {
        GameEvents.current.onTutorialStart += ResetForcefield;
        GameEvents.current.onKillTestSubject += ResetForcefield;
        
        // Find forcefield and wire game objects
        var allObjects = gameObject.GetComponentsInChildren<Transform>();
        foreach(Transform someObject in allObjects)
        {
            if(someObject.name == "Forcefield")
            {
                forcefield = someObject.gameObject;
            }
            else if(someObject.name == "Wire")
            {
                wire = someObject.gameObject;
            }
        }

    }

    // Upon tutorial reset, resets forcefield sheild and wire to initial states
    private void ResetForcefield()
    {
        forcefield.SetActive(true);
        wire.SetActive(true);
        wire.GetComponent<ResetWireMaterial>().OnResetWireMaterial();
    }

    // When destroyed, unsubscribe from event manager
    private void OnDestroy()
    {
        GameEvents.current.onTutorialStart -= ResetForcefield;
        GameEvents.current.onKillTestSubject -= ResetForcefield;
    }
}
