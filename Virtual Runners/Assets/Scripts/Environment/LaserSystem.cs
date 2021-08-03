// Script Written by Jay Pittenger


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/******************************************************************************************************
 * LaserSystem is a class that performs the zapping on the test subject when the test subject lands
 * on the floor. Also triggers respawn of test subject when landing on the floor
 ******************************************************************************************************/


public class LaserSystem : MonoBehaviour
{
    public GameObject TestSubject;
    public float zapTime = 1.0f;
    public AudioClip laserClip;
    private float soundVolume = 1.0f;
    private AudioSource audioSource;


    // Start is called before the first frame update
    void Start()
    {
        // subscribe to game events
        GameEvents.current.onTestSubjectFloorEnter += OnZapTestSubject;
        GameEvents.current.onSFXVolumeChange += UpdateVolumeLevel;

        // disable laser
        GetComponent<LineRenderer>().enabled = false;

        audioSource = gameObject.GetComponent<AudioSource>();
    }


    // performs zapping of test subject and triggers respawn of test subject
    private void OnZapTestSubject()
    {
        // only zap if close to the floor - button respawning will cause laser not to fire
        if (TestSubject.transform.position.y < 0.6f)
        {
            GetComponent<LineRenderer>().enabled = true;
            GetComponent<LineRenderer>().SetPosition(1, TestSubject.transform.position);

            // play zap sound
            audioSource.PlayOneShot(laserClip, soundVolume);

            StartCoroutine(EndZapAfterTime(zapTime));
        }
    }

    IEnumerator EndZapAfterTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        // hide beam and trigger respawn
        EndZapTestSubject();
    }


    // trigger respawn of test subject and hide laser
    private void EndZapTestSubject()
    {
        GetComponent<LineRenderer>().enabled = false;
        // call test subject kill
        GameEvents.current.KillTestSubject();
    }

    // Updates audio clip's play sound volume per UI Settings page's value
    private void UpdateVolumeLevel(float newVolume)
    {
        soundVolume = newVolume;
    }
    

    // Remove event subscriptions on destroy
    private void OnDestroy()
    {
        GameEvents.current.onTestSubjectFloorEnter -= OnZapTestSubject;
        GameEvents.current.onSFXVolumeChange -= UpdateVolumeLevel;
    }
}
