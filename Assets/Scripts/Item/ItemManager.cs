using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    private static ItemManager _instance;
    public static ItemManager instance
    {
        get
        {

            if(_instance == null)
            {
                _instance = FindObjectOfType<ItemManager>();
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }

    }

    public ItemData itemData = new ItemData();

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        GetItem();
    }


    private void HandleItemData(string jsonData)
    {
        ItemData _itemData = JsonUtility.FromJson<ItemData>(jsonData);
        itemData.player_id = _itemData.player_id;
        itemData.player_gold = _itemData.player_gold;
        itemData.player_potion = _itemData.player_potion;
        
    }

    public void GetItem()
    {
        HTTPClient.instance.GET("https://breadmore.azurewebsites.net/api/Player_Data/" + LoginManager.instance.PlayerID, delegate (string www)
        {
            HandleItemData(www);
        });
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
