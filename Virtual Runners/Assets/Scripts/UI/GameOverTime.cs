// Script Written by Jay Pittenger


/******************************************************************************************************
 * GameOverTime is a class that subscribes to RecordScore event and updates score in game over ui menu
 * for recently completed run.
 ******************************************************************************************************/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverTime : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameEvents.current.onRecordScore += PopulateTimeText;
    }

    // Method to populate text for time value of run
    void PopulateTimeText(int seconds, int minutes, int hours, string timeString)
    {
        GetComponent<Text>().text = timeString;
    }

    // Remove event subscriptions on destroy
    private void OnDestroy()
    {
        GameEvents.current.onRecordScore -= PopulateTimeText;
    }
}
