using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
public class MarketManager : MonoBehaviour
{

    private static MarketManager _instance;
    public static MarketManager instance
    {
        get
        {

            if (_instance == null)
            {
                _instance = FindObjectOfType<MarketManager>();
            }
            return _instance;
        }
        
    }


    public List<MarketData> marketDataList = new List<MarketData>();
    // Start is called before the first frame update

    private void HandleMarketData(string jsonData)
    {
        List<MarketData> marketDatas = JsonConvert.DeserializeObject<List<MarketData>>(jsonData);

        foreach (var marketData in marketDatas)
        {
            marketDataList.Add(marketData);
        }

    }

    public void GetMarket()
    {
        HTTPClient.instance.GET("https://breadmore.azurewebsites.net/api/Weapon_Market", delegate (string www)
        {
            HandleMarketData(www);
        });
    }

    void Start()
    {
        GetMarket();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
