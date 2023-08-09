using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class RankingSlot : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetRecord(PlayerRecord playerRecord)
    {
        nameText.text = playerRecord.player_id.ToString();
        scoreText.text = playerRecord.player_score.ToString();
    }
}
