using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class GameSaveLoadManager : MonoBehaviour
{
    public Transform ship;
    public Transform enemyLairs;
    public Transform enemySpawnPoints;
    public Animator savingIconAnimator;
    public static GameSaveLoadManager Instance;

    private List<EnemyLairAI> enemyLairAIs;
    private List<EnemySpawnController> enemySpawnControllers;
    private Ship shipScript;

    private void OnEnable()
    {
        Instance = this;

        if (ship == null)
            ship = GameObject.FindGameObjectWithTag("Ship").transform;

        if (enemyLairs == null)
            enemyLairs = GameObject.FindGameObjectWithTag("EnemyLairs").transform;

        if (enemySpawnPoints == null)
            enemySpawnPoints = GameObject.FindGameObjectWithTag("EnemySpawnPoints").transform;

        shipScript = ship.GetComponent<Ship>();

        enemyLairAIs = new List<EnemyLairAI>();
        enemySpawnControllers = new List<EnemySpawnController>();

        foreach (Transform enemyLair in enemyLairs)
        {
            enemyLairAIs.Add(enemyLair.GetComponentInChildren<EnemyLairAI>());
        }

        foreach (Transform enemySpawnPoint in enemySpawnPoints)
        {
            var enemySpawnController = enemySpawnPoint.GetComponent<EnemySpawnController>();
            enemySpawnController.ClearAllEnemy();
            enemySpawnController.SpawnEnemies();
            enemySpawnControllers.Add(enemySpawnController);
        }

        if (File.Exists(Application.persistentDataPath + "/Save.json"))
            LoadData();
        else
            SaveData();
    }

    public void SaveData()
    {
        savingIconAnimator.SetTrigger("isSaving");
        var saveData = new GameSaveData();
        saveData.LevelName = SceneManager.GetActiveScene().name;
        saveData.shipData = shipScript.shipData;
        saveData.sheildData = shipScript.sheildData;
        saveData.shipData.shipPosition = ship.position;
        saveData.LairIsDead = new bool[enemyLairAIs.Count];
        for (int i = 0; i < enemyLairAIs.Count; i++)
        {
            saveData.LairIsDead[i] = enemyLairAIs[i].isDead;
        }

        string jsonData = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(Application.persistentDataPath + "/Save.json", jsonData);
    }
    public void SaveDataWithOutPosition()
    {
        savingIconAnimator.SetTrigger("isSaving");
        var saveData = new GameSaveData();
        saveData.LevelName = SceneManager.GetActiveScene().name;
        saveData.shipData = shipScript.shipData;
        saveData.sheildData = shipScript.sheildData;
        saveData.LairIsDead = new bool[enemyLairAIs.Count];
        for (int i = 0; i < enemyLairAIs.Count; i++)
        {
            saveData.LairIsDead[i] = enemyLairAIs[i].isDead;
        }

        string jsonData = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(Application.persistentDataPath + "/Save.json", jsonData);
    }

    public void LoadData()
    {
        if (!File.Exists(Application.persistentDataPath + "/Save.json"))
        {
            Debug.Log("GameSave doesn't exist!!!");
            return;
        }

        foreach (var enemySpawnController in enemySpawnControllers)
        {
            enemySpawnController.ClearAllEnemy();
            enemySpawnController.SpawnEnemies();
            enemySpawnController.GetComponent<GameObjectsActiveController>().SetGameObjectsList();
        }

        if (ObjectPooler.Instance != null)
        {
            for (int i = 0; i < ObjectPooler.Instance.transform.childCount; i++)
            {
                ObjectPooler.Instance.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        var saveData = new GameSaveData();
        saveData = JsonUtility.FromJson<GameSaveData>(File.ReadAllText(Application.persistentDataPath + "/Save.json"));
        shipScript.shipDeadParticle.gameObject.SetActive(false);
        shipScript.shipData = saveData.shipData;
        GameDataManager.lairCurrentNumber = GameDataManager.lairTotalNumber = 0;
        if (SceneManager.GetActiveScene().name != saveData.LevelName)
        {
            saveData.LevelName = SceneManager.GetActiveScene().name;
            saveData.shipData.shipPosition = ship.position;
            saveData.LairIsDead = new bool[enemyLairAIs.Count];

            shipScript.SetCurrentHP(saveData.shipData.maxHealth);
            shipScript.SetCurrentShieldHP(saveData.sheildData.maxShieldHP);

            for (int i = 0; i < enemyLairAIs.Count; i++)
            {
                saveData.LairIsDead[i] = enemyLairAIs[i].isDead;
            }
            string jsonData = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(Application.persistentDataPath + "/Save.json", jsonData);
            return;
        }
        shipScript.SetCurrentHP(saveData.shipData.maxHealth);
        shipScript.SetCurrentShieldHP(saveData.sheildData.maxShieldHP);
        ship.position = saveData.shipData.shipPosition;

        for (int i = 0; i < saveData.LairIsDead.Length; i++)
        {
            enemyLairAIs[i].isDead = saveData.LairIsDead[i];
            enemyLairAIs[i].ResetHealth();
        }

    }

    public int GetLairNumber(){
        return enemyLairAIs.Count;
    }
}
