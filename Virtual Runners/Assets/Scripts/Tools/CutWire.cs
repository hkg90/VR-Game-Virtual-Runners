using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutWire : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameEvents.current.onToolObstacleAction += OnCutWire;
    }

    // CutWire cuts the wire and deactivates the Forcefield
    private void OnCutWire(int id, GameObject obstacle)
    {
        if (gameObject.GetInstanceID() == obstacle.GetInstanceID() && gameObject.name == obstacle.name)
        {
            // Cut the Wire
            //Deactivate the Forcefield
            Transform parent = transform.parent;
            int childrenCount = parent.childCount;
            int currentChild = 0;
            bool found = false;
            Transform forcefield = null;
            while (!found && currentChild < childrenCount)
            {
                forcefield = parent.GetChild(currentChild);
                if (forcefield.name == "Forcefield")
                {
                    found = true;
                }
                currentChild++;
            }
            if (found)
            {
                // Stop the AudioSource from playing
                forcefield.GetComponent<AudioSource>().Stop();
                forcefield.GetComponent<AudioSource>().enabled = false;
                forcefield.gameObject.SetActive(false);
            }

            // Deactivate the wire            
            gameObject.SetActive(false);
        }
    }

    // If destroyed, unsubscribe from game events
    private void OnDestroy()
    {
        GameEvents.current.onToolObstacleAction -= OnCutWire;
    }
}
