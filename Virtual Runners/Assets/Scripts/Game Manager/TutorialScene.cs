using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialScene : MonoBehaviour
{   
    private GameObject testSubjectSystem;  
    private GameObject conveyor;
    private GameObject timer;
    private Component timerScript;
    
    
    // Loads TutorialScene asynchronously
    public void OpenTutorial()
    {
        StartCoroutine(LoadTutorialScene());      
    }
    
    // Loads MainScene asynchronously
    public void CloseTutorial()
    {
        //GameEvents.current.TutorialReset();
        StartCoroutine(LoadMainScene());
    }

    // Starts Tutorial once user selects 'start tutorial' button from UI
    public void StartTutorial()
    {        
        // Call Start tutorial event
        GameEvents.current.StartTutorial();
    }

    // When player quits the tutorial, calls tutorial reset event and resets scene
    // for potential next tutorial run
    public void QuitTutorialPlaythrough()
    {
        GameEvents.current.TutorialReset();   
    }
    
    // Coroutine to load TutorialScene asynchronously
    private IEnumerator LoadTutorialScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Assets/Scenes/TutorialScene.unity");
        // Wait until async scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    // Coroutine to load MainScene asynchronously
    IEnumerator LoadMainScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Assets/Scenes/MainScene.unity");
        // Wait until async scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
