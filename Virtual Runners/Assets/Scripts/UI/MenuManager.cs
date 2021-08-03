using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    private float trigger;
    public MenuPanel currentPanel = null;
    private MenuPanel gameOverPanel = null;
    private bool startButtonPressed = false;
    public bool menuOpen = false;
    private bool toggleMenuCalled = false;
    public bool isGameOver = false;  
    public bool isGameOngoing = false;
    public bool isTutorialOngoing = false;
    public bool isTutorialCompleted = false;
    public List<MenuPanel> panelHistory = new List<MenuPanel>(); // Tracks previous UI page(s)
    public MenuPanel[] allPanels; // Stores all menu pages/  panels in UI


    // Start is called before the first frame update
    private void Start()
    {
        // Initialize menu panels 
        SetupMenuPanels();

        // Subscribe to events
        GameEvents.current.onKillTestSubject += UpdateGameOver;
        GameEvents.current.onTestSubjectRespawn += StartGame;
        GameEvents.current.onGamePause += ToggleMenuOpenUI;
        GameEvents.current.onGameResume += ToggleMenuCloseUI;
        GameEvents.current.onTutorialComplete += ResetTutorial;
        GameEvents.current.onTutorialReset += TutorialEnd;
        GameEvents.current.onTutorialStart += TutorialStart;

        // Load UI and pause game
        menuOpen = true;
        GameEvents.current.GamePause();
    }

    // FixedUpdate is called every fixed frame-rate frame
    private void FixedUpdate()
    {
        // Calls update for OVR input refresh for left controller's Start button
        OVRInput.Update();
        startButtonPressed = OVRInput.Get(OVRInput.RawButton.Start);
        
        // If controller's menu button pressed 
        if (startButtonPressed && !toggleMenuCalled) 
        {
            // If menu isn't open, open UI and pause game
            if (!menuOpen)  
            {
                GameEvents.current.GamePause();
            }            
            // If UI is already open, close UI and resume game
            else if (menuOpen && !isGameOver && (isGameOngoing|| isTutorialOngoing)) 
            {
                GameEvents.current.GameResume();
            }
            toggleMenuCalled = true;            
        }
        // If button not pressed, update flag
        else if (!startButtonPressed)
        {
            toggleMenuCalled = false;
        }
    }

    // Handles set up of menu pages/ panels
    private void SetupMenuPanels()
    {
       allPanels = GetComponentsInChildren<MenuPanel>();

        // Establish MenuManager instance for each menu panel
        foreach (MenuPanel panel in allPanels)
        {
            // Send Setup() function in MenuPanel.cs script menu manager instance
            panel.Setup(this);
            if (panel.name == "Panel_GameOver")
            {
                gameOverPanel = panel;
            }
        }
    }

    // Return to back to previously visited page when "back" button selected
    public void GoToPrevious()
    {
        // Check to see if any stored panels in panelHistory list
        if(panelHistory.Count == 0)
        {
            // If no panels stored in history, do nothing            
            return;
        }        
        // If history exists, set current page to desired panel and remove
        // panel from history list
        int lastPageIndex = panelHistory.Count - 1;
        SetCurrent(panelHistory[lastPageIndex]);
        panelHistory.RemoveAt(lastPageIndex);
    }

    // Tracks menu UI navigation history
    public void SetCurrentWithHistory(MenuPanel newPanel)
    {
        panelHistory.Add(currentPanel);
        SetCurrent(newPanel);
    }

    // Updates currently visible UI panel
    public void SetCurrent(MenuPanel newPanel)
    {
        if (currentPanel != null)
        {
            currentPanel.Hide();
        }        
        currentPanel = newPanel;
        currentPanel.Show();
        menuOpen = true;
    }

    // Loads and hides Main Menu UI panel per toggle state
    public void ToggleMenuOpenUI()
    {
        // Load tutorial in progress UI page
        if(isTutorialOngoing && !isTutorialCompleted)
        {
            // Set current page to Tutorial inprogress page
            SetCurrent(allPanels[3]);
        }
        // Load tutorial completed UI page
        else if(isTutorialCompleted)
        {
            // Set current page to Tutorial complete page
            SetCurrent(allPanels[4]);
        }
        // Load main menu initial UI page
        else if (!isGameOver && !isGameOngoing)
        {
            SetCurrent(allPanels[0]);
        }
        // Load game paused UI page if game is ongoing
        else if (!isGameOver && isGameOngoing)
        {
            SetCurrent(allPanels[1]);
        }
        // Load game over UI page
        else if(isGameOver)
        {
            SetCurrent(gameOverPanel);
        }
        menuOpen = true;    
    }

    // When tutorial is completed, reset components for next potential run
    public void ResetTutorial()
    {
        isGameOver = false;        
        isTutorialOngoing = false;
        isTutorialCompleted = true;
        // Call UI & freeze tools
        GameEvents.current.GamePause();       
    }

    // On Tutorial start, set flag to TRUE
    private void TutorialStart()
    {
        isTutorialOngoing = true;
        isTutorialCompleted = false;
    }

    // On Tutorial end, reset flags
    private void TutorialEnd()
    {
        isTutorialCompleted = false;
        isTutorialOngoing = false;
        // Call UI & freeze tools
        GameEvents.current.GamePause();   
    }

    // Close UI canvas and clear UI panel history
    public void ToggleMenuCloseUI()
    {
        // Clear panel history
        panelHistory = new List<MenuPanel>();
        menuOpen = false;
        // Hide UI
        if (currentPanel != null)
        {
            currentPanel.Hide();
        }         
    }
    
    // If a run is started, sets flag to true
    private void StartGame()
    {
        isGameOngoing = true;
    }

    // If player selects 'Continue' button in UI, close menu and continue run
    public void ContinueGame()
    {
        ToggleMenuCloseUI();
        GameEvents.current.GameResume();
    }

    // Updates to game over status for main game play (not for tutorial game play)
    private void UpdateGameOver()
    {
        // If test subject died in regular game, update game over flag and 
        // open UI with Game Over panel
        if(!isTutorialOngoing && !isTutorialCompleted)
        {
            isGameOver = true;
            isGameOngoing = false;
            GameEvents.current.GamePause();
        }        
    }    
    
    // If player selects Restart/ New Game button
    public void Restart()
    {
        // Call test subject respawn and game unpause event
        GameEvents.current.KillTestSubject();
        isGameOver = false;
        GameEvents.current.TestSubjectRespawn();
        GameEvents.current.GameResume();
    }

    // If player selects Quit button - exit application
    public void QuitGame()
    {
        Application.Quit();
        // Applies specifically to quitting an application run in unity editor
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    // When destroyed, unsubscribe from event manager
    private void OnDestroy()
    {
        GameEvents.current.onKillTestSubject -= UpdateGameOver;
        GameEvents.current.onTestSubjectRespawn -= StartGame;
        GameEvents.current.onGamePause -= ToggleMenuOpenUI;
        GameEvents.current.onGameResume -= ToggleMenuCloseUI;
        GameEvents.current.onTutorialComplete -= ResetTutorial;
        GameEvents.current.onTutorialReset -= TutorialEnd;
        GameEvents.current.onTutorialStart -= TutorialStart;
    }
}
