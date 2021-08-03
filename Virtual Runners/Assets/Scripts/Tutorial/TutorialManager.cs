using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialCanvas;
    private int tutorialIndex = -1; 
    private int updatedIndex = -1;
    public Canvas tutorialTitles;
    private GameObject testSubjectSystem;
    private GameObject timer;
    private Component timerScript;
    private GameObject conveyor;
    private GameObject conveyorScript;
    private bool isTutorialOngoing = false;
    //private bool isTutorialComplete = false;
    private Component[] sectionTitles;
    delegate void ProgressTutorialMethod();
    private List<ProgressTutorialMethod> sectionList = new List<ProgressTutorialMethod>();    
    
    
    // Start is called before the first frame update
    void Start()
    {        
        GameEvents.current.onTutorialProgression += IncreaseIndex;
        GameEvents.current.onTutorialStart += StartTutorial;
        GameEvents.current.onTutorialComplete += CloseTutorialUI;
        GameEvents.current.onTutorialReset += EndTutorial;
        GameEvents.current.onGamePause += CloseTutorialUI;        
        GameEvents.current.onGameResume += OpenTutorialUI;

        // Initially deactivate test subject system and convyeor belt script to ensure
        // UI toggling does not create issues with onGamePause/Resume event calls
        testSubjectSystem = GameObject.Find("TestSubjectSystem");       
        conveyor = GameObject.Find("ConveyorBelt");         
        timer = GameObject.Find("Timer");  
        timerScript = timer.GetComponentInChildren<TimerController>();

        // Create a list of delegate objects as placeholders for the different methods
        // that will progress the tutorial.         
        sectionList.Add(GuidedTutorialWalkthrough);
        sectionList.Add(GamePlayTutorial);
        
        // Get list of tutorial section titles
        sectionTitles = tutorialTitles.GetComponentsInChildren(typeof(Text), true);
    }
    
    // Update is called once per frame
    void Update()
    {
        // If tutorial is completed
        if (isTutorialOngoing && (updatedIndex >= sectionTitles.Length))
        {
            EndTutorial();
            // Call tutorial completed game event
            GameEvents.current.TutorialComplete(); 
        }        
        // Else if tutorial is ongoing, and tutorial progress was called
        else if(isTutorialOngoing && tutorialIndex != updatedIndex)
        {           
            // Update title of tutorial
            UpdateTutorialTitle();
            // Call section of tutorial
            sectionList[updatedIndex]();
            // Update index marker
            tutorialIndex = updatedIndex;          
        }
    }

    // Once tutorial ends, updates flags and tutorial components
    void EndTutorial()
    {
        // Deactivate last section's UI title 
        sectionTitles[tutorialIndex].gameObject.SetActive(false);
        // Deactivate tutorial UI canvas
        CloseTutorialUI();
        // Update flag
        isTutorialOngoing = false;         
        // Deactivate game play tutorial components
        DeactivateComponents();
    }

    // On reset, reset indices and flag for next tutorial playthrough
    void StartTutorial()
    {
        isTutorialOngoing = true;   
        tutorialIndex = -1;
        updatedIndex = -1;
        IncreaseIndex();        
    }
    
    // Per called event, opens Tutorial canvas when Main Menu UI is not open
    void OpenTutorialUI()
    {
        if(isTutorialOngoing)
        {
            tutorialCanvas.SetActive(true);
        }        
    }

    // Per called event, closes Tutorial canvas when Main Menu UI is open
    void CloseTutorialUI()
    {
        if(isTutorialOngoing)
        {
            tutorialCanvas.SetActive(false);
        }       
    }

    // Changes Tutorial Canvas's titles as tutorial progresses
    void UpdateTutorialTitle()
    {
        if (tutorialIndex >=0)
        {
            sectionTitles[tutorialIndex].gameObject.SetActive(false);
        }        
        sectionTitles[updatedIndex].gameObject.SetActive(true);
    }
    
    // Start guided tutorial walkthrough of environment
    void GuidedTutorialWalkthrough()
    {
        // Deactivate components not used in guided tutorial
        DeactivateComponents();
        gameObject.GetComponentInChildren<GuidedTutorial>().StartTour();
    }

    // Start game play tutorial
    void GamePlayTutorial()
    {
        // Reactivate components needed for game play tutorial
        ReactivateComponents();
        // Start game play tutorial section
        gameObject.GetComponentInChildren<GamePlayTutorial>().StartGamePlayTutorial();
    }

    // Deactivate game play components (timer, test subject system, conveyor)
    private void DeactivateComponents()
    {
        // Deactivate test subject system for Guided Tour Tutorial - remove UI toggling conflicts        
        if (testSubjectSystem != null)
        {
            testSubjectSystem.SetActive(false);
        }        
        // Deactivate conveyor belt script - remove UI toggling conflicts        
        if(conveyor != null)
        {
            conveyor.GetComponent<ConveyorMovement>().enabled = false;
        }
        //Deactivate timer script - remove UI toggling conflicts
        if(timerScript != null)
        {
            timerScript.GetComponent<TimerController>().enabled = false;
        }
    }

    // Deactivate game play components (timer, test subject system, conveyor)
    private void ReactivateComponents()
    {
        // Deactivate test subject system for Guided Tour Tutorial - remove UI toggling conflicts        
        if (testSubjectSystem != null)
        {
            testSubjectSystem.SetActive(true);
        }        
        // Deactivate conveyor belt script - remove UI toggling conflicts        
        if(conveyor != null)
        {
            conveyor.GetComponent<ConveyorMovement>().enabled = true;
        }
        //Deactivate timer script - remove UI toggling conflicts
        if(timerScript != null)
        {
            timerScript.GetComponent<TimerController>().enabled = true;
        }
    }
    
    // Increment index
    void IncreaseIndex()
    {
        updatedIndex += 1;
    }

    // When destroyed, unsubscribe from event manager
    private void OnDestroy()
    {
        GameEvents.current.onTutorialProgression -= IncreaseIndex;
        GameEvents.current.onTutorialStart -= StartTutorial;
        GameEvents.current.onTutorialComplete -= CloseTutorialUI;
        GameEvents.current.onTutorialReset -= EndTutorial;
        GameEvents.current.onGamePause -= CloseTutorialUI;        
        GameEvents.current.onGameResume -= OpenTutorialUI;
    }
}
