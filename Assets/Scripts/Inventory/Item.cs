using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Item : MonoBehaviour
{
    private static Item _instance;
    public static Item instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Item>();
            }
            return _instance;
        }
    }

    public Sprite Potion;
    public Sprite Jewel;
    public Sprite Gold;
    public Sprite Sword;
    public Sprite Bow;
    public Sprite Magic;
    public Sprite Scroll;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
