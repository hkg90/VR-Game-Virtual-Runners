using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***************************************************************************************
 * GameEvents is the Event System class for our game.
 * To create a new Event: 
    * public event Action onMyEventName;
 * To add the trigger to the event: 
    * public void MyEventName()
 * Within your event trigger you MUST ALWAYS first check if there are any subscribers for the event:
    * if (onMyEventName != null)
 * If there are subscribers, call the event for all subscribers with: 
    * onMyEvent();
 ***************************************************************************************/
public class GameEvents : MonoBehaviour
{
    // Create the GameEvents object
    public static GameEvents current;

    public float floorZapDelay = 2.0f;

    // Set this instance as the GameEvents object when the script starts
    private void Awake()
    {
        current = this;
    }

    // This event is called any time an object enters the RedZone
    public event Action<int> onRedZoneEnter;
    public void RedZoneEnter(int id)
    {
        // Check if onRedZoneEnter has any Subscribers
        if (onRedZoneEnter != null)
        {
            // Send the Event to all Subscribers, passing the ID of the instance that needs to react to the event
            onRedZoneEnter(id);
        }
    }

    // This event is called any time an object exits the RedZone
    public event Action<int> onRedZoneExit;
    public void RedZoneExit(int id)
    {
        // Check if onRedZoneExit has any Subscribers
        if (onRedZoneExit != null)
        {
            // Send the Event to all Subscribers, passing the ID of the instance that needs to react to the event
            onRedZoneExit(id);
        }
    }

    // This event is called when the Test Subject Enters an Obstacle Collider
    public event Action<int> onTestSubjectEnter;
    public void TestSubjectEnter(int id)
    {
        // Check if onTestSubjectEnter has Subscribers
        if (onTestSubjectEnter != null)
        {
            // Send the Event to all Subscribers, passing the ID of the instance that needs to react to the event
            onTestSubjectEnter(id);
        }
    }

    // This event is called when the Test Subject Exits an Obstacle Collider
    public event Action<int> onTestSubjectExit;
    public void TestSubjectExit(int id)
    {
        // Check if onTestSubject has Subscribers
        if (onTestSubjectExit != null)
        {
            // Send the Event to all Subscribers, passing the ID of the instance that needs to react to the event
            onTestSubjectExit(id);
        }
    }

    // This event is called when a tool enters an obstacles's collider
    public event Action<int, GameObject, Collider> onToolEnter;
    public void ToolEnter(int id, GameObject trigger, Collider collision)
    {
        // Check if onTestSubjectEnter has Subscribers
        if (onToolEnter != null)
        {
            // Send the Event to all Subscribers, passing the ID of the instance that needs to react to the event
            onToolEnter(id, trigger, collision);
        }
    } 

    // This event is called when tool exits an obstacles's collider
    public event Action<int, GameObject, Collider> onToolExit;
    public void ToolExit(int id, GameObject trigger, Collider collision)
    {
        // Check if onTestSubjectEnter has Subscribers
        if (onToolExit != null)
        {
            // Send the Event to all Subscribers, passing the ID of the instance that needs to react to the event
            onToolExit(id, trigger, collision);
        }
    }

    // **This event is called when a tool is grabbed from the tool table by player
    //Invoked by:
    //    user grabs tool

    //Subscribers: ResetToolLocation.cs disables tools on table grabbability state 
    public event Action onToolGrabbed;
    public void ToolGrabbed()
    {
        // Check if onToolGrabbed has Subscribers
        if (onToolGrabbed != null)
        {
            // Send the Event to all Subscribers
            onToolGrabbed();
        }
    }

    // **This event is called when a tool is released by player
    //Invoked by:
    //    user releases tool

    //Subscribers: ResetToolLocation.cs enables tools on table grabbability state 
    public event Action onToolReleased;
    public void ToolReleased()
    {
        // Check if onToolGrabbed has Subscribers
        if (onToolReleased != null)
        {
            // Send the Event to all Subscribers
            onToolReleased();
        }
    }

    // This event is called when the Wire Cutter is destroyed
    public event Action onWireCutterDestroy;
    public void WireCutterDestroy()
    {
        // Check if onWireCutterDestroy has Subscribers
        if (onWireCutterDestroy != null)
        {
            // Send the Event to all Subscribers
            onWireCutterDestroy();
        }
    }
    

    // This event is called when a Tool Interacts with an Obstacle
    public event Action<int, GameObject> onToolObstacleAction;
    public void ToolObstacleAction(int id, GameObject obstacle) 
    {
        // Check if onToolObstacleAction has Subscribers
        if (onToolObstacleAction != null)
        {
            onToolObstacleAction(id, obstacle);
        }

    }

    // **This event is called when the Test Subject Collides with the floor triggers**
    //Invoked by: 
    //    TestSubjectRespawn.cs when test subject passes through floor trigger

    //Subscribers:
    //    LaserSystem.cs to zap test subject and invoke KillTestSubject
    //    TestSubjectMovement.cs to freeze test subject in place when being zapped
    //    ConveyorMovement.cs to freeze conveyor belt
    public event Action onTestSubjectFloorEnter;
    public void TestSubjectFloorEnter()
    {
        // Check if onTestSubjectFloor has Subscribers
        if (onTestSubjectFloorEnter != null)
        {
            StartCoroutine(ExecuteFloorEnterAfterTime(floorZapDelay));
        }
    }
    IEnumerator ExecuteFloorEnterAfterTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        // Send the Event to all Subscribers
        onTestSubjectFloorEnter();
    }

    // **This event is called when the Test Subject is to die**
    //Invoked by: 
    //    FurnaceZoneTrigger.cs when test subject passes through it
    //    LaserSystem.cs when onTestSubjectFloorEnter is invoked

    //Subscribers:
    //    TestSubjectRespawn.cs to reset test subject position and color and freeze in duct
    //    TestSubectMovement.cs to freeze movement and reset collision list for tracking
    //        conveyor movement
    //    ConveyorMovement.cs to freeze conveyor belt
    //    TimerController.cs to stop timer
    public event Action onKillTestSubject;
    public void KillTestSubject()
    {
        // Check if onKillTestSubject has Subscribers
        if (onKillTestSubject != null)
        {
            //Debug.Log("*** onKillTestSubject ***");
            // Send the Event to all Subscribers
            onKillTestSubject();
        }
    }

    // **This event is called when the Test Subject is to respawn**
    //Invoked by: 
    //    invoked by UI scripts

    //Subscribers:
    //    TestSubectMovement.cs to unfreeze "new" test subject to start new run
    //    ConveyorMovement.cs to unfreeze conveyor belt
    //    ConveyorSectionShift.cs to reset position of each conveyor section back to initial
    //    TimerController.cs to reset timer and start timing
    //    ToolManager.cs to remove tools on conveyor belt
    //    MenuManager.cs to update flag state
    public event Action onTestSubjectRespawn;
    public void TestSubjectRespawn()
    {
        // Check if onTestSubjectFloor has Subscribers
        if (onTestSubjectRespawn != null)
        {
            //Debug.Log("*** onTestSubjectRespawn ***");
            // Send the Event to all Subscribers
            onTestSubjectRespawn();
        }
    }

    // **This event is called when the game is paused (by test subject dying or per
    //   user toggling UI open)**
    //Invoked by: 
    //    invoked by UI scripts or event in game
    //Subscribers: ConveyorMovement.cs toggles movement off
    //      TimerController.cs pauses timer count
    //      TestSubjectMovement.cs freezes test subject
    //      HighlighObject.cs disables highlight feature of tools on hover
    //      ResetToolLocation.cs disables tool grabbability and updates flag
    //      ToolConveyorMovement.cs freezes tools on conveyor belt
    //      ToolManager.cs updates flag value
    //      DisplayCues.cs disables hit option for cues (where applicable)
    //      GamePlayTutorial.cs hides models (where applicable)
    //      TutorialManager.cs closes tutorial instruction canvas
    //      MenuManager.cs opens UI menu
    public event Action onGamePause;
    public void GamePause()
    {
        // Check if onTestSubjectFloor has Subscribers
        if (onGamePause != null)
        {
            //Debug.Log("*** onGamePause ***");
            // Send the Event to all Subscribers
            onGamePause();
        }
    }

    // **This event is called when the game is resumed (by test subject respawn or per
    //   user toggling UI open)**
    //Invoked by: 
    //    invoked by UI scripts or event in game
    //Subscribers: ConveyorMovement.cs toggles movement on
    //      TimerController.cs resumes timer count
    //      TestSubjectMovement.cs unfreezes test subject
    //      HighlighObject.cs re-enables highlight feature of tools on hover
    //      ResetToolLocation.cs re-enables tool grabbability and updates flag
    //      ToolConveyorMovement.cs unfreezes tools on conveyor belt
    //      ToolManager.cs updates flag value
    //      DisplayCues.cs re-enables hit option for cues (where applicable)
    //      GamePlayTutorial.cs displays models (where applicable)
    //      TutorialManager.cs opens tutorial instruction canvas (where applicable)
    //      MenuManager.cs closes UI menu
    public event Action onGameResume;
    public void GameResume()
    {
        // Check if onTestSubjectFloor has Subscribers
        if (onGameResume != null)
        {
            //Debug.Log("*** onGameResume ***");
            // Send the Event to all Subscribers
            onGameResume();
        }
    }

    // This event is called when the Conveyor Section Loops back to the start of the belt
    public event Action<GameObject> onLoopConveyorSection;
    public void LoopConveyorSection(GameObject section)
    {
        //Debug.Log("*** onLoopConveyorSection ***");
        // Check if onLoopConveyorSection has Subscribers
        if (onLoopConveyorSection != null)
        {
            // Broadcast the event to all Subscribers
            onLoopConveyorSection(section);
        }
    }

    // This event is called when the an Object leaves the FurnaceZone
    public event Action<GameObject> onFurnaceZoneExit;
    public void FurnaceZoneExit(GameObject obstacle)
    {
        // Check if onFurnaceZoneExit has Subscribers
        if (onFurnaceZoneExit != null)
        {
            onFurnaceZoneExit(obstacle);
        }
    }

    // This event is called when an Object is placed in the map
    public event Action<GameObject> onPlaceObstacle;
    public void PlaceObstacle(GameObject obstacle)
    {
        // Check if onPlaceObstacle has Subscribers
        if (onPlaceObstacle != null)
        {
            onPlaceObstacle(obstacle);
        }
    }


    // **This event is called to start the tutorial
    //Invoked by: 
    //    invoked by UI scripts or event in game
    //Subscribers: ConveyorMovement.cs decrease speed for tutorial and updates flag
    //      ConveyorSectionShift.cs resets conveyor belt
    //      TimerController.cs resets timer and timer's text
    //      TestSubjectRespawn.cs resets test subject
    //      ToolManager.cs clear tools on conveyor
    //      DisplayCues.cs reset cue flags
    //      GamePlayTutorial.cs reset flags
    //      GuidedTutorial.cs reset flags
    //      TutorialForceFieldReset.cs reset materials
    //      TutorialManager.cs starts tutorial
    //      TutorialZoneTriggers.cs resets flags
    //      MenuManager.cs updates flags
    public event Action onTutorialStart;
    public void StartTutorial()
    {
        // Check if onTutorialStart has Subscribers
        if (onTutorialStart != null)
        {
            //Debug.Log("*** onTutorialStart ***");
            // Send the Event to all Subscribers
            onTutorialStart();
        }
    }
    
    // **This event is called when a section of the overall tutorial is completed
    //Invoked by: 
    //    invoked by UI scripts or event in game
    //Subscriber: TutorialManager.cs increments index to progress to next section of tutorial
    public event Action onTutorialProgression;
    public void ProgressTutorial()
    {
        // Check if onTutorialProgression has Subscribers
        if (onTutorialProgression != null)
        {
            //Debug.Log("*** onTutorialProgression ***");
            // Send the Event to all Subscribers
            onTutorialProgression();
        }
    }

    // **This event is called when a part of the guided tutorial is completed
    //Invoked by: 
    //    invoked by event in game
    //Subscriber: GuidedTutorialcs increments index to load next part of tutorial
    public event Action onGuidedTourProgression;
    public void ProgressGuidedTour()
    {
        // Check if onGuidedTourProgression has Subscribers
        if (onGuidedTourProgression != null)
        {
            // Send the Event to all Subscribers
            onGuidedTourProgression();
        }
    }

    // **This event is called when a part of the gameplay tutorial is completed
    //Invoked by: 
    //    invoked by event in game
    //Subscriber: GamePlayTutorial.cs increments index to load next part of tutorial
    public event Action onGamePlayTutorial;
    public void ProgressGamePlayTutorial()
    {
        // Check if onGamePlayTutorial has Subscribers
        if (onGamePlayTutorial != null)
        {
            // Send the Event to all Subscribers
            onGamePlayTutorial();
        }
    }

    // **This event is called when a part of the tutorial is completed
    //Invoked by: 
    //    invoked by event in game
    //Subscriber: ConveyorMovement.cs increases speed of conveyor belt
    //      TutorialManager.cs closes tutorial's instruction UI
    //      MenuManager.cs resets tutorial flags and pauses game
    public event Action onTutorialComplete;
    public void TutorialComplete()
    {
        // Check if onTutorialComplete has Subscribers
        if (onTutorialComplete != null)
        {
            // Send the Event to all Subscribers
            onTutorialComplete();
        }
    }

    // **This event is called when a part of the tutorial is reset
    //Invoked by: 
    //    invoked by UI input or event in game
    //Subscriber: ConveyorMovement.cs resets flag
    //      ToolManager.cs clears tools on conveyor
    //      GamePlayTutorial.cs deactivates instruction UI canvas
    //      GuidedTutorial.cs deactivates instruction UI canvas
    //      TutorialManager.cs deactivates tutorial components and UI canvas
    //      MenuManager.cs resets tutorial flags and pauses game
    public event Action onTutorialReset;
    public void TutorialReset()
    {
        // Check if onTutorialReset has Subscribers
        if (onTutorialReset != null)
        {
            // Send the Event to all Subscribers
            onTutorialReset();
        }
    }

    // **This event is called when the game data is loaded for score board**
    //Invoked by:
    //    SaveDataSystem.cs when highscores are received from data file

    //Subscribers:
    //    HighScore.cs to populate high score board in game with scores
    public event Action<float[][]> onPopulateScoreBoard;
    public void PopulateScoreBoard(float[][] HighScores)
    {
        // Check if onPopulateScoreBoard has Subscribers
        if (onPopulateScoreBoard != null)
        {
            // Send the Event to all Subscribers
            onPopulateScoreBoard(HighScores);
        }
    }

    // **This event is called when HighScores have changed and need to be saved**
    //Invoked by:
    //    HighScore.cs when highscores have updated

    //Subscribers:
    //    SaveDataSystem.cs to update save data with new highscores
    public event Action<float[][]> onSaveHighScores;
    public void SaveHighScores(float[][] HighScores)
    {
        // Check if onSaveHighScores has Subscribers
        if (onSaveHighScores != null)
        {
            // Send the Event to all Subscribers
            onSaveHighScores(HighScores);
        }
    }

    // **This event is called when Timer is stopped in order to record the score**
    //Invoked by:
    //    TimerController.cs when timer has stopped

    //Subscribers:
    //    HighScore.cs to see if score is highscore, and if so add to highscores
    public event Action<int, int, int, string> onRecordScore;
    public void RecordScore(int seconds, int minutes, int hours, string timeString)
    {
        // Check if onRecordScores has Subscribers
        if (onRecordScore != null)
        {
            // Send the Event to all Subscribers
            onRecordScore(seconds, minutes, hours, timeString);

        }
    }

    // **This event is called when score has been processed by highscore system**
    //Invoked by:
    //    HighScore.cs when score has been processed

    //Subscribers:
    //    HighScoreTextController.cs to show text that user just got a high score
    public event Action<bool> onNewHighScore;
    public void NewHighScore(bool isHighScore)
    {
        // Check if onNewHighScore has Subscribers
        if (onNewHighScore != null)
        {
            // Send the Event to all Subscribers
            onNewHighScore(isHighScore);
        }
    }

    // **This event is called when conveyor rate needs to be increased**
    //Invoked by:
    //    TimerController.cs every second

    //Subscribers:
    //    ConveyorMovement.cs increases conveyor speed
    public event Action onIncreaseConveyor;
    public void IncreaseConveyor()
    {
        // Check if onIncreaseConveyor has Subscribers
        if (onIncreaseConveyor != null)
        {
            // Send the Event to all Subscribers
            onIncreaseConveyor();
        }
    }

    // **This event is called any time player changes SFX volume setting from UI or 
    // to establish the SFX volume vale on scene load
    //Invoked by:
    //    user's UI selection in Settings menu

    //Subscribers:
    //    BackgroundMusicChange.cs updates volume value
    //    PickupSound.cs updates volume value
    //    LaserSystem.cs updates volume value
    //    Footsteps.cs updates volume value
    //    TestSubjectEnters.cs updates volume value
    //    TestSubjectMovement.cs updates volume value
    //    WireCutterClose.cs updates volume value
    //    SaveDataSystem.cs saves new sound volume data to .dat file
    //    SetSFXVolume.cs updates slider's value
    public event Action<float> onSFXVolumeChange;
    public void SFXVolumeChange(float volumeValue)
    {
        // Check if onSFXVolumeChange has any Subscribers
        if (onSFXVolumeChange != null)
        {
            // Send the Event to all Subscribers, passing the ID of the instance that needs to react to the event
            onSFXVolumeChange(volumeValue);
        }
    }

    // **This event is called to load background music's sound setting on scene load
    //Invoked by:
    //    user's UI selection in Settings menu

    //Subscribers:
    //    BackgroundMusicChange.cs updates volume value
    //    SaveDataSystem.cs saves new sound volume data to .dat file
    //    SetMusicVolume.cs updates slider's value
    public event Action<float> onMusicVolumeChanged;
    public void MusicVolumeChanged(float volumeValue)
    {
        // Check if onMusicVolumeLoad has any Subscribers
        if (onMusicVolumeChanged != null)
        {
            // Send the Event to all Subscribers, passing the ID of the instance that needs to react to the event
            onMusicVolumeChanged(volumeValue);
        }
    }

    // This event is called to Reset all the Obstacles on the Map when the Test Subject Respwans
    public event Action onResetObstacles;
    public void ResetObstacles()
    {
        // Check if onResetObstacles has Subscribers
        if (onResetObstacles != null)
        {
            // Send the Event to all Subscribers
            onResetObstacles();
        }
    }
}
