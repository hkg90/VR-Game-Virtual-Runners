using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSubjectEnter : MonoBehaviour
{
    public int id;

    public Material ActiveMaterial;
    public Material InactiveMaterial;

    private AudioSource audioSource;
    [SerializeField]
    private AudioClip forcefieldCollideSound;
    private AudioClip pitOfDeathCollideSound;
    private AudioClip blockCollideSound;

    // Start is called before the first frame update
    void Start()
    {
        id = GetInstanceID();
        GameEvents.current.onTestSubjectEnter += OnTestSubjectEnter;
        GameEvents.current.onTestSubjectExit += OnTestSubjectExit;
        GameEvents.current.onSFXVolumeChange += UpdateVolumeLevel;
        audioSource = GetComponent<AudioSource>();
    }

    // Receive OnPitOfDeathEnter events
    private void OnTestSubjectEnter(int id)
    {
        if (id == this.id)
        {
            DoSplashEffect();
        }
    }

    private void DoSplashEffect()
    {
        gameObject.GetComponent<MeshRenderer>().material = ActiveMaterial;
        // Play collision sound
        if (gameObject.name == "Forcefield")
        {
            audioSource.loop = true;
            audioSource.clip = forcefieldCollideSound;
            audioSource.Play();
        }
        else if (gameObject.name == "Acid")
        {
            // Kill the test subject
            GameEvents.current.KillTestSubject();
        }
    }

    // Receive OnPitOfDeathExit events
    private void OnTestSubjectExit(int id)
    {
        if (id == this.id)
        {
            DoExitPit();
        }
    }

    private void DoExitPit()
    {
        // Stop Forcefield Sound
        if (gameObject.name == "Forcefield")
        {
            audioSource.loop = false;
            audioSource.Stop();
        }
        gameObject.GetComponent<MeshRenderer>().material = InactiveMaterial;
    }

    // Updates audio clip's play sound volume per UI Settings page's value
    private void UpdateVolumeLevel(float newVolume)
    {
        if(audioSource != null)
        {
            audioSource.volume = newVolume;
        }        
    }

    private void OnDestroy()
    {
        GameEvents.current.onTestSubjectEnter -= OnTestSubjectEnter;
        GameEvents.current.onTestSubjectExit -= OnTestSubjectExit;
        GameEvents.current.onSFXVolumeChange -= UpdateVolumeLevel;
    }
}
