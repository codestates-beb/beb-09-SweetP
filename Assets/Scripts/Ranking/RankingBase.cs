using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingBase : MonoBehaviour
{

    [SerializeField]
    private GameObject slotsParent;

    public RankingSlot Rslots;

    // Start is called before the first frame update
    void Start()
    {
        AcquireRanking();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AcquireRanking()
    {
        for (int i = 0; i < RankingManager.instance.playerRecords.Count; i++)
        {
            RankingSlot rankingSlot = Rslots;
            rankingSlot.SetRecord(RankingManager.instance.playerRecords[i]);
            Instantiate(rankingSlot, slotsParent.transform);
        }
    }
}
