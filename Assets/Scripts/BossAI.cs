using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using TMPro;

[RequireComponent(typeof(EnemyAI))]
public class BossAI : MonoBehaviour
{
    public bool isDead;
    public Transform ship;
    public Transform shootPoint;
    public Transform eggPool;
    public Transform cameraFollowPoint;
    public float cameraLerpValue;
    public Transform eye;
    public Transform eyeMask;
    public float eyeMoveLength;
    public CinemachineVirtualCamera bossCamera;
    public TentacleBlockController tentacle;
    public Animator animator;
    public string deadExplosionTag;
    [Header("血量條")]
    public Slider bossHealthBar;
    public Image bossHealthBarFill;
    public TextMeshProUGUI bossHealthBarText;
    public Color[] levelHeathColor;
    [Header("進化資料")]
    public GameObject[] evolvePartSprites;
    public float[] EvovleMaxHealth;
    public int evolveIndex;
    [Header("衝撞攻擊資料")]
    public float rushDamper;
    public GameObject rushParticle;
    [Header("產卵攻擊資料")]
    public string[] eggTag;
    public int eggSpawnNumberPerTime;
    public float eggSpawnRate;
    [Header("雷射攻擊資料")]
    public GameObject[] laserPrefab;
    public ParticleSystem laserChargeParticle;
    public GameObject laserChargeBallParticle;
    public float laserChargeBallMaxSize;
    public float laserChargeBallToMaxSpeed;
    public float laserDelay;
    public float laserRotateSpeed;
    public float laserRotateLineLenght;
    [Header("尾獸玉攻擊資料")]
    public string[] laserBallTag;
    public float laserBallSpeed;
    public float laserBallTorqueSpeed;
    public int laserBallNumberPerTime;
    public float laserBallSpawnRate;
    public float laserBallMaxSize;
    public float laserBallToMaxSizeSpeed;
    public ParticleSystem laserBallChargeParicle;

    private EnemyAI enemyAI;
    private BulletManager shipBulletManager;
    private float shipDistance;
    private int attackIndex;
    private bool isFinishAttck;
    private Vector3 targetPosion;
    private bool isFirst;
    private float laserRotateLineY;
    private Vector3 laserRotateLinePointA;
    private Vector3 laserRotateLinePointB;
    private Vector3 laserRotateLinePointC;
    private Vector3 laserRotateDir;
    private Vector3 laserRotateAddDir;
    private bool islaserRotateFromPointA;
    private int eggNumber;
    private int laserBallNumber;
    private GameObject laserBall_Temp;
    private GameObject egg_Temp;
    private Rigidbody2D laserBallRbody_Temp;
    private float timer;
    private int randomIndex;
    private bool cameraFocus;
    private bool isEvolving;

    private void OnValidate()
    {
        enemyAI = GetComponent<EnemyAI>();
        enemyAI.isBoss = true;
    }
    private void Awake()
    {
        ship = GameObject.FindGameObjectWithTag("Ship").transform;
        shipBulletManager = ship.GetComponentInChildren<BulletManager>();
        enemyAI = GetComponent<EnemyAI>();
    }

    private void OnEnable()
    {
        isFinishAttck = true;
        isFirst = true;
        eggNumber = 0;
        laserBallNumber = 0;
        animator.SetBool("isDead", false);
        isEvolving = false;
        cameraFocus = false;
    }

    private void Update()
    {
        shipDistance = (ship.position - transform.position).sqrMagnitude;
        if (shipDistance < enemyAI.enemyData.detectShipRange * enemyAI.enemyData.detectShipRange)
        {
            animator.SetBool("isFindShip", true);
            bossHealthBar.gameObject.SetActive(true);
            RefreshHealthUI();
            EyeFollowShip();
            SetCameraPriority(true);
            if (!cameraFocus)
                cameraFollowPoint.position = Vector2.Lerp(transform.position, ship.position, cameraLerpValue);
        }
        else
        {
            bossHealthBar.gameObject.SetActive(false);
            animator.SetBool("isFindShip", false);
            SetCameraPriority(false);
        }
    }

    private void EyeFollowShip()
    {
        if (isDead)
        {
            var randomDir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            eye.position = eyeMask.position + randomDir * eyeMoveLength;
            return;
        }
        var dir = (ship.position - eyeMask.position).normalized;
        eye.position = eyeMask.position + dir * eyeMoveLength;
    }

    private void SetCameraPriority(bool isOn)
    {
        bossCamera.Priority = isOn ? 15 : 0;
        shipBulletManager.poolMaxRadious = isOn ? 100f : 60f;
    }

    public void Attack()
    {
        if (isDead)
        {
            enemyAI.Rbody2D.velocity = Vector2.zero;
            return;
        }

        if (isEvolving)
            return;

        if (isFinishAttck)
        {
            attackIndex = Random.Range(0, 4);
            isFinishAttck = false;
        }
        switch (attackIndex)
        {
            case 0:
                Rush();
                break;
            case 1:
                ShootEgg();
                break;
            case 2:
                MouthLaser();
                break;
            case 3:
                MouthLaserBall();
                break;

        }
    }
    private void Rush()
    {
        if (isFirst)
        {
            var oneUnitLenght = (ship.position - transform.position).normalized;
            targetPosion = ship.position - oneUnitLenght * 50f;
            isFirst = false;
        }
        if ((targetPosion - transform.position).sqrMagnitude > rushDamper * rushDamper)
        {
            rushParticle.SetActive(true);
            List<Collider2D> collider2Ds = new List<Collider2D>();
            if (enemyAI.Rbody2D.GetContacts(enemyAI.shipLayer, collider2Ds) > 0)
            {
                //damage the ship
                enemyAI.DamageShip(collider2Ds[0]);
                rushParticle.SetActive(false);
                isFirst = true;
                isFinishAttck = true;
                enemyAI.SetState(EnemyAI.State.ChaseTarget);
                return;
            }
            enemyAI.MoveTo(targetPosion, true, true);
            Debug.DrawLine(transform.position, targetPosion, Color.blue);
        }
        else
        {
            rushParticle.SetActive(false);
            isFirst = true;
            isFinishAttck = true;
            enemyAI.SetState(EnemyAI.State.ChaseTarget);
        }
    }
    private void ShootEgg()
    {
        enemyAI.Rbody2D.velocity = Vector3.zero;
        if (eggNumber == eggSpawnNumberPerTime)
        {
            isFinishAttck = true;
            enemyAI.SetState(EnemyAI.State.ChaseTarget);
            timer = 0;
            eggNumber = 0;
            return;
        }

        if (timer > eggSpawnRate)
        {
            randomIndex = Random.Range(0, eggTag.Length);
            egg_Temp = ObjectPooler.Instance.SpawnFromPool(eggTag[randomIndex], shootPoint.position, null);
            if (!egg_Temp.transform.GetChild(0).gameObject.activeSelf)
            {
                egg_Temp.SetActive(false);
                egg_Temp.SetActive(true);
            }
            Invoke("BirthEgg", 1f);
            timer = 0f;
        }
        else
            timer += Time.deltaTime;
    }

    private void BirthEgg()
    {
        //play sound here
        var eggRbody = egg_Temp.GetComponent<Rigidbody2D>();
        eggRbody.velocity = (ship.position - shootPoint.position).normalized * enemyAI.enemyData.BulletSpeed;
        eggNumber++;
    }

    private void MouthLaser()
    {
        enemyAI.Rbody2D.velocity = Vector3.zero;
        if (isFirst)
        {
            SoundManager.Instance.PlaySoundOneShot(SoundManager.SoundType.bossLaserCharge, false);//play sound here
            isFirst = false;
            laserRotateLineY = ship.position.y;
            islaserRotateFromPointA = (ship.position.x - shootPoint.position.x) > 0 ? true : false;
            laserRotateLinePointA = new Vector2(shootPoint.position.x - laserRotateLineLenght / 2, laserRotateLineY);
            laserRotateLinePointB = new Vector2(shootPoint.position.x + laserRotateLineLenght / 2, laserRotateLineY);
            if (islaserRotateFromPointA)
            {
                laserRotateLinePointC = laserRotateLinePointA;
                laserRotateDir = laserRotateLinePointA - shootPoint.position;
            }
            else
            {
                laserRotateLinePointC = laserRotateLinePointB;
                laserRotateDir = laserRotateLinePointB - shootPoint.position;
            }

            laserPrefab[evolveIndex].transform.right = laserRotateDir.normalized;
            var startColor = laserPrefab[evolveIndex].transform.GetChild(0).GetComponent<ParticleSystem>().main.startColor;
            var main = laserChargeParticle.main;
            main.startColor = startColor;
            laserChargeParticle.gameObject.SetActive(true);
            laserChargeBallParticle = laserPrefab[evolveIndex].transform.GetChild(0).gameObject;
            laserChargeBallParticle.transform.localScale = new Vector3(0f, 0f, 1f);
            laserChargeBallParticle.SetActive(true);
        }

        if (timer > laserDelay)
        {
            laserChargeParticle.gameObject.SetActive(false);
            laserChargeBallParticle.transform.localScale = new Vector3(laserChargeBallMaxSize, laserChargeBallMaxSize, 1f);
            laserPrefab[evolveIndex].transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            if (laserChargeBallParticle.transform.localScale.magnitude < new Vector3(laserChargeBallMaxSize, laserChargeBallMaxSize, 1f).magnitude)
            {
                var addSize = laserChargeBallToMaxSpeed * Time.deltaTime;
                laserChargeBallParticle.transform.localScale += new Vector3(addSize, addSize, 0f);
            }
            else
                laserChargeBallParticle.transform.localScale = new Vector3(laserChargeBallMaxSize, laserChargeBallMaxSize, 1f);
            timer += Time.deltaTime;
            return;
        }

        if (laserRotateLinePointC.x > laserRotateLinePointB.x || laserRotateLinePointC.x < laserRotateLinePointA.x)
        {
            timer = 0;
            laserChargeBallParticle.SetActive(false);
            laserPrefab[evolveIndex].transform.GetChild(1).gameObject.SetActive(false);
            isFirst = true;
            isFinishAttck = true;
            enemyAI.SetState(EnemyAI.State.ChaseTarget);
            return;
        }

        laserRotateDir = laserRotateLinePointC - shootPoint.position;
        laserPrefab[evolveIndex].transform.right = laserRotateDir.normalized;

        if (islaserRotateFromPointA)
            laserRotateLinePointC += (laserRotateLinePointB - laserRotateLinePointA) * Time.deltaTime * laserRotateSpeed;
        else
            laserRotateLinePointC += (laserRotateLinePointA - laserRotateLinePointB) * Time.deltaTime * laserRotateSpeed;


    }

    private void MouthLaserBall()
    {
        enemyAI.Rbody2D.velocity = Vector3.zero;
        if (laserBallNumber == laserBallNumberPerTime)
        {
            isFirst = true;
            isFinishAttck = true;
            enemyAI.SetState(EnemyAI.State.ChaseTarget);
            timer = 0;
            laserBallNumber = 0;
            return;
        }

        if (timer > laserBallSpawnRate)
        {
            SpawnLaserBall();
        }
        else
            timer += Time.deltaTime;
    }

    private void SpawnLaserBall()
    {
        if (isFirst)
        {
            SoundManager.Instance.PlaySoundOneShot(SoundManager.SoundType.bossLaserBallCharge, false);
            isFirst = false;
            laserBall_Temp = ObjectPooler.Instance.SpawnFromPool(laserBallTag[evolveIndex], shootPoint.position, null);
            laserBall_Temp.GetComponent<BasicBullet>().isLaserBallChargeUp = true;
            laserBall_Temp.transform.localScale = new Vector3(0f, 0f, 1f);
            Debug.Log("<color=red>laserBall</color>", laserBall_Temp);
            laserBallRbody_Temp = laserBall_Temp.GetComponent<Rigidbody2D>();
            var particle = laserBall_Temp.GetComponentInChildren<ParticleSystem>();
            var startColor = particle.main.startColor;
            var main = laserBallChargeParicle.main;
            main.startColor = startColor;
            laserBallChargeParicle.gameObject.SetActive(true);
        }

        if (laserBall_Temp.transform.localScale.magnitude < new Vector3(laserBallMaxSize, laserBallMaxSize, 1).magnitude)
        {
            laserBallChargeParicle.gameObject.SetActive(true);
            laserBall_Temp.transform.position = shootPoint.position;
            laserBall_Temp.transform.localScale += new Vector3(Time.deltaTime * laserBallToMaxSizeSpeed, Time.deltaTime * laserBallToMaxSizeSpeed, 0);
        }
        else
        {
            isFirst = true;
            timer = 0;
            laserBallNumber++;
            laserBallChargeParicle.gameObject.SetActive(false);
            laserBall_Temp.transform.localScale = new Vector3(laserBallMaxSize, laserBallMaxSize, 1);
            laserBallRbody_Temp.AddTorque(laserBallTorqueSpeed);
            laserBallRbody_Temp.velocity = (ship.position - shootPoint.position).normalized * laserBallSpeed;
            laserBall_Temp.GetComponent<BasicBullet>().isLaserBallChargeUp = false;
        }



    }

    public void ResetCameraFollowPosition()
    {
        cameraFocus = false;
        isEvolving = false;
        cameraFollowPoint.position = Vector2.Lerp(transform.position, ship.position, cameraLerpValue);
    }

    public void RefreshHealthUI()
    {
        bossHealthBarFill.color = levelHeathColor[evolveIndex];
        bossHealthBar.value = enemyAI.GetCurrentHealth() / enemyAI.enemyData.maxHealth;
        bossHealthBarText.SetText((int)enemyAI.GetCurrentHealth() + "/" + (int)enemyAI.enemyData.maxHealth);
    }

    public void Dead()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        ObjectPooler.Instance.SpawnFromPool(deadExplosionTag, transform.position, null);
        Invoke("OpenTentacle", 2f);
    }

    public void OpenTentacle()
    {
        tentacle.isOpen = true;
        Destroy(gameObject);
    }

    public void CheckIsDead()
    {
        if (evolveIndex >= evolvePartSprites.Length)
        {
            evolveIndex = evolvePartSprites.Length;
            cameraFocus = true;
            cameraFollowPoint.position = transform.position;
            isDead = true;
            animator.SetBool("isDead", true);
            Invoke("Dead", 1f);
            return;
        }
        cameraFocus = true;
        isEvolving = true;
        SoundManager.Instance.PlaySoundOneShot(SoundManager.SoundType.bossEvolve, false);
        cameraFollowPoint.position = transform.position;
        Invoke("ResetCameraFollowPosition", 2f);
        evolvePartSprites[evolveIndex].SetActive(true);
        enemyAI.enemyData.maxHealth = EvovleMaxHealth[evolveIndex];
        enemyAI.SetCurrentHealth(1);
        StartCoroutine("HealthBarAnimation");
        evolveIndex++;
    }

    public IEnumerator HealthBarAnimation()
    {
        var i = 1;
        while (i < enemyAI.enemyData.maxHealth)
        {
            enemyAI.SetCurrentHealth(i);
            i += 200;
            yield return new WaitForEndOfFrame();
        }
        enemyAI.SetCurrentHealth(enemyAI.enemyData.maxHealth);
    }


}
