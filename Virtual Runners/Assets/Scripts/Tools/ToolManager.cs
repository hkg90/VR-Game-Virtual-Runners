using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script manages the initial generation of tools & tool table when the game loads 
// and regeneration of tools onto the tool table.
public class ToolManager : MonoBehaviour
{
    // Public variables of tools and tool table prefabs
    
    private bool toolsGrabbaleKinematic;
    private bool toolsGrabbaleCollider;
    private bool gamePaused = false;
    public GameObject toolTable;
    public GameObject bridge;
    public GameObject wireCutter;
    public List<GameObject> blocks;

    // Stores tool place holder spawn positions
    public List<Vector3> toolPositions;
    public Transform[] temp;
    private int randomNumber;
        
    // Start is called before the first frame update
    void Start()
    {
        // Create tool table in scene        
        Instantiate(toolTable, toolTable.transform.position, toolTable.transform.rotation);
        
        // Get tool place holder transform positions        
        temp =  toolTable.GetComponentsInChildren<Transform>();
        int range = temp.Length;
        int index = 0;

        // Save tool place holder spawn positions
        for (int i = 1; i < range; i++) 
        {
            toolPositions.Add(temp[i].position);        
            index++;
        }

        // Get states
        toolsGrabbaleCollider = bridge.GetComponent<BoxCollider>().enabled;
        toolsGrabbaleKinematic = bridge.GetComponent<Rigidbody>().isKinematic;
        
        // Subscribe to events
        GameEvents.current.onTestSubjectRespawn += ClearTools;
        GameEvents.current.onTutorialStart += ClearTools;
        GameEvents.current.onTutorialReset += ClearTools;
        GameEvents.current.onGamePause += PauseFlag;
        GameEvents.current.onGameResume += ResumeFlag;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if tools exist in scene
        Invoke("CheckToolExistence", 1);
    }

    void CheckToolExistence()
    {        
        // Generate random number for block list
        randomNumber = Random.Range(0, 3);
        
        // Find tool object with 'Bridge' tag, if object not found, then create new tool object.
        if (GameObject.FindGameObjectWithTag("Bridge") == null)
        {
            Instantiate(bridge, toolPositions[0], bridge.transform.rotation);
            if(gamePaused)
            {
                StartCoroutine(InitializeGrabStatus("Bridge"));
            }
        }
        // Find tool object with 'WireCutter' tag, if object not found, then create new tool object.
        if (GameObject.FindGameObjectWithTag("WireCutter") == null)
        {
            Instantiate(wireCutter, toolPositions[1], wireCutter.transform.rotation);
            if(gamePaused)
            {
                StartCoroutine(InitializeGrabStatus("WireCutter"));
            }
        }
        // Find tool object with 'Block' tag, if object not found, then create new tool object.
        if (GameObject.FindGameObjectWithTag("Block") == null)
        {
            Instantiate(blocks[randomNumber], toolPositions[2], blocks[randomNumber].transform.rotation);
            if(gamePaused)
            {
                StartCoroutine(InitializeGrabStatus("Block"));
            }
        }
    }

    // Coroutine that updates grab status of newly created tools
    private IEnumerator InitializeGrabStatus(string toolTag)
    {
        // Give time for tool to respawn on table in correct location (won't get stuck in table on load)
        yield return new WaitForSeconds(0.1f);
        var tempTool = GameObject.FindGameObjectWithTag(toolTag);
        if(tempTool != null)
        {
            tempTool.GetComponent<ResetToolLocation>().DisableToolGrabbable();
        }        
    }

    // When test subject is respawned, clear all tools on conveyor
    void ClearTools()
    {
        GameObject[] conveyorBelt; 
        conveyorBelt = GameObject.FindGameObjectsWithTag("conveyor");
        foreach (GameObject onBeltObject in conveyorBelt)
        {
            if (onBeltObject.name.Contains("Bridge") || onBeltObject.name.Contains("Crate"))
            {
                Destroy(onBeltObject);
            }
        }

        // Find tool table objects
        GameObject[] tableBridges; 
        tableBridges = GameObject.FindGameObjectsWithTag("Bridge");
        GameObject[] tableWireCutters; 
        tableWireCutters = GameObject.FindGameObjectsWithTag("WireCutter");
        GameObject[] tableBlocks; 
        tableBlocks = GameObject.FindGameObjectsWithTag("Block");

        // Upon reload, reinstantiate tool table objects where needed
        foreach (GameObject tableBridge in tableBridges)
        {
            if (tableBridge.GetComponent<ResetToolLocation>().onToolTable == false)
            {
                Destroy(tableBridge);
            }
        }
        foreach (GameObject tableWireCutter in tableWireCutters)
        {
            if (tableWireCutter.GetComponent<ResetToolLocation>().onToolTable == false)
            {
                Destroy(tableWireCutter);
            }
        }
        foreach (GameObject tableBlock in tableBlocks)
        {
            if (tableBlock.GetComponent<ResetToolLocation>().onToolTable == false) 
            {
                Destroy(tableBlock);
            }
        }        
    }

    // Updates flag to determine if update to tool's grab state is needed on Game Pause
    private void PauseFlag()
    {
        gamePaused = true;
    }

    // Updates flag to determine if update to tool's grab state is needed on Game Resume
    private void ResumeFlag()
    {
        gamePaused = false;
    }

    // If destroyed, unsubscribe from events
    void OnDestroy()
    {
        GameEvents.current.onTestSubjectRespawn -= ClearTools;
        GameEvents.current.onTutorialStart -= ClearTools;
        GameEvents.current.onTutorialReset -= ClearTools;
        GameEvents.current.onGamePause -= PauseFlag;
        GameEvents.current.onGameResume -= ResumeFlag;
    }
}
