using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class SetSFXVolume : MonoBehaviour
{
    public Slider sfxSlider;


    // Start is called before the first frame update
    void Start()
    {
        //Subscribe to game event
        GameEvents.current.onSFXVolumeChange += UpdateScrollPosition;

        // Add listener to slider and invokes method when value is changed
        sfxSlider.onValueChanged.AddListener( delegate {SetVolumeLevel();} );        
    }

    // Adjust volume of set audio source
    public void SetVolumeLevel()
    {
       GameEvents.current.SFXVolumeChange(sfxSlider.value);
    }

    // Adjusts scroller position as needed (ie on game load)
    private void UpdateScrollPosition(float newVolume)
    {
        sfxSlider.value = newVolume;
    }

    // If game object is destroyed, remove event subscription
    private void OnDestroy()
    {
        GameEvents.current.onSFXVolumeChange -= UpdateScrollPosition;
    }
}
