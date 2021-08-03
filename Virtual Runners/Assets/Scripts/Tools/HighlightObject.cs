using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages material state of tool object. When hit with raycast, the tool will change colors to the highlight color
public class HighlightObject : MonoBehaviour
{
    public Material woodLight;
    public Material woodDark;
    private bool flag = false;
    private  MeshRenderer toolRenderer;
    private bool gamePaused = false;
 

    // Start is called before the first frame update
    void Start()
    {
        toolRenderer = gameObject.GetComponent<MeshRenderer>();    
        // Subscribe to events
        GameEvents.current.onGamePause += GamePaused;
        GameEvents.current.onGameResume += GameResumed;
        GamePaused();
    }
    
    // Update is called once per frame
    void Update()
    {
        // Update flag to false after ray cast ends
        if(flag){
            flag = false;
        }
        else{
            OnRayExit();
        }
    }
    
    // When ray cast hits, update material to highlight color
    public void OnRayHit()
    {       
        if(!gamePaused)
        {
            flag = true;
            int range = gameObject.GetComponent<MeshRenderer>().materials.Length;
            
            for (int i = 0; i < range; i++)
            {
                gameObject.GetComponent<MeshRenderer>().materials[i].color = Color.cyan;
            }
        }
    }

    // When ray cast ends, update material to original colors
    void OnRayExit()
    {
        int range = toolRenderer.materials.Length;        
        toolRenderer.materials[0].color = woodLight.color;        
        toolRenderer.materials[1].color = woodDark.color;
    }

    // When game is paused, update flag
    private void GamePaused()
    {
        gamePaused = true;
    }
    
    // When game is resumed, update flag
    private void GameResumed()
    {
        gamePaused = false;
    }

    // If object is destroyed, unsubscribe from events
    private void OnDestroy()
    {
        GameEvents.current.onGamePause -= GamePaused;
        GameEvents.current.onGameResume -= GameResumed;
    }
}
