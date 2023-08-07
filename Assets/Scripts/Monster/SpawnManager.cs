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
        for (int i = 0; i < monsterDataList.Count; i++)
        {
            if (!monsterDataList[i].IsAlive)
            {
                monsterDataList[i].IsAlive = true;
                Color skinColor = Color.white;

                Enemy enemy = Instantiate(
                    enemyPrefabs[(int)monsterDataList[i].monsterType],
                    monsterDataList[i].spawnerPos.position,
                    monsterDataList[i].spawnerPos.rotation);
                enemy.Setup(monsterDataList[i].baseHP, monsterDataList[i].baseDamage, monsterDataList[i].baseSpeed, skinColor);

                // 몬스터의 인스턴스를 MonsterData에 저장합니다.
                monsterDataList[i].currentEnemyInstance = enemy;

                // 몬스터에 onDeath 이벤트를 등록합니다.
                enemy.onDeath += () => OnMonsterDeath(enemy);

                Debug.Log("spawn!");
            }
        }
    }

    private void OnMonsterDeath(Enemy enemy)
    {
        // 죽은 몬스터의 인스턴스를 찾아 MonsterData에서 처리합니다.
        MonsterData monsterData = monsterDataList.Find(data => data.currentEnemyInstance == enemy);
        if (monsterData != null)
        {
            monsterData.IsAlive = false;
            StartCoroutine(RespawnCoroutine());
        }
    }

    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(5f);
        CreateEnemy();
    }

    void Start()
    {
        CreateEnemy();
    }

    void Update()
    {
        // 기타 업데이트 로직
    }
}
