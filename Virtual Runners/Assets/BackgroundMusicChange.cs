using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicChange : MonoBehaviour
{    
    private AudioSource audioSource;

    
    // Start is called before the first frame update
    void Start()
    {
        // Subscribe to game events
        GameEvents.current.onMusicVolumeChanged += UpdateVolumeLevel;
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.Play();
    }

    // Updates audio clip's play sound volume per UI Settings page's value
    private void UpdateVolumeLevel(float newVolume)
    {
        audioSource.volume = newVolume;
    }    

    // Remove event subscriptions on destroy
    private void OnDestroy()
    {
        GameEvents.current.onMusicVolumeChanged -= UpdateVolumeLevel;
    }
}
