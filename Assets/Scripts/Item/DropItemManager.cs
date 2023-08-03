using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemManager : MonoBehaviour
{
    private static DropItemManager _instance;
    public static DropItemManager instance
    {
        get
        {

            if (_instance == null)
            {
                _instance = FindObjectOfType<DropItemManager>();
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }

    }

    public List<Scroll> DropScrollList = new List<Scroll>();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }


    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}
