using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitPlayerPos : MonoBehaviour
{
    public GameObject player;
    private Transform playerParent;
    public void SetPlayerPosition(Vector3 position)
    {
        transform.position = position;
    }


    
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(player, transform);

        //Transform player = playerParent.Find("Player");

        ////pos = transform.position;
        //player.position = transform.position;
        //player.rotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        //SetPlayerPosition(pos);
    }
}
