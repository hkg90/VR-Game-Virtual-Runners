using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GuidedTutorial : MonoBehaviour
{
    public GameObject guidedInstructionsPanel;
    private int tourIndex = -1; 
    private int updatedTourIndex = -1;
    private bool startFlag = false;    
    delegate void GuidedTourMethod();
    private List<GuidedTourMethod> tourList = new List<GuidedTourMethod>();
    private Component[] tourInstructions;
    private Component[] cuesList;

    
    // Start is called before the first frame update
    private void Start()
    {
        // Subscribe to events
        GameEvents.current.onGuidedTourProgression += IncrementTourIndex;
        GameEvents.current.onTutorialStart += ResetGuidedTutorial;
        GameEvents.current.onTutorialReset += EndGuidedTutorial;
        
        // Add functions to list
        tourList.Add(UpdateInsctruction);
        tourList.Add(UpdateTourCues);

        // Add visual/ audio cues to list
        cuesList = gameObject.GetComponentsInChildren(typeof(Light), true);

        // Get all tour instructions
        tourInstructions = guidedInstructionsPanel.GetComponentsInChildren(typeof(Text), true);
    }

    // Update is called once per frame
    private void Update()
    {        
        if(startFlag && (tourIndex != updatedTourIndex))
        {
            // If completed last part of guided tutorial
            if (updatedTourIndex >= cuesList.Length)
            {
                EndGuidedTutorial();
                // Continue progression of Tutorial
                GameEvents.current.ProgressTutorial();
            }
            // Else load next part of guided tutorial
            else
            {
                // Start next part of tour
                foreach (GuidedTourMethod method in tourList)
                {
                    method();
                }
                tourIndex = updatedTourIndex;
            }            
        }
    }

    // On tutorial completion or forced end, reset components and close UI
    private void EndGuidedTutorial()
    {
        startFlag = false;
        // Deactivate cues and text
        if(tourIndex >= 0)
        {
            cuesList[tourIndex].gameObject.SetActive(false);
            tourInstructions[tourIndex].gameObject.SetActive(false);  
        }
        // Close Guided Instructions Tutorial instructions panel
        guidedInstructionsPanel.SetActive(false);   
    }

    // Toggle part of corresponding guided tour' instruction for UI panel
    private void UpdateInsctruction()
    {
        if (tourIndex >= 0)
        {
            tourInstructions[tourIndex].gameObject.SetActive(false);
        }        
        tourInstructions[updatedTourIndex].gameObject.SetActive(true);
    }
    
    // Update visual and sound for cues for each corresponding guided tour section
    private void UpdateTourCues()
    {
        // Deactivate current cues
        if (tourIndex >= 0)
        {
            cuesList[tourIndex].gameObject.SetActive(false);
            cuesList[tourIndex].GetComponent<DisplayCues>().currentCue = false;
        }
        // Activate next set of cues
        cuesList[updatedTourIndex].gameObject.SetActive(true);
        cuesList[updatedTourIndex].GetComponent<DisplayCues>().currentCue = true;
    }

    // Increment tour index 
    private void IncrementTourIndex()
    {
        updatedTourIndex += 1;
    }

    // Resets currently active panel
    private void ResetGuidedTutorial()
    {
        // Reset flag and indicies
        startFlag = false;
        tourIndex = -1;
        updatedTourIndex = -1;
    }

    // Start environment/ game layout guided tour
    public void StartTour()
    {
        startFlag = true;        
        // Open Guided Instructions Tutorial instructions panel
        guidedInstructionsPanel.SetActive(true);
        IncrementTourIndex();
    }

    // When destroyed, unsubscribe from event manager
    private void OnDestroy()
    {
        GameEvents.current.onGuidedTourProgression -= IncrementTourIndex;
        GameEvents.current.onTutorialStart -= ResetGuidedTutorial;
        GameEvents.current.onTutorialReset -= EndGuidedTutorial;
    }

}
