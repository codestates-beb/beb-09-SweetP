using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCLoad : MonoBehaviour
{
    private static NPCLoad _instance;
    public static NPCLoad instance
    {

        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                _instance = FindObjectOfType<NPCLoad>();
            }

            // 싱글톤 오브젝트를 반환
            return _instance;
        }
    }

    public GameObject player;
    private static bool hasInstance = false;
    private bool playerLoaded = false;
    // Start is called before the first frame update
    void Awake()
    {
        if (!hasInstance)
        {
            DontDestroyOnLoad(gameObject);
            hasInstance = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    
    // Update is called once per frame
    void Update()
    {
        if (!playerLoaded)
        {
            // Find the PlayerParent object and then find the Player object within it
            GameObject playerParent = GameObject.Find("PlayerParent(Clone)");
            if (playerParent != null)
            {
                Transform playerTransform = playerParent.transform.Find("Player");
                if (playerTransform != null)
                {
                    player = playerTransform.gameObject;
                    playerLoaded = true; // Set the flag to true to prevent further execution
                }
            }
        }

        if(player == null)
        {
            playerLoaded = false;
        }
    }
}
