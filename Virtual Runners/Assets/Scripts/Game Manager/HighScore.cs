// Script Written By Jay Pittenger


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/******************************************************************************************************
 * HighScore is a class that performs highscore check on a timer results and displays top 10 scores on board
 * in game
 ******************************************************************************************************/


public class HighScore : MonoBehaviour
{
    private float seconds;
    private float minutes;
    private float hours;
    private float[][] HighScores;


    void Start()
    {
        HighScores = new float[10][];
        for(int i = 0; i < 10; i++)
        {
            HighScores[i] = new float[3];
        }
        // event subscription
        GameEvents.current.onRecordScore += IsHighScore;
        GameEvents.current.onPopulateScoreBoard += UpdateHighScore;
    }


    // Method used to check if new score belongs in top 10 high scores
    private void IsHighScore(int timeSeconds, int timeMinutes, int timeHours, string timeString)
    {
        bool highScore = false;
        seconds = timeSeconds;
        minutes = timeMinutes;
        hours = timeHours;

        float[] scoreTime = { hours, minutes, seconds };
        // if highscore list is not full, add to end of list for sorting later
        if (HighScores.Length < 10)
        {
            HighScores[HighScores.Length] = scoreTime;
            highScore = true;
        }
        //  if list is full, find out of score time is longer than shortest time in list
        else if(HighScores[9][0] <= hours)
        {
            // if the new score has longer hours, replace last element with score
            if(HighScores[9][0] < hours)
            {
                HighScores[9] = scoreTime;
                highScore = true;
            }
            // equal hours - check minutes and seconds
            else
            {
                // make sure minutes is equal to or greater than current minutes
                if(HighScores[9][1] <= minutes)
                {
                    // if the new score has longer minutes, replace last element with score
                    if (HighScores[9][1] < minutes)
                    {
                        HighScores[9] = scoreTime;
                        highScore = true;
                    }
                    // equal minutes - check seconds
                    else
                    {
                        // if the seconds are longer than current seconds, replace
                        if (HighScores[9][2] < seconds)
                        {
                            HighScores[9] = scoreTime;
                            highScore = true;
                        }
                    }
                }
            }
        }
        // if new score has been added to high score list - resort list to place in correct spot
        if (highScore)
        {
            // sort highscore array 
            InsertionSortTime();
            // update highscore board in game
            UpdateHighScore(HighScores);

            // trigger highscore save
            GameEvents.current.SaveHighScores(HighScores);

            // trigger event for new high score added - true
            GameEvents.current.NewHighScore(true);
        } else
        {
            GameEvents.current.NewHighScore(false);
        }
    }


    // Method used to update high score on score board in game
    private void UpdateHighScore(float[][] ScoreArray)
    {
        HighScores = new float[10][];
        for (int i = 0; i < 10; i++)
        {
            HighScores[i] = new float[3];
        }
        HighScores = ScoreArray;
        if (ScoreArray == null)
        {
            HighScores = new float[10][];
            for (int i = 0; i < 10; i++)
            {
                HighScores[i] = new float[3];
            }
        }
        // update score board in game
        bool hoursInScores = false;
        //  if highest scores is in hours, include hours in all scores
        if(HighScores[0][0] > 0)
        {
            hoursInScores = true;
        }
        string scoreText = "";
        string hoursString;
        string minutesString;
        string secondsString;
        for(int i = 0; i < HighScores.Length; i++){
            
            if (HighScores[i][0] == 0 && HighScores[i][1] == 0 && HighScores[i][2] == 0)
            {
                break;
            }
            // add leading zero for hours if needed
            if (HighScores[i][0] < 10)
            {
                hoursString = "0" + HighScores[i][0].ToString();
            } else
            {
                hoursString = HighScores[i][0].ToString();
            }
            // add leading zero for minutes if needed
            if (HighScores[i][1] < 10)
            {
                minutesString = "0" + HighScores[i][1].ToString();
            } else
            {
                minutesString = HighScores[i][1].ToString();
            }
            // add leading zero for seconds f needed
            if (HighScores[i][2] < 10)
            {
                secondsString = "0" + HighScores[i][2].ToString();
            } else
            {
                secondsString = HighScores[i][2].ToString();
            }
            if (hoursInScores)
            {
                scoreText += "\n" + hoursString + ":" + minutesString + ":"  + secondsString;
            } else
            {
                scoreText += "\n" + minutesString + ":" + secondsString;
            } 
            
        }
        GetComponent<Text>().text = scoreText;
    }


    // Method peforms insertion sort based on max time in the HighScores 2D array
    private void InsertionSortTime()
    {
        for (int i = 0; i < HighScores.Length; i++)
        {
            // get current time in  seconds
            float[] currTime = HighScores[i];
            float elSec = (HighScores[i][0] * 3600) + (HighScores[i][1] * 60) + HighScores[i][2];
            int k = i - 1;
            //iterate over previous elements until time is greater than or equal to elSec
            while (k >= 0 && ((HighScores[k][0] * 3600) + (HighScores[k][1] * 60) + HighScores[k][2]) < elSec)
            {
                // shift elements to the right to make room for el
                HighScores[k + 1] = HighScores[k];
                k -= 1;
            }
            HighScores[k + 1] = currTime;
        }
    }


    // Remove event subscriptions on destroy
    private void OnDestroy()
    {
        GameEvents.current.onRecordScore -= IsHighScore;
        GameEvents.current.onPopulateScoreBoard -= UpdateHighScore;
    }
}
