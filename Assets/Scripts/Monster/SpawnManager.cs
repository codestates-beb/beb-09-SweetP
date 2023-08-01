using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private static SpawnManager _instance;
    public static SpawnManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SpawnManager>();
            }
            return _instance;
        }
    }

    public Enemy[] enemyPrefabs;

    public List<MonsterData> monsterDataList = new List<MonsterData>();


    private void CreateEnemy()
    {
        for(int i=0; i<monsterDataList.Count; i++)
        {
            Color skinColor = Color.white;

            Enemy enemy = Instantiate(
            enemyPrefabs[(int)monsterDataList[i].monsterType],
            monsterDataList[i].spawnerPos.position,
            monsterDataList[i].spawnerPos.rotation);
            enemy.Setup(monsterDataList[i].baseHP, monsterDataList[i].baseDamage, monsterDataList[i].baseSpeed, skinColor);
            Debug.Log("spawn!");
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        CreateEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
