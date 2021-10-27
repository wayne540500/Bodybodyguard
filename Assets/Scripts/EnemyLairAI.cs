using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class EnemyLairAI : MonoBehaviour
{
    public bool isDead;
    public EnemyLairData enemyLairData;
    public Transform virusPool;
    public int maxEnemyNumber;
    public EnemySpawnController enemySpawnController;
    public CinemachineVirtualCamera virtualCamera;
    public Animator CameraAnimator;
    public string[] DropingPoolTag;

    [SerializeField] private float currentHealth;
    private List<GameObject> enemyList;
    private Transform ship;
    private float spawnTimer = 0f;
    private int virusIndex = 0;
    private bool isFirstDead = true;

    private void Awake()
    {
        ship = GameObject.FindGameObjectWithTag("Ship").transform;
        enemyList = new List<GameObject>();
        enemySpawnController.maxEnemyNumber = maxEnemyNumber;
        enemySpawnController.SpawnEnemies();
        enemyList = enemySpawnController.GetEnemyList();
        for (int i = 0; i < enemyList.Count; i++)
        {
            if (enemyList[i] == null)
                enemyList.RemoveAt(i);
            enemyList[i].GetComponent<EnemyAI>().SetStartPosition(enemySpawnController.GetRandomPostion());
        }

    }

    private void Start()
    {
        if (isDead)
        {
            GameDataManager.lairCurrentNumber++;
            gameObject.SetActive(false);
        }
        currentHealth = enemyLairData.maxHealth;
        GameDataManager.lairTotalNumber++;
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Dead();
            return;
        }

        if ((transform.position - ship.position).sqrMagnitude < enemyLairData.detectShipRange * enemyLairData.detectShipRange)
        {
            if (virtualCamera.Priority != 20)
                virtualCamera.Priority = 20;
            if (spawnTimer >= enemyLairData.SpawnRate)
            {
                ReleaseVirus();
                spawnTimer = 0f;
            }
            else
            {
                spawnTimer += Time.deltaTime;
            }
        }
        else
        {
            spawnTimer = 0f;
            if (virtualCamera.Priority != 0)
                virtualCamera.Priority = 0;
        }
    }

    public void GetDamaged(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0f)
            Dead();
    }

    public void ResetHealth()
    {
        currentHealth = enemyLairData.maxHealth;
    }

    private void ReleaseVirus()
    {
        if (virusIndex > enemyList.Count - 1)
            return;

        enemyList[virusIndex].transform.position = transform.position;
        enemyList[virusIndex].SetActive(true);
        virusIndex++;
    }

    private void Dead()
    {
        if (isFirstDead)
        {
            //GameDataManager.lairCurrentNumber++;
            isDead = true;
            StartCoroutine(AddLairCurrentNumber());
            CameraAnimator.SetBool("isDead", true);
            isFirstDead = false;
            return;
        }


    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, enemyLairData.detectShipRange);
    }

    IEnumerator AddLairCurrentNumber()
    {
        yield return new WaitForSeconds(110f / 60f);
        foreach (Transform sprite in transform)
        {
            sprite.gameObject.SetActive(false);
        }
        yield return new WaitForSeconds((202f - 110f) / 60f);
        GameDataManager.lairCurrentNumber++;
        for (int i = 0; i < DropingPoolTag.Length; i++)
        {
            if (!string.IsNullOrEmpty(DropingPoolTag[i]))
            {
                var randomPosiotion = transform.position + (new Vector3(Random.Range(-1f, 1), Random.Range(-1f, 1)) * 10f);
                ObjectPooler.Instance.SpawnFromPool(DropingPoolTag[i], randomPosiotion, null);
            }
        }
        GameSaveLoadManager.Instance.SaveData();
        yield return new WaitForSeconds(2f);
        Destroy(virtualCamera.gameObject);
        Destroy(gameObject);
    }
}
