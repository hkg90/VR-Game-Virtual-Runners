using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMap : MonoBehaviour
{
    public int Difficulty;
    public ObstacleManager OM;
    public bool IsActiveOnStart;
    public Vector3 MapWorldSize;
    

    OM_Node[,,] Map;
    [SerializeField]
    private int ConveyorIndex;
    [SerializeField]
    private int RespawnCount;

    float nodeDiameter;
    float StartY;
    const int MAP_SIZE_X = 11;
    const int MAP_SIZE_Y = 3;
    const int MAP_SIZE_Z = 5;
    [SerializeField]
    const float NODE_RADIUS = 0.045f;
    const string PIT_OF_DEATH = "Pit";
    const string BLOCK = "Block";
    const string FORCEFIELD = "Forcefield";

    public enum LastMove { 
        NONE = -1,
        FORWARD = 0,
        LEFT = 1,
        RIGHT = 2
    };

    private void Awake()
    {
        RespawnCount = 0;
    }

    private void Start()
    {
        OM = GameObject.Find("ObstacleStaging").GetComponent<ObstacleManager>();
        Difficulty = 0;
        nodeDiameter = NODE_RADIUS * 2.0f;     
        MapWorldSize = new Vector3(MAP_SIZE_X * nodeDiameter, MAP_SIZE_Y * nodeDiameter, MAP_SIZE_Z * nodeDiameter);

        // Subscribe to the Conveyor Loop Event
        GameEvents.current.onLoopConveyorSection += onResetMapSection;
        GameEvents.current.onTestSubjectRespawn += onRestartGame;

        // Populate the Map Section with Obstacles if the section is Active On Start
        if (IsActiveOnStart)
        {
            CreateMap();
        }
    }

    // onHandleTestSubjectDeath handles onTestSubjectDeath Events
    // When called, this method will reset the map section to the game start state
    public void onRestartGame()
    {
        // Reset the Difficulty
        Difficulty = 0;

        // Check if this is the game start
        if (RespawnCount > 0)
        {
            // Reset the Map
            Map = null;
            // Populate the Map Section with Obstacles if the section is Active On Start
            if (IsActiveOnStart)
            {
                //Debug.Log("$ ObstacleMap.onRestartGame() - CreateMap()");
                CreateMap();
            }
        }

        // Increment Respawn Count
        RespawnCount++;
    }

    // onResetMapSection handles onLoopConveyorSection Events
    // When called, this method will check if it's the section that needs to loop, and if so call CreateMap()
    public void onResetMapSection(GameObject section)
    {
        if (section == gameObject.transform.parent.gameObject)
        {
            Difficulty++;
            CreateMap();
        }
    }

    void CreateMap() 
    {
        Map = null;
        Map = new OM_Node[MAP_SIZE_X, MAP_SIZE_Y, MAP_SIZE_Z];
        Vector3 mapBottomLeft = transform.position - Vector3.right * MapWorldSize.x / 2.0f - Vector3.forward * MapWorldSize.z / 2.0f;
        // Set the positions of each Node
        for (int x = 0; x < MAP_SIZE_X; x++)
        {
            for (int y = 0; y < MAP_SIZE_Y; y++)
            {
                for (int z = 0; z < MAP_SIZE_Z; z++)
                {
                    float xPos = mapBottomLeft.x + x * nodeDiameter + NODE_RADIUS;
                    float yPos = mapBottomLeft.y + y * nodeDiameter;
                    StartY = yPos;
                    float zPos = mapBottomLeft.z + z * nodeDiameter + NODE_RADIUS;
                    Vector3 pos = new Vector3(xPos, yPos, zPos);
                    Map[x, y, z] = new OM_Node(pos);
                    Map[x, y, z].xIndex = x;
                    Map[x, y, z].yIndex = y;
                    Map[x, y, z].zIndex = z;
                }
            }
        }

        int minDifficulty = Mathf.Min(Difficulty / 4, 3);

        // Pick Layout Type
        int layoutType = Random.Range(minDifficulty, 5);

        //layoutType = 3;

        switch (layoutType)
        {
            case 0:
                DoBlockSection();
                break;
            case 1:
                DoForcefieldSection();
                break;
            case 2:
                DoPitOfDeathSection(); 
                break;
            case 3:
                DoBlocksPlusForcefieldSection();
                break;
            case 4:
                DoWallSection(); 
                break;
                /*
            case 5:
                DoForcefieldPlusWallSection();
                break;
            case 6:
                DoBlocksPlusPitSection();
                break;
            case 7:
                DoForcefieldPlusPitSection();
                break;
            case 8:
                */
            default:
                //DoEverythingSection();
                break;
                
        }

        PlaceObstacles();
    }

    private void DoBlockSection()
    {
        // Do 0, 1, or 2 steps
        int stepCount = Random.Range(0, 4);         // 0 = no steps || 1 or 2 == 1 step || 3 = 2 steps
        if (stepCount > 1)
        {
            stepCount--;          // Normalize the random values > 1
        }

        // Place the steps
        // Create a list to hold the indices of each step row
        List<int> stepRows = new List<int>();
        for (int i = 0; i < stepCount; i++)
        {
            // Get a random Row
            // Build a list of available row indices
            List<int> openRows = new List<int>();
            for (int x = 0; x < MAP_SIZE_X; x++)
            {
                if (Map[x, 0, 0].occupant == null && Map[x, 0, 0].isProtected == false)
                {
                    openRows.Add(x);
                }
            }

            // Check if any rows are avialable, if not, exit without creating steps
            if (openRows.Count < 1)         // Need at least 2 open rows
            {
                break;
            }
            // Get a random row from open rows
            int randRow = Random.Range(0, openRows.Count);
            int rowIndex = openRows[randRow];
            // Check if the previous cell is occupied or protected
            if (rowIndex < 1 || Map[rowIndex - 1, 0, 0].occupant != null || Map[rowIndex - 1, 0, 0].isProtected == true)
            {
                // Move to next row
                rowIndex++;
            }
            // Remove the rowIndex and the previous row from openRows
            openRows.Remove(rowIndex - 1);
            openRows.Remove(rowIndex);

            // Create the steps
            // Do first row
            for (int z = 0; z < MAP_SIZE_Z; z++)
            {
                // Leave 1 in 3 steps empty
                if (Random.Range(0, 3) < 1)
                {
                    // Protect the cell
                    Map[rowIndex - 1, 0, z].isProtected = true;
                }
                else 
                { 
                    //OM_Node someNode = Map[rowIndex - 1, 0, z];
                    Map[rowIndex - 1, 0, z].occupant = BLOCK;
                    // Get the next Block from staging
                    GameObject someBlock = OM.SpawnBlock();
                    // Call the Place Obstacle Event
                    //GameEvents.current.PlaceObstacle(someBlock.GetInstanceID());
                    someBlock.GetComponent<ResetObstacle>().PlaceObstacle();
                    // Offset Block Y-position
                    Map[rowIndex - 1, 0, z].mapPosition.y += NODE_RADIUS;
                    someBlock.transform.position = Map[rowIndex - 1, 0, z].mapPosition;
                    someBlock.transform.parent = gameObject.transform;
                }
                
            }
            // Do second row
            for (int y = 0; y < 2; y++)
            {
                // Do the each layer
                for (int z = 0; z < MAP_SIZE_Z; z++)
                {
                    //OM_Node someNode = Map[rowIndex, y, z];
                    Map[rowIndex, y, z].occupant = BLOCK;
                    // Get the next Block from staging
                    GameObject someBlock = OM.SpawnBlock();
                    // Call the Place Obstacle Event
                    //GameEvents.current.PlaceObstacle(someBlock.GetInstanceID());
                    someBlock.GetComponent<ResetObstacle>().PlaceObstacle();
                    // Offset Block Y-position
                    Map[rowIndex, y, z].mapPosition.y += NODE_RADIUS;
                    someBlock.transform.position = Map[rowIndex, y, z].mapPosition;
                    someBlock.transform.parent = gameObject.transform;
                }
            }
        }


        // Build a List of OM_Nodes
        List<OM_Node> availableNodes = new List<OM_Node>();
        // Add all the Nodes from OM_Map to availableNodes
        for (int x = 0; x < MAP_SIZE_X; x++)
        {
            // Loop through all the Nodes in each Row
            for (int z = 0; z < MAP_SIZE_Z; z++)
            {
                // Only add the Node if it is available
                OM_Node someNode = Map[x, 0, z];
                if (someNode.occupant == null && someNode.isProtected == false)
                {
                    availableNodes.Add(Map[x, 0, z]);
                }
            }
        }

      


        // Get random Section Density
        int density = Difficulty + Mathf.FloorToInt(Random.Range(1.0f, 2.99f));
        // Get Block Count for Density
        int blockCount = density + Mathf.FloorToInt(MAP_SIZE_Z * MAP_SIZE_X / 10.0f);

        // Build a Queue of Random Nodes
        Queue<OM_Node> randomNodes = new Queue<OM_Node>();

        // Pick Random Nodes to place Blocks in
        for (int i = 0; i < blockCount; i++)
        {
            // Get a random index
            int nodeIndex = Mathf.FloorToInt(Random.Range(0.0f, (float)availableNodes.Count - 0.01f));
            // Add the node at that instance to the randomNodes queue
            randomNodes.Enqueue(availableNodes[nodeIndex]);
            // Remove the node at that instance from the list
            availableNodes.RemoveAt(nodeIndex);
        }

        // We now have a queue of all the nodes we want to place

        // Place a Block in each Node in the queue
        while (randomNodes.Count > 0)
        {
            // Remove the node from randomNodes
            OM_Node someNode = randomNodes.Dequeue();
            // Set the Node as Occupied
            Map[someNode.xIndex, someNode.yIndex, someNode.zIndex].occupant = BLOCK;
            Map[someNode.xIndex, someNode.yIndex, someNode.zIndex] = someNode;

            // TODO - Move everythign below here to PlaceObstacles()

            // Get the next Block from staging
            GameObject someBlock = OM.SpawnBlock();
            // Call the Place Obstacle Event
            //GameEvents.current.PlaceObstacle(someBlock.GetInstanceID());
            someBlock.GetComponent<ResetObstacle>().PlaceObstacle();
            // Offset Block Y-position
            Map[someNode.xIndex, someNode.yIndex, someNode.zIndex].mapPosition.y += NODE_RADIUS;
            someBlock.transform.position = Map[someNode.xIndex, someNode.yIndex, someNode.zIndex].mapPosition;
            someBlock.transform.parent = gameObject.transform;
        }
    }

    private void DoForcefieldSection()
    {
        // Get random number of Forcefields
        int forcefieldCount = Mathf.FloorToInt(Random.Range(1.0f, 3.99f));

        // Build List of available rows
        List<int> availableRows = new List<int>();
        for (int i = 0; i < MAP_SIZE_X; i++)
        {
            availableRows.Add(i);
        }

        // Pick random rows for forcefields
        List<int> forcefieldRows = new List<int>();
        for (int i = 0; i < forcefieldCount; i++)
        {
            // Get a random index within availableRows
            int randIndex = Mathf.FloorToInt(Random.Range(0.0f, (float)availableRows.Count - 0.01f));
            forcefieldRows.Add(availableRows[randIndex]);
            availableRows.RemoveAt(randIndex);
        }

        // Place the Forcefields
        foreach (int i in forcefieldRows)
        {
            // Set all the Nodes to occupied
            for (int j = 0; j < MAP_SIZE_Z; j++)
            {
                Map[i, 0, j].occupant = FORCEFIELD;
            }

            // Get the next Forcefield from staging
            GameObject someForcefield = OM.SpawnForceField();
            // Call the Place Obstacle Event
            //GameEvents.current.PlaceObstacle(someForcefield.GetInstanceID());
            someForcefield.GetComponent<ResetObstacle>().PlaceObstacle();
            // Position the Forcefield on the Conveyor
            someForcefield.transform.position = Map[i, 0, 2].mapPosition;
            someForcefield.transform.parent = gameObject.transform;
        }
    }

    private void DoWallSection()
    {
        // Get random number of Walls
        int wallCount = Random.Range(1, Mathf.Min(3, Mathf.Max(1, Difficulty)));

        // Build List of available rows
        List<int> availableRows = new List<int>();
        for (int i = 1; i < MAP_SIZE_X - 1; i++)
        {
            availableRows.Add(i);
        }

        // Pick random rows for Walls
        List<int> wallRows = new List<int>();
        for (int i = 0; i < wallCount; i++)
        {
            // Get a random row index
            int randIndex = Mathf.FloorToInt(Random.Range(0.0f, (float)availableRows.Count - 0.01f));
            int randRow = availableRows[randIndex];
            // Check if the preceding rows are not available
            if (!availableRows.Contains(randRow - 2))
            {
                // Check if the precedeing row is not available
                if (!availableRows.Contains(randRow - 1))
                {
                    // This means both rows in front of the selected row are protected/occupied
                    // Check if next row is available
                    if (availableRows.Contains(randRow + 1))
                    {
                        // Check if the next row is also open
                        if (availableRows.Contains(randRow + 2))
                        {
                            // Place the wall in this row
                            randRow += 2;
                        }
                        else
                        {
                            // At this point, give up and throw an error
                            Debug.LogError("Unable to find available row for Wall");
                        }
                    }
                    else
                    {
                        // At this point, give up and throw an error
                        Debug.LogError("Unable to find available row for Wall");
                    }
                }
                else
                {
                    // This means 2 rows in front is occupied, but preceding row is available
                    // Check if the next row is available
                    if (availableRows.Contains(randRow + 1))
                    {
                        // Place the wall in this row
                        randRow++;
                    }
                    else
                    {
                        // At this piont, give up and throw an error
                        Debug.LogError("Unable to find available row for Wall");
                    }
                }
            }
            else 
            {
                // This means 2 rows in front is open
                // Check if the preceding row is not available
                if (!availableRows.Contains(randRow - 1))
                {
                    // Check if the next row is available
                    if (availableRows.Contains(randRow + 1))
                    {
                        // This means we've found two open rows
                        // Check if third is open
                        if (availableRows.Contains(randRow + 2))
                        {
                            // Place the wall in this row
                            randRow += 2;
                        }
                        else
                        {
                            // At this point, give up and throw an error
                            Debug.LogError("Unable to find available row for Wall");
                        }
                    }
                    else 
                    {
                        // At this point, give up and throw an error
                        Debug.LogError("Unable to find available row for Wall");
                    }
                }
            }
            // Add row to forcefieldRows
            wallRows.Add(availableRows[randRow]);
            // Remove randRow and the 2 preceding rows from availableRows
            availableRows.RemoveAt(randRow);
            availableRows.RemoveAt(randRow - 1);
            availableRows.RemoveAt(randRow - 2);
            
        }

        foreach (int i in wallRows)
        {
            // Place walls in each row
            for (int z = 0; z < MAP_SIZE_Z; z++)
            {
                for (int y = 0; y < 2; y++)     // Build Wall 2 Cells tall, TODO - 3+ blocks tall with difficulty
                {
                    Map[i, y, z].occupant = BLOCK;
                    if (i - 1 >= 0)
                    {
                        Map[i - 1, 0, z].isProtected = true;
                    }
                    if (i - 2 >= 0)
                    {
                        Map[i - 2, 0, z].isProtected = true;
                    }

                    // Get the next Forcefield from staging
                    GameObject someBlock = OM.SpawnBlock();
                    // Call the Place Obstacle Event to activate all Colliders
                    //GameEvents.current.PlaceObstacle(someBlock.GetInstanceID());
                    someBlock.GetComponent<ResetObstacle>().PlaceObstacle();
                    // Offset Block Y-position
                    Map[i, y, z].mapPosition.y += NODE_RADIUS;
                    Vector3 blockPosition = Map[i, y, z].mapPosition;
                    someBlock.transform.position = blockPosition;
                    someBlock.transform.parent = gameObject.transform;
                }
            }
        }
    }

    private void DoPitOfDeathSection()
    {
        // Get Pit of Death count
        int pitCount = Mathf.FloorToInt(Random.Range(1.0f, 2.99f));
        //pitCount = 1;       // for testing

        // Build list of available rows
        List<int> availableRows = new List<int>();
        for (int i = 0; i < MAP_SIZE_X; i++)
        {
            availableRows.Add(i);
        }

        // Place Pits of Death in Random Rows
        Queue<OM_Node> deathNodes = new Queue<OM_Node>();
        for (int i = 0; i < pitCount; i++)
        {
            // Get a random row to start Pit of Death
            int startRow = Mathf.FloorToInt(Random.Range(0.0f, (float)availableRows.Count - 0.01f));
            // Remove this row from availableRows
            availableRows.Remove(startRow);
            int curColumn = 0;
            int curRow = startRow;
            LastMove lastMove = LastMove.NONE;
            do {
                // Set the Map Node Occupant
                OM_Node someNode = Map[curRow, 0, curColumn];
                if (someNode.occupant == null)
                {
                    someNode.occupant = PIT_OF_DEATH;
                    deathNodes.Enqueue(someNode);
                }

                // Move to the next Node

                // Get a random value for the movement direction
                float direction = Random.Range(0.0f, 0.99f);

                // Was last move a forward move?
                if ((int)lastMove < 1)
                {
                    // Then move either Forward, Left or Right
                    if (direction < 0.50f)
                    {
                        // Move Forward
                        lastMove = LastMove.FORWARD;
                        curColumn++;
                    }
                    else if (direction < 0.75f)
                    {
                        // Check if we can move Left
                        if (curRow > 0)
                        {
                            // If yes, move Left
                            lastMove = LastMove.LEFT;
                            curRow--;
                        }
                    }
                    else 
                    {
                        // Check if we can move RIght
                        if (curRow + 1 < MAP_SIZE_X)
                        {
                            // If yes, move Right
                            lastMove = LastMove.RIGHT;
                            curRow++;
                        }
                    }
                }
                else {
                    // Then either move Forward or in the same direction
                    if (direction < 0.75f)
                    {
                        // Move Forward
                        lastMove = LastMove.FORWARD;
                        curColumn++;
                    }
                    else 
                    {
                        // Move in same direction as last move
                        if (lastMove == LastMove.LEFT)
                        {
                            // Check if we can move Left
                            if (curRow > 0)
                            {
                                // If yes, move Left
                                lastMove = LastMove.LEFT;
                                curRow--;
                            }
                        }
                        else {                         
                            // Check if we can move RIght
                            if (curRow + 1 < MAP_SIZE_X)
                            {
                                // If yes, move Right
                                lastMove = LastMove.RIGHT;
                                curRow++;
                            }
                        }
                    }
                }
            } while (curColumn < MAP_SIZE_Z);
        }

        // Place Pits of Death
        foreach (OM_Node node in deathNodes)
        {
            // Determine which Pit Of Death to use
            bool hasTop = false;
            bool hasTopRight = false;
            bool hasRight = false;
            bool hasBottomRight = false;
            bool hasBottom = false;
            bool hasBottomLeft = false;
            bool hasLeft = false;
            bool hasTopLeft = false;

            // Check if there is a pit above
            if (node.zIndex + 1 < MAP_SIZE_Z)
            {
                if (Map[node.xIndex, node.yIndex, node.zIndex + 1].occupant == PIT_OF_DEATH)
                {
                    hasTop = true;
                }
            }
            // Check if there is a pit in Top Right
            if (node.xIndex + 1 < MAP_SIZE_X && node.zIndex + 1 < MAP_SIZE_Z) 
            {
                if (Map[node.xIndex + 1, node.yIndex, node.zIndex + 1].occupant == PIT_OF_DEATH)
                {
                    hasTopRight = true;
                }
            }
            // Check if there is a pit to the right
            if (node.xIndex + 1 < MAP_SIZE_X)
            {
                if (Map[node.xIndex + 1, node.yIndex, node.zIndex].occupant == PIT_OF_DEATH)
                {
                    hasRight = true;
                }
            }
            // Check if there is a pit to the bottom right
            if (node.xIndex + 1 < MAP_SIZE_X && node.zIndex - 1 >= 0)
            {
                if (Map[node.xIndex + 1, node.yIndex, node.zIndex - 1].occupant == PIT_OF_DEATH)
                {
                    hasBottomRight = true;
                }
            }
            // Check if there is a pit below
            if (node.zIndex - 1 >= 0)
            {
                if (Map[node.xIndex, node.yIndex, node.zIndex - 1].occupant == PIT_OF_DEATH)
                {
                    hasBottom = true;
                }
            }
            // Check if there is a pit in bottom left
            if (node.xIndex - 1 >= 0 && node.zIndex - 1 >= 0)
            {
                if (Map[node.xIndex - 1, node.yIndex, node.zIndex - 1].occupant == PIT_OF_DEATH)
                {
                    hasBottomLeft = true;
                }
            }
            // Check if there is a pit to the left
            if (node.xIndex - 1 >= 0)
            {
                if (Map[node.xIndex - 1, node.yIndex, node.zIndex].occupant == PIT_OF_DEATH)
                {
                    hasLeft = true;
                }
            }
            // Check if there is a pit in top left
            if (node.xIndex - 1 >= 0 && node.zIndex + 1 < MAP_SIZE_Z)
            {
                if (Map[node.xIndex - 1, node.yIndex, node.zIndex + 1].occupant == PIT_OF_DEATH)
                {
                    hasTopLeft = true;
                }
            }

            // Assign the Pit Type value
            int pitType = 0;
            Vector3 rotation = new Vector3(0.0f, 0.0f, 0.0f);
            // Check if full
            if (!hasTop && !hasBottom && !hasLeft && !hasRight)
            {
                pitType = 0;
            }
            // Check if only Bottom
            else if (hasTop && !hasBottom && hasLeft && hasRight)
            {
                pitType = 1;
                rotation = new Vector3(0.0f, 0.0f, 180.0f);
            }
            // Check if only Top
            else if (!hasTop && hasBottom && hasLeft && hasRight)
            {
                pitType = 1;
            }
            // Check if only Left
            else if (hasTop && hasBottom && !hasLeft && hasRight)
            {
                pitType = 1;
                rotation = new Vector3(0.0f, 0.0f, -90.0f);
            }
            // Check if only Right
            else if (hasTop && hasBottom && hasLeft && !hasRight)
            {
                pitType = 1;
                rotation = new Vector3(0.0f, 0.0f, 90.0f);
            }
            // Check if Closed L with Top & Right Open
            else if (hasTop && !hasBottom && !hasLeft && hasRight && !hasTopRight)
            {
                pitType = 2;
                rotation = new Vector3(0.0f, 0.0f, -90.0f);
            }
            // Check if Closed L with Top & Left Open
            else if (hasTop && !hasBottom && hasLeft && !hasRight && !hasTopLeft)
            {
                pitType = 2;
                rotation = new Vector3(0.0f, 0.0f, 180.0f);
            }
            // Check if Close L with Bottom & Left Open
            else if (!hasTop && hasBottom && hasLeft && !hasRight && !hasBottomLeft)
            {
                pitType = 2;
                rotation = new Vector3(0.0f, 0.0f, 90.0f);
            }
            // Check if Close L with Bottom & Right Open
            else if (!hasTop && hasBottom && !hasLeft && hasRight && !hasBottomRight)
            {
                pitType = 2;
                //rotation = new Vector3(0.0f, 0.0f, 180.0f);
            }
            // Check if Open L with Top & Right Open
            else if (hasTop && !hasBottom && !hasLeft && hasRight && hasTopRight)
            {
                pitType = 3;
                rotation = new Vector3(0.0f, 0.0f, -90.0f);
            }
            // Check if Open L with Top & Left Open
            else if (hasTop && !hasBottom && hasLeft && !hasRight && hasTopLeft)
            {
                pitType = 3;
                rotation = new Vector3(0.0f, 0.0f, 180.0f);
            }
            // Check if Open L with Bottom & Left Open
            else if (!hasTop && hasBottom && hasLeft && !hasRight && hasBottomLeft)
            {
                pitType = 3;
                rotation = new Vector3(0.0f, 0.0f, 90.0f);
            }
            // Check if Open L with Bottom & Right Open
            else if (!hasTop && hasBottom && !hasLeft && hasRight && hasBottomRight)
            {
                pitType = 3;
                //rotation = new Vector3(0.0f, 0.0f, -90.0f);
            }
            // Check if Pit in all directions
            else if (hasTop && hasBottom && hasLeft && hasRight)
            {
                pitType = 4;
                // Move up along y-axis to avoid clipping
                node.mapPosition.y += 0.001f;
            }
            // Check if 2 opposite sides
            else if ((!hasTop && !hasBottom) && (hasLeft && hasRight))
            {
                pitType = 5;
                //rotation = new Vector3(0.0f, 0.0f, -90.0f);
            }
            else if ((hasTop && hasBottom) && (!hasLeft && !hasRight))
            {
                pitType = 5;
                rotation = new Vector3(0.0f, 0.0f, 90.0f);
            }
            // Check if U that opens to Top
            else if (hasTop && !hasBottom && !hasLeft && !hasRight)
            {
                pitType = 6;
                rotation = new Vector3(0.0f, 0.0f, 180.0f);
            }
            // Check if U that opens to Bottom
            else if (!hasTop && hasBottom && !hasLeft && !hasRight)
            {
                pitType = 6;
            }
            // Check if U that opens to Right
            else if (!hasTop && !hasBottom && !hasLeft && hasRight)
            {
                pitType = 6;
                rotation = new Vector3(0.0f, 0.0f, -90.0f);
            }
            // Check if U that opens to Left
            else if (!hasTop && !hasBottom && hasLeft && !hasRight)
            {
                pitType = 6;
                rotation = new Vector3(0.0f, 0.0f, 90.0f);
            }
            else
            {
                pitType = 0;
            }

            // Get the next Pit Of Death from staging
            GameObject somePitOfDeath = OM.SpawnPitOfDeath(pitType);
            // Call the Place Obstacle Event
            //GameEvents.current.PlaceObstacle(somePitOfDeath.GetInstanceID());
            somePitOfDeath.GetComponent<ResetObstacle>().PlaceObstacle();
            // Position the Pit of Death on the Conveyor
            somePitOfDeath.transform.position = node.mapPosition;
            somePitOfDeath.transform.Rotate(rotation);
            somePitOfDeath.GetComponent<ResetObstacle>().baseRot.z = -rotation.z;

            somePitOfDeath.transform.parent = gameObject.transform;
        }
    }

    private void DoBlocksPlusForcefieldSection()
    {
        // Solid row
        int solidCount = Random.Range(0, 2);
        // Get count of steps row
        int stepCount = Random.Range(0, 2);
        // Get count of Force Fields
        int forcefieldCount = Random.Range(1, 4);

        // Build list of available rows
        List<int> availableRows = new List<int>();
        for (int i = 0; i < MAP_SIZE_X; i++)
        {
            availableRows.Add(i);
        }

        // Build list of solidRows
        List<int> solidRows = new List<int>();
        for (int i = 0; i < solidCount; i++)
        {
            // Pick a random row
            int randIndex = Random.Range(0, availableRows.Count);

            // Add the row to solidRows
            solidRows.Add(availableRows[randIndex]);

            // Remove the row from availableRows
            availableRows.RemoveAt(randIndex);
        }
        // Populate Map Tiles with data
        for (int i = 0; i < solidRows.Count; i++)
        {
            // Add a Block to each cell in the Map row
            for (int z = 0; z < MAP_SIZE_Z; z++)
            {
                // Leave 1/4 empty
                if (Random.Range(0, 4) < 3)
                {
                    if (Map[solidRows[i], 0, z].occupant == null)
                    {
                        Map[solidRows[i], 0, z].occupant = BLOCK;
                        Map[solidRows[i], 0, z].mapPosition.y += NODE_RADIUS;
                    }
                }
                else 
                {
                    Map[solidRows[i], 0, z].isProtected = true;
                }
            }
        }

        // Build list of Steps
        List<int> stepRows = new List<int>();
        for (int i = 0; i < stepCount; i++)
        {
            // Pick a random row
            int randIndex = Random.Range(0, availableRows.Count);

            // Check if the previous row is already occupied
            if (!availableRows.Contains(randIndex - 1))
            {
                // Check if the next row is available
                if (availableRows.Contains(randIndex + 1))
                {
                    // If yes, then move to next row
                    randIndex++;
                }
                else 
                {
                    Debug.LogError("Unable to find available rows for steps");
                }
            }

            // Add the row to stepRows
            stepRows.Add(availableRows[randIndex]);

            // Remove the row and the previous row from available rows
            availableRows.RemoveAt(randIndex);
            availableRows.RemoveAt(randIndex - 1);
        }
        // Populate Tile Map with data
        for (int i = 0; i < stepCount; i++)
        {
            // Do the previous row
            for (int z = 0; z < MAP_SIZE_Z; z++)
            {
                // Leave 1/4 tiles empty
                if (Random.Range(0, 4) < 3)
                {
                    if (Map[stepRows[i] - 1, 0, z].occupant == null)
                    {
                        Map[stepRows[i] - 1, 0, z].occupant = BLOCK;
                        Map[stepRows[i] - 1, 0, z].mapPosition.y += NODE_RADIUS;
                    }
                }
                else
                {
                    Map[stepRows[i] - 1, 0, z].isProtected = true;
                }
            }
            // Do the current row
            for (int y = 0; y < 2; y++)
            {
                for (int z = 0; z < MAP_SIZE_Z; z++)
                {
                    bool doTile = true;
                    if (y > 0)
                    {
                        // Leave 1/4 tiles empty
                        if (Random.Range(0, 4) == 3)
                        {
                            doTile = false;
                        }
                        else 
                        {
                            Map[stepRows[i], y, z].isProtected = true;
                        }
                    }
                    if (doTile)
                    {
                        if (Map[stepRows[i], y, z].occupant == null)
                        {
                            Map[stepRows[i], y, z].occupant = BLOCK;
                            Map[stepRows[i], y, z].mapPosition.y += NODE_RADIUS;
                        }
                    }
                }
            }
        }

        // Build Force Fields
        List<int> forcefieldRows = new List<int>();
        for (int i = 0; i < forcefieldCount; i++)
        {
            // Pick a random row
            int randIndex = Random.Range(0, availableRows.Count);

            // Add the row to forcefieldRows
            forcefieldRows.Add(availableRows[randIndex]);

            // Remove the row from available rows
            availableRows.RemoveAt(randIndex);
        }
        // Set Tile Map data
        for (int i = 0; i < forcefieldCount; i++)
        {
            for (int z = 0; z < MAP_SIZE_Z; z++)
            {
                if (Map[forcefieldRows[i], 0, z].occupant == null)
                {
                    Map[forcefieldRows[i], 0, z].occupant = FORCEFIELD;
                }
            }
        }

        // Build list of remaining cells
        List<OM_Node> availableNodes = new List<OM_Node>();
        for (int x = 0; x < MAP_SIZE_X; x++)
        {
            for (int z = 0; z < MAP_SIZE_Z; z++)
            {
                if (Map[x, 0, z].occupant == null && Map[x, 0, z].isProtected == false)
                {
                    // Place a block in 1/4 available cells
                    if (Random.Range(0, 4) == 3)
                    {
                        Map[x, 0, z].occupant = BLOCK;
                        Map[x, 0, z].mapPosition.y += NODE_RADIUS;
                    }
                }
            }
        }

        // Place obstacles on the Conveyor
        for (int x = 0; x < MAP_SIZE_X; x++)
        {
            // Check if we're doing a Forcefield
            if (Map[x, 0, 0].occupant == FORCEFIELD)
            {
                // Do Force Field
                // Get the next Forcefield from staging
                GameObject someForcefield = OM.SpawnForceField();
                // Call the Place Obstacle Event
                //GameEvents.current.PlaceObstacle(someForcefield.GetInstanceID());
                someForcefield.GetComponent<ResetObstacle>().PlaceObstacle();
                // Position the Forcefield on the Conveyor
                someForcefield.transform.position = Map[x, 0, 2].mapPosition;
                someForcefield.transform.parent = gameObject.transform;
            }
            else
            {
                // Place blocks
                for (int y = 0; y < 2; y++)
                {
                    for (int z = 0; z < MAP_SIZE_Z; z++)
                    {
                        if (Map[x, y, z].occupant == BLOCK)
                        {
                            // Get the next Block from staging
                            GameObject someBlock = OM.SpawnBlock();
                            // Call the Place Obstacle
                            someBlock.GetComponent<ResetObstacle>().PlaceObstacle();
                            someBlock.transform.position = Map[x, y, z].mapPosition;
                            someBlock.transform.parent = gameObject.transform;
                        }
                    }
                }
            }
        }
        
    }

    public void DoForcefieldPlusWallSection()
    { 
    
    }

    private void DoBlocksPlusPitSection()
    { 
    
    }

    private void DoForcefieldPlusPitSection()
    { 
    
    }

    private void DoEverythingSection()
    { 
    
    }

    private void PlaceObstacles()
    {
        // Loop through each cell
        // Grab the Obstacle from OM
        // Place the Obstacle in it's location
        Vector3 mapBottomLeft = transform.position - Vector3.right * MapWorldSize.x / 2.0f - Vector3.forward * MapWorldSize.z / 2.0f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + Vector3.up * MapWorldSize.y / 2.0f, new Vector3(MapWorldSize.x, MapWorldSize.y, MapWorldSize.z));

        if (Map != null) {
            foreach (OM_Node n in Map) {
                if (n.occupant == "Empty")
                {
                    Gizmos.color = Color.white;
                }
                else if (n.occupant == FORCEFIELD)
                {
                    Gizmos.color = Color.blue;
                }
                else if (n.occupant == PIT_OF_DEATH)
                {
                    Gizmos.color = Color.green;
                }
                else if (n.occupant == BLOCK)
                {
                    Gizmos.color = Color.yellow;
                }
                else
                {
                    Gizmos.color = Color.white;
                }
                Gizmos.DrawCube(n.mapPosition, Vector3.one * (nodeDiameter - .0025f));
            }
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe to the Conveyor Loop Event
        GameEvents.current.onLoopConveyorSection -= onResetMapSection;
        GameEvents.current.onTestSubjectRespawn -= onRestartGame;
    }
}
