using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolEnter : MonoBehaviour
{
    public int id;

    public Material ForcefieldInactiveMaterial;
    public Material WireNormalMaterial;
    public Material WireHighlightMaterial;


    // Start is called before the first frame update
    void Start()
    {
        id = GetInstanceID();
        GameEvents.current.onToolEnter += OnWireCutterEnter;
        GameEvents.current.onToolExit  += OnWireCutterExit;
        GameEvents.current.onToolExit  += OnBridgeEnter;
        
    }

    // Event Handler for when the Wire Cutter Enters a Wire's collider
    void OnWireCutterEnter(int id, GameObject trigger, Collider collision)
    {
        if (id == this.id)
        {
            var toolName = collision.gameObject.name;
            var triggerName = trigger.gameObject.name;
            //Debug.Log("Wire hover: " + triggerName);
            if (triggerName == "Wire Cutter(Clone)")
            {
                // Do the wire highlight
                gameObject.GetComponent<MeshRenderer>().material = WireHighlightMaterial;
            }
        }
    }

    // Event Handler for when the Wire Cutter Exits a Wire's Collider
    void OnWireCutterExit(int id, GameObject trigger, Collider collision)
    {
        if (id == this.id)
        {
            // Reset the Wire Material
            gameObject.GetComponent<MeshRenderer>().material = WireNormalMaterial;
        }
    }
    
    // Update is called once per frame
    void OnBridgeEnter(int id, GameObject trigger, Collider collision)
    {
        if (id == this.id)
        {
            DoAttachBridge(collision);
        }
    }    

    // Attach bridge to pit
    private void DoAttachBridge(Collider collision)
    {
        // Add code to either attach the bridge by snapping it into place or rendering a new 
        // model of pit + bridge
    }

    // If game object is destroyed, remove subscriptions
    private void OnDestroy()
    {
        GameEvents.current.onToolEnter -= OnWireCutterEnter;
        GameEvents.current.onToolExit  -= OnWireCutterExit;
        GameEvents.current.onToolExit  -= OnBridgeEnter;
    }
}
