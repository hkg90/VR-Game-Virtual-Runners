using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] clips;
    private float soundVolume = 1.0f;

    private AudioSource audioSource;

    // Start is called before the first frame update
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        GameEvents.current.onSFXVolumeChange += UpdateVolumeLevel;
    }

    private void Step()
    {
        AudioClip clip = GetRandomClip();
        audioSource.PlayOneShot(clip, soundVolume);
    }

    private AudioClip GetRandomClip()
    {
        return clips[Random.Range(0, clips.Length)];
    }

    // Updates audio clip's play sound volume per UI Settings page's value
    private void UpdateVolumeLevel(float newVolume)
    {
        soundVolume = newVolume;
    }

    private void OnDestroy()
    {
        GameEvents.current.onSFXVolumeChange -= UpdateVolumeLevel;
    }

}
