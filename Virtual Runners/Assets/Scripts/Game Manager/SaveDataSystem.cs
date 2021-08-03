// Script Written by Jay Pittenger


using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;


/******************************************************************************************************
 * SaveDataSystem is a class that performs save and load of highscore and tutorial bool data. Data
 * is saved to a files as binary via serialization and is deserialized on load.
 ******************************************************************************************************/


public class SaveDataSystem : MonoBehaviour
{
    public bool GamePlayed;
    private float[][] HighScores;
    public GameObject scoreBoard;
    private float MusicVolume;
    private float SFXVolume;


    // Start is called before the first frame update
    void Start()
    {
        // uncomment this line for testing - previous scores will be deleted at the start of game execution
        //File.Delete(Application.persistentDataPath + "/GameData.dat");

        // event subscription
        GameEvents.current.onSaveHighScores += SaveNewHighScores;
        GameEvents.current.onSFXVolumeChange += SaveSFXVolume;
        GameEvents.current.onMusicVolumeChanged += SaveMusicVolume;

        HighScores = new float[10][];
        for (int i = 0; i < 10; i++)
        {
            HighScores[i] = new float[3];
        }
        // perform intial load of game data
        PerformLoad();
        if (!GamePlayed)
        {
            // FUTURE - trigger tutorial prompt here
            GamePlayed = true;
            // save that tutorial was prompted to user upon first play of game
            PerformSave();
        }
        // if there are scores to load, load scores into game after time to allow game objects to all load
        if(HighScores != null)
        {
            StartCoroutine(UpdateScoreBoardAfterTime(2));
        } else
        {
            scoreBoard.GetComponent<Text>().text = "\n\nNo Scores Yet.";
        }
        // Load volume settings into game after time to allow game objects to all load
        StartCoroutine(UpdateVolumSettingsAfterTime(0.1f));
    }


    // method used to update score board after time so that all scripts have loaded
    IEnumerator UpdateScoreBoardAfterTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        // Send the Event to all Subscribers
        GameEvents.current.PopulateScoreBoard(HighScores);
    }

    // method used to update volume settings after time so that all scripts have loaded
    IEnumerator UpdateVolumSettingsAfterTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        // Send the Event to all Subscribers
        GameEvents.current.MusicVolumeChanged(MusicVolume);
        GameEvents.current.SFXVolumeChange(SFXVolume);
    }

    // Method to save new high score data prior to performing save method
    private void SaveNewHighScores(float[][] scores)
    {
        HighScores = scores;
        PerformSave();
    }

    // Method to save new SFX sound data prior to performing save method
    private void SaveSFXVolume(float newVolume)
    {
        SFXVolume = newVolume;
        PerformSave();
    }

     // Method to save new sound data prior to performing save method
    private void SaveMusicVolume(float newVolume)
    {
        MusicVolume = newVolume;
        PerformSave();
    }

    // Method used to save data to file - creates file if it doesnt exist
    private void PerformSave()
    {
        // if a save file exists, delete it to replace with new file
        if (File.Exists(Application.persistentDataPath + "/GameData.dat"))
        {
            File.Delete(Application.persistentDataPath + "/GameData.dat");
        }
        
        // create save file
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/GameData.dat");

        SaveData data = new SaveData();
        data.savedGamePlayed = GamePlayed;
        data.savedHighScores = HighScores;
        data.savedSFXVolume = SFXVolume;
        data.savedMusicVolume = MusicVolume;        
        bf.Serialize(file, data);
        file.Close();
    }    


    // Method used to load data from file if it exists
    private void PerformLoad()
    {
        // if file exists - get data from file
        if(File.Exists(Application.persistentDataPath + "/GameData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/GameData.dat", FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();
            GamePlayed = data.savedGamePlayed;
            HighScores = data.savedHighScores;
            SFXVolume = data.savedSFXVolume;
            MusicVolume = data.savedMusicVolume;
            // Apply saved data
            GameEvents.current.PopulateScoreBoard(HighScores);            
        }
        else
        {
            HighScores = null;
            GamePlayed = false;
            SFXVolume = 0.5f;
            MusicVolume = 0.5f;
        }
    }


    // Remove event subscriptions on destroy
    private void OnDestroy()
    {
        GameEvents.current.onSaveHighScores -= SaveNewHighScores;
        GameEvents.current.onSFXVolumeChange -= SaveSFXVolume;
        GameEvents.current.onMusicVolumeChanged -= SaveMusicVolume;
    }
}


// internal class to hold data for save and load
[System.Serializable]
internal class SaveData
{
    public bool savedGamePlayed;
    public float[][] savedHighScores;
    public float savedSFXVolume;
    public float savedMusicVolume;
}