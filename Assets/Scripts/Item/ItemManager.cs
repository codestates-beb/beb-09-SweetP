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
    public int IncreseProb;
    public ItemData itemData = new ItemData();
    public ScrollData scrollData = new ScrollData();
    public int PPC =99999;
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
        itemData = _itemData;
        
    }

    private void HandleScrollData(string jsonData)
    {
        ScrollData _scrollData = JsonUtility.FromJson<ScrollData>(jsonData);
        scrollData = _scrollData;
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
        HTTPClient.instance.GET("https://breadmore.azurewebsites.net/api/Player_Scroll/" + LoginManager.instance.PlayerID, delegate (string www)
        {
            HandleScrollData(www);
        });
    }

    public void InitScroll()
    {
        string jsonData = JsonUtility.ToJson(scrollData);
        HTTPClient.instance.PUT("https://breadmore.azurewebsites.net/api/Player_Scroll/" + LoginManager.instance.PlayerID, jsonData, delegate (string www)
        {
           
        });
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

    public int RetrunScrollProb(ScrollType scrollType)
    {
        switch (scrollType)
        {
            case (ScrollType)0:
                IncreseProb = 0;
                break;
            case (ScrollType)1:
                IncreseProb = 10;
                break;
            case (ScrollType)2:
                IncreseProb = 20;
                break;
        }

        return IncreseProb;
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
