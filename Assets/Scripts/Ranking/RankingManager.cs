using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class RankingManager : MonoBehaviour
{
    private static RankingManager _instance;
    public static RankingManager instance
    {
        get
        {

            if (_instance == null)
            {
                _instance = FindObjectOfType<RankingManager>();
            }
            return _instance;
        }

    }

    public List<PlayerRecord> playerRecords = new List<PlayerRecord>();
    private class PlayerRecordArray
    {
        public PlayerRecord[] records;
    }
    // Start is called before the first frame update
    void Start()
    {
        GetRanking();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void HandleRecordList(string jsonData)
    {
        PlayerRecordArray playerRecordArray = JsonUtility.FromJson<PlayerRecordArray>("{\"records\":" + jsonData + "}");
        if(playerRecordArray != null && playerRecordArray.records != null)
        {
            foreach(var recordArray in playerRecordArray.records)
            {
                PlayerRecord playerRecord = new PlayerRecord();
                playerRecord.player_id = recordArray.player_id;
                playerRecord.player_score = recordArray.player_score;
                if(playerRecord.player_score !=0)
                playerRecords.Add(playerRecord);
            }
        }

    }
    public void GetRanking()
    {
        HTTPClient.instance.GET("https://breadmore.azurewebsites.net/api/Player_Record",
            delegate (string www)
            {
                HandleRecordList(www);

                SortPlayerRecordsByScore();
            });
    }

    private void SortPlayerRecordsByScore()
    {
        playerRecords = playerRecords.OrderBy(record => record.player_score).ToList();
    }
}
