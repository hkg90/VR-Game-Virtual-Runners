using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PickupSound : MonoBehaviour
{
    private bool isHovering;

    public AudioClip hoverEnterSound;
    public AudioClip hoverExitSound;
    public AudioClip selectEnterSound;
    public AudioClip selectExitSound;
    private float soundVolume = 1.0f;

    private void Awake()
    {
        isHovering = false;
    }

    private void Start()
    {
        // Subscribe to game event
        GameEvents.current.onSFXVolumeChange += UpdateVolumeLevel;
    }

    public void PlayHoverEnterSound()
    {
        if (hoverEnterSound != null)
        {
            if (!isHovering)
            {
                AudioSource.PlayClipAtPoint(hoverEnterSound, transform.position, soundVolume);
                isHovering = true;
            }
        }
    }

    public void PlayHoverExitSound()
    {
        isHovering = false;
        if (hoverExitSound != null)
        {
            AudioSource.PlayClipAtPoint(hoverExitSound, transform.position, soundVolume);
        }
    }

    public void PlaySelectEnterSound()
    {
        if (selectEnterSound != null)
        {
            AudioSource.PlayClipAtPoint(selectEnterSound, transform.position, soundVolume);
        }
    }

    public void PlaySelectExitSound()
    {
        if (selectExitSound != null)
        {
            AudioSource.PlayClipAtPoint(selectExitSound, transform.position, soundVolume);
        }
    }

    // Updates audio clip's play sound volume per UI Settings page's value
    private void UpdateVolumeLevel(float newVolume)
    {
        soundVolume = newVolume;
    }

    // If object is destroyed, remove event subscription
    private void OnDestroy()
    {
        GameEvents.current.onSFXVolumeChange -= UpdateVolumeLevel;
    }
    
    
    /*
    // Start is called before the first frame update
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void PlayPickupSound()
    {
        audioSource.PlayOneShot(pickupToolClip);
    }
    */
}
