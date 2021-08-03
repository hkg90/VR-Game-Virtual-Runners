using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundManager
{
    public enum AudioClipID { 
        METAL_FOOTSTEPS = 0,
        WOOD_FOOTSTEPS = 1
    };


    public static AudioClip MetalFootsteps;
    public static AudioClip WoodFootsteps;
    
    
    public static void PlaySound(AudioClipID _clipID)
    {
        GameObject soundGameObject = new GameObject("Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        AudioClip someClip;
        switch ((int)_clipID)
        {
            case 0:
                someClip = MetalFootsteps;
                break;
            case 1:
                someClip = WoodFootsteps;
                break;
            default:
                someClip = null;
                break;
        };

        audioSource.PlayOneShot(someClip);

    }
}
