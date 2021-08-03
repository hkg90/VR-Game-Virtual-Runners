using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************************************************************************************
 * BurningEffect is class that will play a burning particle effect on the object
 ******************************************************************************************************/
public class BurningEffect : MonoBehaviour
{

    public int id;

    // Start is called before the first frame update
    void Start()
    {
        id = GetInstanceID();
        // Subscribe to the OnRedZoneEnter and Exit Events
        GameEvents.current.onRedZoneEnter += OnRedZoneEnter;
        GameEvents.current.onRedZoneExit += OnRedZoneExit;
    }

    // Receive OnRedZoneEnter events
    private void OnRedZoneEnter(int id)
    {
        if (id == this.id)
        {
            StartBurningEffect();
        }
    }
    // Start the burning effect
    private void StartBurningEffect() 
    {        
        // Play the particle effect
        ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
        ps.Play();
    }
    // Receive the OnRedZoneExit event
    private void OnRedZoneExit(int id)
    {
        if (id == this.id)
        {
            StopBurningEffect();
        }
    }
    // Stop the burning effect
    private void StopBurningEffect()
    {
        // Stop the particle effect
        ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
        ps.Stop();
    }
    // Unsubscribe from the events
    private void OnDestroy()
    {
        GameEvents.current.onRedZoneEnter -= OnRedZoneEnter;
        GameEvents.current.onRedZoneExit -= OnRedZoneExit;
    }
}
