// Script Written by Jay Pittenger


/******************************************************************************************************
 * HighScoreTextController is a class that subscribes to NewHighScore in order to update game over UI
 * with new high score text if needed
 ******************************************************************************************************/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreTextController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameEvents.current.onNewHighScore += PopulateHighScoreText;
    }

    // Updates highscore text in UI with new run time (if applicable)
    void PopulateHighScoreText(bool isHighScore)
    {

        // implement flashing of highscore text
        if (isHighScore)
        {
            StartCoroutine(ExecuteAfterTime(0.5f, 4));
        } 
        // not a highscore, dont display any text
        else
        {
            GetComponent<Text>().text = "";
        } 
    }


    // Recursive method for flashing text that performs count flashes with seconds delay between them 
    IEnumerator ExecuteAfterTime(float seconds, int count)
    {
        yield return new WaitForSeconds(seconds);
        if(GetComponent<Text>().text == "")
        {
            GetComponent<Text>().text = "New High Score!";
        } else
        {
            GetComponent<Text>().text = "";
        }
        if(count > 0)
        {
            StartCoroutine(ExecuteAfterTime(0.5f, count - 1));
        }
    }


    // Remove event subscriptions on destroy
    private void OnDestroy()
    {
        GameEvents.current.onNewHighScore -= PopulateHighScoreText;
    }
}
