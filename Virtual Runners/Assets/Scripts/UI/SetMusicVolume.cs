using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SetMusicVolume : MonoBehaviour
{
    public Slider musicSlider;


    // Start is called before the first frame update
    public void Start()
    {        
        // Subscribe to game event
        GameEvents.current.onMusicVolumeChanged += UpdateScrollPosition;
        // Add listener to slider and invokes method when value is changed
        musicSlider.onValueChanged.AddListener( delegate {SetVolumeLevel();} );
    }

    // Adjust volume of set audio source
    public void SetVolumeLevel()
    {
        GameEvents.current.MusicVolumeChanged(musicSlider.value);
    }

    // Updates volume and slider position per value sent by game event
    private void UpdateScrollPosition(float newVolume)
    {
        musicSlider.value = newVolume;        
    }
    
    // If game object is destroyed, remove event subscription
    private void OnDestroy()
    {
        GameEvents.current.onMusicVolumeChanged -= UpdateScrollPosition;
    }
}
