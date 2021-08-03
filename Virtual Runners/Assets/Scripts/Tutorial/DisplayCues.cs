using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayCues : MonoBehaviour
{   
    private Light lightCue;
    private AudioSource audioCue;
    public bool currentCue = false; // Flag toggled by GuidedTutorial.cs, UpdateTourCues()
    public bool viewedByPlayer = false;
    private bool delayCalled = false;
    private bool tutorialPaused = false;     
    public float secondsDelayed = 1f;
    private float timeStart;
    private float currentTime;
    private float elapsedNewTime;


    // Start is called before the first frame update
    private void Start()
    {
        GameEvents.current.onTutorialStart += ResetFlags;
        GameEvents.current.onGamePause += PauseTutorialCues;
        GameEvents.current.onGameResume += ResumeTutorialCues;

        lightCue = gameObject.GetComponent<Light>();
        audioCue = gameObject.GetComponent<AudioSource>();
        timeStart = Time.time;
    }
    
    // Update is called once per frame
    private void Update()
    {
        currentTime = Time.time;
        elapsedNewTime = currentTime - timeStart;
        // If player's HMD sees tutorial object, stop toggling cues 
        if(currentCue && viewedByPlayer && !delayCalled && !tutorialPaused)
        {
            // Leave light on while player's looking at object
            gameObject.GetComponentInChildren<Light>().enabled = true;
            StartCoroutine(NextSection());
        }
        // If game paused, pause toggling cues
        else if(currentCue && tutorialPaused)
        {
            return;
        }      
        // If player has NOT seen tutorial object, get player's attention to look at tutorial object
        else if (currentCue && ((elapsedNewTime) > 1) && !viewedByPlayer)
        {
            timeStart = currentTime;
            ToggleCues(); 
        }
    }

    // If hit by HMD ray, update flag to true
    public void OnRayHit()
    {
        viewedByPlayer = true;
    }
    
    // Until player looks at object, flash light and play audio cue to get their attention
    private void ToggleCues()
     {
        // Toggle light and sound cue        
        lightCue.enabled = !(lightCue.enabled);
        audioCue.Play();
     }

    // When tutorial is paused (UI is open), pause cue toggles
    private void PauseTutorialCues()
    {
        tutorialPaused = true;
    }

    // After tutorial is resumed (UI is just closed), resume cue toggles
    private void ResumeTutorialCues()
    {
        tutorialPaused = false;
    }

    // When tutorial started, reset flags to initial state
    private void ResetFlags()
    {
        viewedByPlayer = false;
        delayCalled = false;
        currentCue = false;
        lightCue.enabled = true;    
        tutorialPaused = false;
    }
    
    // Async function to delay calling next part of guided tour
    IEnumerator NextSection()
    {
        // delayedCalled flag ensures NextSection() is only called once
        delayCalled = true;
        // Delay calling next part of guided tour to allow user to look at object and UI screen
        yield return new WaitForSeconds(secondsDelayed);
        // Deactivate visual cue
        lightCue.enabled = false;      
        // Call event
        GameEvents.current.ProgressGuidedTour();
    }

    // When destroyed, unsubscribe from event manager
    private void OnDestroy()
    {
        GameEvents.current.onTutorialStart -= ResetFlags;
        GameEvents.current.onGamePause -= PauseTutorialCues;
        GameEvents.current.onGameResume -= ResumeTutorialCues;
    }
}
