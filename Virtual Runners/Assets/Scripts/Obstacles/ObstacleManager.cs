using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    public GameObject Forcefield;
    public GameObject PitOfDeathFull;
    public GameObject PitOfDeathBottom;
    public GameObject PitOfDeathLClosed;
    public GameObject PitOfDeathLOpen;
    public GameObject PitOfDeathNone;
    public GameObject PitOfDeathTopBottom;
    public GameObject PitOfDeathU;
    public GameObject Block;

    public Queue<GameObject> availableForcefields;
    public Queue<GameObject> availablePitsFull;
    public Queue<GameObject> availablePitsBottom;
    public Queue<GameObject> availablePitsLClosed;
    public Queue<GameObject> availablePitsLOpen;
    public Queue<GameObject> availablePitsNone;
    public Queue<GameObject> availablePitsTopBottom;
    public Queue<GameObject> availablePitsU;
    public Queue<GameObject> availableBlocks;

    public Vector3 ForceFieldStagePosition;
    public Vector3 PitOfDeathStagePosition;
    public Vector3 BlockStagePosition;

    public bool FirstLife;

    [SerializeField]
    private const int MIN_OBSTACLES = 2;

    // Start is called before the first frame update
    private void Awake()
    {
        FirstLife = true;

        availableForcefields = new Queue<GameObject>();
        availablePitsFull = new Queue<GameObject>();
        availablePitsBottom = new Queue<GameObject>();
        availablePitsLClosed = new Queue<GameObject>();
        availablePitsLOpen = new Queue<GameObject>();
        availablePitsNone = new Queue<GameObject>();
        availablePitsTopBottom = new Queue<GameObject>();
        availablePitsU = new Queue<GameObject>();
        availableBlocks = new Queue<GameObject>();

        ForceFieldStagePosition = new Vector3(-5.0f, 0.0f, 0.0f);
        PitOfDeathStagePosition = new Vector3(0.0f,  0.0f, 5.0f);
        BlockStagePosition      = new Vector3(5.0f,  0.0f, 0.0f);

        // Subscribe to Respawn Test Subject
        GameEvents.current.onTestSubjectRespawn += ResetMap;

        // Instantiate all the Objects
        InstantiateForcefields();
        InstantiatePits();
        InstantiateBlocks();
    }

    // Reset Map hands the onTestSubjectRespawn Event
    // When called this method will tell all the Obstacles in the map if they need to reset or not
    public void ResetMap()
    {
        if (!FirstLife)
        {
            // Tell all subscribed obstacles to reset
            GameEvents.current.ResetObstacles();
        }
        FirstLife = false;
    }

    private void InstantiateForcefields()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject someForcefield = Instantiate(Forcefield, gameObject.transform);
            someForcefield.transform.position = ForceFieldStagePosition;
            availableForcefields.Enqueue(someForcefield);
            ForceFieldStagePosition.x -= 0.18f;
            if (ForceFieldStagePosition.x < -20.0f)
            {
                ForceFieldStagePosition.x = -5.0f;
            }
        }
    }

    public GameObject SpawnForceField()
    {
        if (availableForcefields.Count < MIN_OBSTACLES)
        {
            InstantiateForcefields();
        }
        return availableForcefields.Dequeue();
    }

    private void InstantiatePits()
    {
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                GameObject pitType;
                GameObject somePit;
                switch (i)
                {
                    case 0:
                        pitType = PitOfDeathFull;
                        somePit = CreatePit(pitType);
                        availablePitsFull.Enqueue(somePit);
                        break;
                    case 1:
                        pitType = PitOfDeathBottom;
                        somePit = CreatePit(pitType);
                        availablePitsBottom.Enqueue(somePit);
                        break;
                    case 2:                        
                        pitType = PitOfDeathLClosed;
                        somePit = CreatePit(pitType);
                        availablePitsLClosed.Enqueue(somePit);
                        break;
                    case 3:
                        pitType = PitOfDeathLOpen;
                        somePit = CreatePit(pitType);
                        availablePitsLOpen.Enqueue(somePit);
                        break;
                    case 4:
                        pitType = PitOfDeathNone;
                        somePit = CreatePit(pitType);
                        availablePitsNone.Enqueue(somePit);
                        break;
                    case 5:
                        pitType = PitOfDeathTopBottom;
                        somePit = CreatePit(pitType);
                        availablePitsTopBottom.Enqueue(somePit);
                        break;
                    case 6:
                        pitType = PitOfDeathU;
                        somePit = CreatePit(pitType);
                        availablePitsU.Enqueue(somePit);
                        break;
                    default:
                        pitType = PitOfDeathFull;
                        somePit = CreatePit(pitType);
                        availablePitsFull.Enqueue(somePit);
                        break;
                };

                PitOfDeathStagePosition.x += 0.18f;
                if (PitOfDeathStagePosition.x > 50.0f)
                {
                    PitOfDeathStagePosition.x = 0.0f;
                }
            }
        }
    }

    public void ReplacePit(int pitType)
    {
        GameObject somePit = null;
        switch (pitType) 
        {
            case 0:
                somePit = CreatePit(PitOfDeathFull);
                availablePitsFull.Enqueue(somePit);
                break;
            case 1:
                somePit = CreatePit(PitOfDeathBottom);
                availablePitsBottom.Enqueue(somePit);
                break;
            case 2:
                somePit = CreatePit(PitOfDeathLClosed);
                availablePitsLClosed.Enqueue(somePit);
                break;
            case 3:
                somePit = CreatePit(PitOfDeathLOpen);
                availablePitsLOpen.Enqueue(somePit);
                break;
            case 4:
                somePit = CreatePit(PitOfDeathNone);
                availablePitsNone.Enqueue(somePit);
                break;
            case 5:
                somePit = CreatePit(PitOfDeathTopBottom);
                availablePitsTopBottom.Enqueue(somePit);
                break;
            case 6:
                somePit = CreatePit(PitOfDeathU);
                availablePitsU.Enqueue(somePit);
                break;
            default:
                somePit = CreatePit(PitOfDeathFull);
                availablePitsFull.Enqueue(somePit);
                break;
        };
        PitOfDeathStagePosition.x += 0.18f;
    }

    private GameObject CreatePit(GameObject pitType)
    {
        GameObject somePitOfDeath = Instantiate(pitType, gameObject.transform);
        somePitOfDeath.transform.position = PitOfDeathStagePosition;
        return somePitOfDeath;
    }

    public GameObject SpawnPitOfDeath(int type)
    {
        GameObject somePit = null;
        switch (type)
        {
            case 0:
                if (availablePitsFull.Count < MIN_OBSTACLES)
                {
                    InstantiatePits();
                }
                somePit = availablePitsFull.Dequeue();
                break;
            case 1:
                if (availablePitsBottom.Count < MIN_OBSTACLES)
                {
                    InstantiatePits();
                }
                somePit = availablePitsBottom.Dequeue();
                break;
            case 2:
                if (availablePitsLClosed.Count < MIN_OBSTACLES)
                {
                    InstantiatePits();
                }
                somePit = availablePitsLClosed.Dequeue();
                break;
            case 3:
                if (availablePitsLOpen.Count < MIN_OBSTACLES)
                {
                    InstantiatePits();
                }
                somePit = availablePitsLOpen.Dequeue();
                break;
            case 4:
                if (availablePitsNone.Count < MIN_OBSTACLES)
                {
                    InstantiatePits();
                }
                somePit = availablePitsNone.Dequeue();
                break;
            case 5:
                if (availablePitsTopBottom.Count < MIN_OBSTACLES)
                {
                    InstantiatePits();
                }
                somePit = availablePitsTopBottom.Dequeue();
                break;
            case 6:
                if (availablePitsU.Count < MIN_OBSTACLES)
                {
                    InstantiatePits();
                }
                somePit = availablePitsU.Dequeue();
                break;
            default:
                if (availablePitsFull.Count < MIN_OBSTACLES)
                {
                    InstantiatePits();
                }
                somePit = availablePitsFull.Dequeue();
                break;
        };
        return somePit;
    }

    private void InstantiateBlocks()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject someBlock = Instantiate(Block, gameObject.transform);
            someBlock.transform.position = BlockStagePosition;
            availableBlocks.Enqueue(someBlock);
            BlockStagePosition.x += 0.18f;
            if (BlockStagePosition.x > 25.0f)
            {
                BlockStagePosition.x = 5.0f;
            }
        }
    }

    public GameObject SpawnBlock()
    {
        if (availableBlocks.Count < MIN_OBSTACLES)
        {
            InstantiateBlocks();
        }
        GameObject someBlock = availableBlocks.Dequeue();
        someBlock.GetComponentInChildren<ParticleSystem>().Stop();
        return someBlock;
    }
}
