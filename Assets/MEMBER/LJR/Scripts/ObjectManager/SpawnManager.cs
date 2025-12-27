using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] GameObject[] enemyPrefabs; // 몬스터 prefab 배열
    [SerializeField] Transform[] spawnPoints;   // 스폰 위치 배열 (던전 내 특정 포인트)

    [SerializeField] List<GameObject> enemyList;

    [SerializeField] GameObject bossPrefab;     // 보스 몬스터 prefab
    [SerializeField] Transform bossSpawnPoint;  // 보스 몬스터 스폰 위치


    private void Start()
    {
        SpawnEnemies();
    }


    public void SpawnEnemies()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            foreach(Transform position in spawnPoints[i])
            {
                // 랜덤한 몬스터 prefab 선택
                GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

                // 몬스터가 스폰 위치에 겹치지 않도록 위치 조정
                GameObject enemy = Instantiate(enemyPrefab, position.position, Quaternion.identity);
                enemyList.Add(enemy); // 생성된 몬스터를 리스트에 추가
            }
        }
        
        GameObject boss = Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity); // 보스 몬스터 생성
        enemyList.Add(boss); // 보스 몬스터를 리스트에 추가
    }

    public void DestroyEnemies()
    {
        foreach( GameObject enemy in enemyList)
        {
            if (enemy != null)
            {
                Destroy(enemy); // 몬스터 제거
            }
        }
    }
}