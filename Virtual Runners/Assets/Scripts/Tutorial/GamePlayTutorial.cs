using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GamePlayTutorial : MonoBehaviour
{
    public GameObject panelInstructions;
    private bool startFlag = false;
    private List<GameObject> tutorialSections = new List<GameObject>();
    private Component[] instructions;
    private Component[] controllerModels;
    public int sectionIndex = -1;
    public int updatedIndex = -1;


    // Start is called before the first frame update
    private void Start()
    {
        // Find game objects
        instructions = panelInstructions.GetComponentsInChildren(typeof(Text), true);  
        controllerModels = gameObject.GetComponentsInChildren(typeof(Animator), true);         

        // Subscribe to tutorial progression event
        GameEvents.current.onGamePlayTutorial += IncrementIndex;
        GameEvents.current.onGamePause += HideModel;
        GameEvents.current.onGameResume += DisplayModel;
        GameEvents.current.onKillTestSubject += RespawnTestSubject;
        GameEvents.current.onTutorialStart += ResetGamePlayTutorial;
        GameEvents.current.onTutorialReset += EndGamePlayTutorial;

    }

    // Update is called once per frame
    private void Update()
    {
        if(startFlag && (sectionIndex != updatedIndex))
        {
            // If all instructions shown, deactivate components and progress tutorial
            if (updatedIndex >= instructions.Length)
            {
                EndGamePlayTutorial();
                // All game play tutorial parts finished, continue progression of Tutorial
                GameEvents.current.ProgressTutorial();
            }
            // Else, load next part of game play tutorial
            else
            {
                UpdateInsctruction();
                sectionIndex = updatedIndex;
            }            
        }        
    }

    // On game play tutorial end, deactivate components and update flag
    private void EndGamePlayTutorial()
    {
        startFlag = false;
        // Deactivate model and instructions
        if(sectionIndex >= 0)
        {
            instructions[sectionIndex].gameObject.SetActive(false);
            controllerModels[sectionIndex].gameObject.SetActive(false);
        }
        // Deactivate panel
        panelInstructions.SetActive(false);
    }

    // Reset Game Play Tutorial components
    private void ResetGamePlayTutorial()
    {
        //Reset flag and indices
        
        startFlag = false;
        sectionIndex = -1;
        updatedIndex = -1;
    }

    // If test subject dies during tutorial, respawn and unfreeze for next run
    private void RespawnTestSubject()
    {
        StartCoroutine(InitiateTestSubjectRespawn());
    }

    // Coroutine to allow enough time for reset to occur prior to calling
    // TestSubjectRespawn() event

    IEnumerator InitiateTestSubjectRespawn()
    {
        yield return new WaitForSeconds(1f);
        if(startFlag)
        {
            GameEvents.current.TestSubjectRespawn();
        }
    }
    
    // If game is unpaused, re-display controller model as needed
    private void DisplayModel()
    {
        if(startFlag && sectionIndex >=0)
        {
            controllerModels[sectionIndex].gameObject.SetActive(true);
        }
    }

    // If game is paused, hide controller model as needed
    private void HideModel()
    {
        if(startFlag && sectionIndex >=0)
        {
            controllerModels[sectionIndex].gameObject.SetActive(false);
        }        
    }
    
    // Resets all flags and indices values and initiates game play tutorial
    public void StartGamePlayTutorial()
    {
        // Update flag
        startFlag = true;
        // Increment index
        IncrementIndex();
        // Open panel to display instructions
        panelInstructions.SetActive(true);        
        // Start run        
        GameEvents.current.GameResume();        
    }

    // Toggle next part of corresponding instructions and models
    private void UpdateInsctruction()
    {
        // Hide old instruction and model
        if (sectionIndex >= 0)
        {
            instructions[sectionIndex].gameObject.SetActive(false);
            controllerModels[sectionIndex].gameObject.SetActive(false);
        }        
        // Load new instruction and model
        instructions[updatedIndex].gameObject.SetActive(true);
        controllerModels[updatedIndex].gameObject.SetActive(true);
    }

    // Increment index 
    private void IncrementIndex()
    {
        updatedIndex += 1;
    }

    // When destroyed, unsubscribe from event manager
    private void OnDestroy()
    {
        GameEvents.current.onGamePlayTutorial -= IncrementIndex;
        GameEvents.current.onGamePause -= HideModel;
        GameEvents.current.onGameResume -= DisplayModel;
        GameEvents.current.onKillTestSubject -= RespawnTestSubject;
        GameEvents.current.onTutorialStart -= ResetGamePlayTutorial;
        GameEvents.current.onTutorialReset -= EndGamePlayTutorial;
    }
}
