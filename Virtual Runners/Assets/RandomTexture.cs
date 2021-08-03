using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTexture : MonoBehaviour
{
    public List<Material> Materials;

    // Start is called before the first frame update
    void Start()
    {
        // Get a random Material
        int materialCount = Materials.Count;
        int randMat = Random.Range(0, materialCount);

        Material myMaterial = Materials[randMat];

        gameObject.GetComponent<MeshRenderer>().material = myMaterial;
    }

}
