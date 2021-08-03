using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ResetWireMaterial is a class that resets all Wire prebabs to their default material when the onWireCutterDestroy event is broadcast
public class ResetWireMaterial : MonoBehaviour
{
    // Default material for the Wire
    public Material DefaultMaterial;

    // Start is called before the first frame update
    void Start()
    {
        // Subscribe to the onWireCutterDestroy event
        GameEvents.current.onWireCutterDestroy += OnResetWireMaterial;
    }

    // Event Handler that sets the Wire's material to the default material
    public void OnResetWireMaterial()
    {
        // Set the material to default
        gameObject.GetComponent<MeshRenderer>().material = DefaultMaterial;
    }

    // OnDestroy is called when the object is destroyed
    private void OnDestroy()
    {
        // Unsubscribe from all events
        GameEvents.current.onWireCutterDestroy -= OnResetWireMaterial;
    }
}
