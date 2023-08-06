using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshFloor : MonoBehaviour
{
    private MeshFilter meshFilter;
    // Start is called before the first frame update
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
    }


    public Mesh GetFloorMesh()
    {
        return meshFilter.sharedMesh;
    }

}
