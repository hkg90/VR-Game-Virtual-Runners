using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OM_Node
{
    public Vector3 mapPosition;
    public int xIndex, yIndex, zIndex;
    public string occupant;
    public bool isProtected;

    public OM_Node(Vector3 _mapPosition)
    {
        mapPosition = _mapPosition;
        occupant = null;
        isProtected = false;
    }

}
