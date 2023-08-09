using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitPlayerPos : MonoBehaviour
{
    public GameObject player;
    public Transform playerTransform;
    public NPCBase[] NPCs;

    public void SetPlayerPosition(Vector3 position)
    {
        transform.position = position;
    }
    // Update is called once per frame
    void Start()
    {
        GameObject gameObject = Instantiate(player, transform);
    }

    // Update is called once per frame
    void Update()
    {
     
    }
}
