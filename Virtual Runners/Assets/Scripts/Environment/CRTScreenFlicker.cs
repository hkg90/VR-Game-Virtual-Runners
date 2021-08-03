// Script Written by Jay Pittenger


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/******************************************************************************************************
 * CRTScreenFlicker is a class that performs the flicker color change on the CRT monitor in the scene
 ******************************************************************************************************/


public class CRTScreenFlicker : MonoBehaviour
{
    System.Random rnd;
    private int randNum;
    private float randResult;
    Renderer Renderer; 

    void Start()
    {
        rnd = new System.Random();
        Renderer = GetComponent<Renderer>();
    }


    void FixedUpdate()
    {
        randNum = rnd.Next(10, 16);
        randResult = 0.01f * randNum;
            
        Color color = new Color(0, randResult, 0, 1);
        Renderer.material.SetColor("_EmissionColor", color);
    }
}
