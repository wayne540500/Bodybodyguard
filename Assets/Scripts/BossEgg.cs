using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEgg : MonoBehaviour
{
    public float explodeDelay;
    public Animator eggAnimator;
    public float animationMaxSpeed;
    public EnemySpawnController enemySpawnController;
    public GameObjectsActiveController gameObjectsActiveController;
    public Rigidbody2D rbody;
    public GameObject explosionParticle;
    public float maxHealth;
    public Material hitEffectMaterial;
    public Color deadParticleColor;
    public string deadParticleTag;
    public int maxEnemyNumber;
    public Transform enemyPool;
    public string[] dropPickupTag;
    public float dropProbability;

    private float explodeTimer;
    private float currentHealth;
    private List<GameObject> enemyList;
    private Material originalMaterial;
    private SpriteRenderer spriteRenderer;
    private bool isExploded;

    private void Awake()
    {
        isExploded = false;
        enemySpawnController.maxEnemyNumber = maxEnemyNumber;
        enemySpawnController.SpawnEnemies();
        enemyList = enemySpawnController.GetEnemyList();
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
    }

    private void OnEnable()
    {
        isExploded = false;
        gameObjectsActiveController.enabled = false;
        if (enemyPool.childCount < maxEnemyNumber)
        {
            enemySpawnController.ClearAllEnemy();
            enemySpawnController.SpawnEnemies();
            enemyList = enemySpawnController.GetEnemyList();
            gameObjectsActiveController.SetGameObjectsList();
        }
        for (int i = 0; i < enemyList.Count; i++)
        {
            enemyList[i].SetActive(false);
        }
        currentHealth = maxHealth;
        explodeTimer = explodeDelay;
        rbody.isKinematic = false;
        eggAnimator.SetFloat("Speed", 1f);
        eggAnimator.SetBool("startBirth", false);
        eggAnimator.SetBool("explode", false);
    }

    private void Update()
    {
        if (isExploded)
            return;
        if (currentHealth <= 0)
        {
            Dead();
            return;
        }
        if (explodeTimer < explodeDelay / 3)
        {
            eggAnimator.SetBool("startBirth", true);
            var speed_temp = eggAnimator.GetFloat("Speed");
            eggAnimator.SetFloat("Speed", speed_temp + (animationMaxSpeed / (explodeDelay / 3) * Time.deltaTime));
        }
        if (explodeTimer > 0)
        {
            explodeTimer -= Time.deltaTime;
        }
        else
        {
            eggAnimator.SetBool("explode", true);
            explodeTimer = 0;
        }
    }

    public void Explode()
    {
        SoundManager.Instance.PlaySoundOneShot(SoundManager.SoundType.bossEggExplode, false);
        explosionParticle.SetActive(true);
        for (int i = 0; i < enemyList.Count; i++)
        {
            if (enemyList[i] == null)
                continue;
            enemyList[i].GetComponent<EnemyAI>().SetStartPosition(enemySpawnController.GetRandomPostion());
            enemyList[i].transform.position = transform.position;
            enemyList[i].SetActive(true);
        }
        rbody.velocity = Vector2.zero;
        rbody.isKinematic = true;
        isExploded = true;
        gameObjectsActiveController.enabled = true;
    }

    public void Dead()
    {
        SoundManager.Instance.PlaySoundOneShot(SoundManager.SoundType.bossEggExplode, false);
        var particle = ObjectPooler.Instance.SpawnFromPool(deadParticleTag, transform.position, null).GetComponent<ParticleSystem>().main;
        particle.startColor = deadParticleColor;
        if (dropPickupTag.Length != 0)
        {
            if (Random.Range(0f, 1f) <= dropProbability)
            {
                var index = Random.Range(0, dropPickupTag.Length);
                if (!string.IsNullOrEmpty(dropPickupTag[index]))
                    ObjectPooler.Instance.SpawnFromPool(dropPickupTag[index], transform.position, null);
            }
        }
        gameObject.SetActive(false);
    }

    public void GetDamaged(float damage)
    {
        currentHealth -= damage;
        spriteRenderer.material = hitEffectMaterial;
        Invoke("ResetMaterial", 0.1f);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Dead();
        }
    }

    public void ResetMaterial()
    {
        spriteRenderer.material = originalMaterial;
    }
}
