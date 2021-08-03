using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetObstacle : MonoBehaviour
{
    private ObstacleManager OM;
    [SerializeField]
    private bool isOnConveyor;

    public Vector3 baseRot;

    // Awake is called immediately after instatiation
    private void Awake()
    {
        //Debug.Log("$ ResetObstacle.Awake() - setting isOnConveyor = false");
        isOnConveyor = false;

    }

    // Start is called before the first frame update
    void Start()
    {
        // Get a handle to the Obstacle Manager
        OM = GameObject.Find("ObstacleStaging").GetComponent<ObstacleManager>();

        // Get the Base Rotation
        baseRot = new Vector3(0.0f, 0.0f, 0.0f);

        // Subscribe to the onPlaceObstacle Event
        GameEvents.current.onPlaceObstacle += OnPlaceObstacle;
        // Subscribe to the onFurnaceZoneExit Event
        GameEvents.current.onFurnaceZoneExit += OnResetObstacle;
        GameEvents.current.onResetObstacles += OnRestartLevel;
        //GameEvents.current.onTestSubjectRespawn += OnRestartLevel;

    }

    // OnRestartLevel handles onTestSubjectRespawn Events
    // The method calls OnResetObstacle on all Obstacles subscribed
    public void OnRestartLevel()
    {
        //Debug.Log("$ ResetObstacle().OnRestartLevel() - Started");
        if (isOnConveyor)
        {
            //Debug.Log("$ ResetObstacle().OnRestartLevel() - Resetting");
            OnResetObstacle(gameObject);
        }
    }


    // OnPlaceObstacle handles onPlaceObstacle Events
    // The method checks if we are the Obstacle that is being placed, and if yes, we enable the collider
    public void OnPlaceObstacle(GameObject someObstacle)
    {
        // Check if we are the object that is being placed
        if (someObstacle == gameObject)
        {
            //Debug.Log("Obstacled being Placed");
            isOnConveyor = true;
            gameObject.GetComponent<Collider>().enabled = true;
        }
    }

    public void PlaceObstacle()
    {
        //Debug.Log("Placing Obstacle");
        //Debug.Log("$ ResetObstacle.PlaceObstacle() - setting isOnConveyor = true");
        isOnConveyor = true;
        gameObject.GetComponent<Collider>().enabled = true;
    }

    // OnResetObstacle will reset the Objects Transform, Parent and any states in the Object and it's children
    public void OnResetObstacle(GameObject someObstacle)
    {
        // Check if we are the object that needs to be reset
        if (someObstacle == gameObject)
        {
            //Debug.Log("$ ResetObstacle.onResetObstacle - setting isOnConveyor = false");
            // Flag as removed from the Conveyor
            isOnConveyor = false;
            // Disable the Collider
            gameObject.GetComponent<Collider>().enabled = false;
            // Clear Parent
            gameObject.transform.parent = null;
            // Reset Active
            gameObject.SetActive(true);
            // If Forcefield, reset Wire, reset Forcefield
            if (gameObject.name == "Forcefield Wall Full(Clone)")
            {
                ResetForcefield();
                // Reset Transform
                transform.position = OM.ForceFieldStagePosition;
                OM.ForceFieldStagePosition.x -= 0.18f;
                return;
            }
            // If Block, add to Add back to Queue of Available Blocks
            if (gameObject.name == "Blocker(Clone)")
            {
                OM.availableBlocks.Enqueue(gameObject);
                // Reset Transform
                transform.position = OM.BlockStagePosition;
                OM.BlockStagePosition.x += 0.18f;
                return;
            }
            // Add the Pit of Death back to the Available Pits Of Death
            if (gameObject.name == "AcidPool_Full(Clone)")
            {
                OM.ReplacePit(0);
            }
            else if (gameObject.name == "AcidPool_Bottom(Clone)")
            {
                OM.ReplacePit(1);
            }
            else if (gameObject.name == "AcidPool_L_Closed(Clone)")
            {
                OM.ReplacePit(2);
            }
            else if (gameObject.name == "AcidPool_L_Open(Clone)")
            {
                OM.ReplacePit(3);
            }
            else if (gameObject.name == "AcidPool_None(Clone)")
            {
                OM.ReplacePit(4);
            }
            else if (gameObject.name == "AcidPool_TopBottom(Clone)")
            {
                OM.ReplacePit(5);
            }
            else if (gameObject.name == "AcidPool_U(Clone)")
            {
                OM.ReplacePit(6);
            }
            // Destroy the current Pit
            Destroy(gameObject);
        }
    }

    // ResetForcefield() resets all the children in the Forcefield that is being returned to the available objects
    public void ResetForcefield()
    {
        // Set all Children to Active
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
            //transform.GetChild(i).gameObject.GetComponent<Collider>().enabled = true;
        }

        // Enable the Audio Source
        AudioSource someSource = gameObject.GetComponentInChildren<AudioSource>();
        someSource.enabled = true;
        // Stop all Audio
        someSource.Stop();

        // Reset the Forcefield Material
        GameEvents.current.TestSubjectExit(gameObject.GetComponentInChildren<TestSubjectEnter>().GetInstanceID());

        // Reset the Forcefield Wire Material        
        ToolEnter te = gameObject.GetComponentInChildren<ToolEnter>();
        GameEvents.current.ToolExit(te.GetInstanceID(), null, te.gameObject.GetComponent<Collider>());

        

        OM.availableForcefields.Enqueue(gameObject);
    }

    private void OnDestroy()
    {
        // Unsubscribe to the onFurnaceZoneExit Event
        GameEvents.current.onFurnaceZoneExit -= OnResetObstacle;
        GameEvents.current.onPlaceObstacle -= OnPlaceObstacle;
        GameEvents.current.onTestSubjectRespawn -= OnRestartLevel;
    }
}
