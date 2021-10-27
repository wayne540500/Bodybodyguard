using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    public bool isContainTurret;
    public float rotateAngle;
    [Range(0f, 1000f)]
    public float spawnAreaHeight;
    [Range(0f, 1000f)]
    public float spawnAreaWeight;
    public int maxEnemyNumber;
    public List<GameObject> enemyPrefabs = new List<GameObject>();

    private List<GameObject> enemyList;

    private void Awake()
    {
        //SpawnEnemies();
    }

    public Vector3 GetRandomPostion()
    {
        //return transform.position + new Vector3(Random.Range(-1f, 1f) * spawnAreaWeight, Random.Range(-1f, 1f) * spawnAreaHeight);
        return transform.position + transform.right.normalized * spawnAreaWeight * Random.Range(-1f, 1f) 
                + transform.up.normalized * spawnAreaHeight * Random.Range(-1f, 1f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        var point1 = transform.position + transform.right.normalized * spawnAreaWeight + transform.up.normalized * spawnAreaHeight;
        var point2 = transform.position + transform.right.normalized * -spawnAreaWeight + transform.up.normalized * spawnAreaHeight;
        var point3 = transform.position + transform.right.normalized * -spawnAreaWeight + transform.up.normalized * -spawnAreaHeight;
        var point4 = transform.position + transform.right.normalized * spawnAreaWeight + transform.up.normalized * -spawnAreaHeight;
        Gizmos.DrawLine(point1, point2);
        Gizmos.DrawLine(point2, point3);
        Gizmos.DrawLine(point3, point4);
        Gizmos.DrawLine(point4, point1);
        //Gizmos.DrawWireCube(transform.position, (transform.right.normalized * spawnAreaWeight + transform.up.normalized * spawnAreaHeight) * 2);
    }

    private void OnValidate() {
        if(isContainTurret)
            rotateAngle = transform.eulerAngles.z;
    }

    public void SpawnEnemies()
    {
        if (enemyPrefabs.Count == 0)
            return;

        enemyList = new List<GameObject>();
        Random.InitState(Random.Range(0, 50));

        for (int i = 0; i < maxEnemyNumber; i++)
        {
            var randomIndex = Random.Range(0, enemyPrefabs.Count);
            var randomPosition = GetRandomPostion();
            GameObject enemy = Instantiate(enemyPrefabs[randomIndex], randomPosition, Quaternion.identity, transform);
            if (isContainTurret)
            {
                var turretEnemyAi = enemy.GetComponent<EnemyAI>();
                enemy.transform.GetChild(0).rotation = Quaternion.Euler(0, 0, rotateAngle);
                turretEnemyAi.SetStartPosition(transform.position);
                turretEnemyAi.enemyData.roamRange = spawnAreaHeight == 0 ? spawnAreaWeight : spawnAreaHeight;
            }
            enemyList.Add(enemy);
            enemy.SetActive(false);
        }
    }

    public void ClearAllEnemy()
    {
        foreach (Transform enemy in transform)
        {
            Destroy(enemy.gameObject);
        }
    }

    public List<GameObject> GetEnemyList()
    {
        return enemyList;
    }
}
