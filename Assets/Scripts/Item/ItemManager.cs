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
    public int PPC =99999;
    public List<ScrollData> scrollDataList = new List<ScrollData>();
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
        GetScroll();
    }

    public void UsePPC(int amount)
    {
        PPC -= amount;
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

    public void GetScroll()
    {
        for(int i=0; i<scrollDataList.Count; i++)
        {
            scrollDataList[i].count = 5;
        }
    }

    public void ChangeGold(int gold)
    {
        itemData.player_gold += gold;
        RefreshPlayerData();
    }

    public void RefreshPlayerData() {
        string jsonData = JsonUtility.ToJson(itemData);
        string url = "https://breadmore.azurewebsites.net/api/Player_Data/" + LoginManager.instance.PlayerID;
        HTTPClient.instance.PUT(url, jsonData, delegate (string www) { });
    }

    public void PutItem()
    {
        
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
