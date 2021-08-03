using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetEventCamera : MonoBehaviour
{
    public Camera chosenCamera;
    // Awake is called when the script instance is being loaded
    void Awake()
    {
        GetComponent<Canvas>().worldCamera = chosenCamera;
    }
}
