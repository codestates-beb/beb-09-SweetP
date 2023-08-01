using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleNPC : MonoBehaviour
{
    private GameObject GroundPanel;
    private GameObject RaidPanel;
    private BattleType battleType;
    private GroundMap groundMap;
    public Transform SpawnPosDesert;
    public Transform SpawnPosForest;
    public Transform player;

    public void SelectGround()
    {
        UIManager.instance.BattlePanel.SetActive(false);
        GroundPanel.SetActive(true);
        battleType = BattleType.Ground;
    }

    public void SelectRaid()
    {
        UIManager.instance.BattlePanel.SetActive(false);
        RaidPanel.SetActive(true);
        battleType = BattleType.Raid;
    }

    public void SelectGroundMapDesert()
    {
        print("desert");
        groundMap = GroundMap.Desert;
        UIManager.instance.IsOpenPanel = false;
        GroundPanel.SetActive(false);
        SceneManager.LoadScene("DesertMap");
    }

    public void SelectGroundMapForest()
    {
        groundMap = GroundMap.Forest;
    }

    // Start is called before the first frame update
    void Start()
    {
        GroundPanel = UIManager.instance.GroundPanel;
        RaidPanel = UIManager.instance.RaidPanel;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
