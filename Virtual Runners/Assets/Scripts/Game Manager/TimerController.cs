// Script Written by Jay Pittenger


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/******************************************************************************************************
 * TimeController is a class that handles the game timer for the test subject trial
 ******************************************************************************************************/


public class TimerController : MonoBehaviour
{

    public decimal timeSeconds = 0;
    private decimal lastTimeSeconds = 0;
    private decimal totalTimeSeconds = 0;
    private decimal initialTime = 0;
    public int timeMinutes = 0;
    public int timeHours = 0;
    private decimal pausedTime = 0;
    private decimal timeAtPause = 0;
    private Text timerText;
    private string timeSecondsString;
    private string timeMinutesString;
    private string timeHoursString;
    private bool activeRun = false;
    private bool initialTimeSet = false;
    private bool isPaused = false;


    void Start()
    {
        // get text component of timer
        timerText = gameObject.GetComponent<Text>();
        // subscriptions
        GameEvents.current.onTestSubjectRespawn += StartTimer;
        GameEvents.current.onKillTestSubject += StopTimer;
        GameEvents.current.onGamePause += PauseTimer;
        GameEvents.current.onGameResume += ResumeTimer;
        GameEvents.current.onTutorialStart += ResetInitialTime;
        GameEvents.current.onTutorialStart += ResetTimeText;
    }


    void FixedUpdate()
    {
        // if run is currently happening and game is not paused
        if (activeRun && !isPaused)
        {
            // if the initial runtime has not been set
            if (!initialTimeSet)
            {
                initialTime = decimal.Truncate((decimal)Time.timeSinceLevelLoad) - pausedTime;
                initialTimeSet = true;
            }
            // get current time in seconds
            totalTimeSeconds = decimal.Truncate((decimal)Time.timeSinceLevelLoad) - initialTime - pausedTime;
            // only update text if a second has passed since last update
            if (totalTimeSeconds != lastTimeSeconds)
            {
                TimeTextUpdate();
                GameEvents.current.IncreaseConveyor();
            }
            lastTimeSeconds = totalTimeSeconds;
        }
        // If run is currently paused, track time since pause
        else if(isPaused)
        {
            pausedTime = decimal.Truncate((decimal)Time.timeSinceLevelLoad) - timeAtPause - initialTime;
        }
    }


    private void ResetTimeText()
    {
        timerText.text = "00:00";
    }


    private void TimeTextUpdate()
    {
        // calculate time seconds to display based on total time subtracting minutes
        timeSeconds = totalTimeSeconds - (60 * timeMinutes) - (3600 * timeHours);

        // SECONDS
        // add leading zero if seconds < 10
        if (timeSeconds < 10)
        {
            timeSecondsString = "0" + timeSeconds.ToString();
        }
        else if (timeSeconds == 60)
        {
            timeSecondsString = "00";
            timeSeconds = 0;
            timeMinutes += 1;
        }
        else
        {
            timeSecondsString = timeSeconds.ToString();
        }

        // MINUTES
        // add leading zero if minutes < 10
        if (timeMinutes < 10)
        {
            timeMinutesString = "0" + timeMinutes.ToString();
        }
        // update hours
        else if(timeMinutes == 60)
        {
            timeMinutes = 0;
            timeMinutesString = "00";
            timeHours += 1;
            timerText.fontSize = 40;
        }
        else
        {
            timeMinutesString = timeMinutes.ToString();
        }

        // HOURS
        // add leading zero if hours < 10
        if (timeHours < 10)
        {
            timeHoursString = "0" + timeHours.ToString();
        }
        else
        {
            timeHoursString = timeHours.ToString();
        }

        // display time in text - display minutes:seconds if hours is zero
        if (timeHours == 0)
        {
            timerText.text = timeMinutesString + ":" + timeSecondsString;
        } 
        // display hours:minutes:seconds
        else
        {
            timerText.text = timeHoursString + ":" + timeMinutesString + ":" + timeSecondsString;
        }
    }


    // Starts timer run
    private void StartTimer()
    {
        timerText.fontSize = 60;
        timeMinutes = 0;
        timeHours = 0;
        activeRun = true;
        initialTimeSet = false;
        pausedTime = 0;
        timeAtPause = 0;
    }


    // Pauses timer on game pause event call
    private void PauseTimer()
    {
        timeAtPause = totalTimeSeconds;
        isPaused = true;
    }


    // Resumes timer on game resume event call
     private void ResumeTimer()
    {
        activeRun = true;
        isPaused = false;
        timeAtPause = 0;
    }


    // Resets timer's start time on tutorial reset game event call
    private void ResetInitialTime()
    {
        timeMinutes = 0;
        timeHours = 0;
        pausedTime = 0;
        timeAtPause = 0;
        initialTimeSet = false;
    }


    // Stops timer and calls record score game event
    private void StopTimer()
    {
        if (activeRun)
        {
            string totalTimeString;
            if (timeHours == 0)
            {
                totalTimeString = timeMinutesString + ":" + timeSecondsString;
            }
            // display hours:minutes:seconds
            else
            {
                totalTimeString = timeHoursString + ":" + timeMinutesString + ":" + timeSecondsString;
            }
            GameEvents.current.RecordScore((int)timeSeconds, timeMinutes, timeHours, totalTimeString);
            activeRun = false;
        }
    }


    // Remove event subscriptions on destroy
    private void OnDestroy()
    {
        GameEvents.current.onTestSubjectRespawn -= StartTimer;
        GameEvents.current.onKillTestSubject -= StopTimer;
        GameEvents.current.onGamePause -= PauseTimer;
        GameEvents.current.onGameResume -= ResumeTimer;
        GameEvents.current.onTutorialStart -= ResetInitialTime;
        GameEvents.current.onTutorialStart -= ResetTimeText;
    }
}
