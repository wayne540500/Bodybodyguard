using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using TMPro;

public class Ship : MonoBehaviour
{
    public Transform MinimapCamera;
    public Transform shipInside;
    public Slider healthBar;
    public Slider ShieldBar;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI shieldText;
    public TextMeshProUGUI wrenchText;
    public Animator wrenchAnimator;
    public Animator shipDamageEffectAnimator;
    public Animator shipShieldDamageEffectAnimator;
    public Animator shipUpgradeAnimator;
    public Animator shipDeadAnimator;
    public ParticleSystem shipHealParticle;
    public ParticleSystem shipShieldHealParticle;
    public ParticleSystem shipShieldBrokeParticle;
    public ParticleSystem shipDeadParticle;
    public Material hitEffectMaterial;
    public SpriteRenderer shipSprite;
    public MultiplayerEventSystem P1_EventSystem;
    public SpawnPlayers spawnPlayers;
    public bool isFirstPlaySound;
    public ShipData shipData;
    public SheildData sheildData;
    public static Ship Instance;

    [SerializeField] private float currentHealth;
    [SerializeField] private float currentShieldHP;
    private Rigidbody2D rbody2D;
    private float maxHealth_temp;
    private float maxShieldHP_temp;
    private bool canPlayShieldBrokeParticle;
    private bool isFirstDead;
    private Material originalMaterial;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        originalMaterial = shipSprite.material;
        rbody2D = GetComponent<Rigidbody2D>();
        currentHealth = shipData.maxHealth;
        currentShieldHP = sheildData.maxShieldHP;
        healthBar.maxValue = shipData.maxHealth;
        ShieldBar.maxValue = sheildData.maxShieldHP;
        maxHealth_temp = shipData.maxHealth;
        canPlayShieldBrokeParticle = true;
        RefreshUI();
    }

    private void Update()
    {
        MinimapCamera.position = new Vector3(transform.position.x, transform.position.y, -10);
        shipInside.position = transform.position;

        RefreshUI();

        if (maxHealth_temp != shipData.maxHealth)
        {
            healthBar.maxValue = shipData.maxHealth;
            maxHealth_temp = shipData.maxHealth;
            currentHealth = maxHealth_temp;
        }

        if (maxShieldHP_temp != sheildData.maxShieldHP)
        {
            ShieldBar.maxValue = sheildData.maxShieldHP;
            maxShieldHP_temp = sheildData.maxShieldHP;
            currentShieldHP = maxShieldHP_temp;
        }

        if (currentShieldHP <= 0)
        {
            if (canPlayShieldBrokeParticle)
            {
                var main = shipShieldBrokeParticle.main;
                main.startColor = sheildData.shieldSprite.GetComponentInParent<ShieldController>().particleColor;
                shipShieldBrokeParticle.Play();
                canPlayShieldBrokeParticle = false;
            }
        }
        else
            isFirstDead = true;

        sheildData.shieldSprite.SetActive(currentShieldHP > 0 ? true : false);
        canPlayShieldBrokeParticle = currentShieldHP > 0 ? true : false;

        if (currentHealth <= 0)
        {
            currentHealth = 0f;
            Dead();
        }
    }

    public void GetDamaged(float damage)
    {
        //CameraController.Instance.ShakeCamera(damage, .1f);
        if (damage > 100f)
            SoundManager.Instance.PlaySoundOneShot(SoundManager.SoundType.shipDamaged_Large, false);
        else
            SoundManager.Instance.PlaySoundOneShot(SoundManager.SoundType.shipDamaged_Little, false);

        if (currentShieldHP > 0f)
        {
            shipShieldDamageEffectAnimator.SetTrigger("isShipHit");
            currentShieldHP -= damage;
            if (currentShieldHP < 0f)
            {
                SoundManager.Instance.PlaySoundOneShot(SoundManager.SoundType.ShieldBroken, false);
                currentShieldHP = 0f;
                var main = shipShieldBrokeParticle.main;
                main.startColor = sheildData.shieldSprite.GetComponentInParent<ShieldController>().particleColor;
                shipShieldBrokeParticle.Play();
                canPlayShieldBrokeParticle = false;
                sheildData.shieldSprite.SetActive(false);
            }
        }
        else
        {
            shipSprite.material = hitEffectMaterial;
            Invoke("ResetMaterial", 0.1f);
            shipDamageEffectAnimator.SetTrigger("isShipHit");
            currentHealth -= damage;
        }
    }

    public void Dead()
    {
        if (isFirstDead)
        {
            SoundManager.Instance.PlaySound(SoundManager.SoundType.shipExplode, true);
            isFirstDead = false;
        }

        if (!shipDeadParticle.isPlaying)
        {
            CameraController.Instance.GetMainCamera().Priority = 30;
            shipDeadParticle.gameObject.SetActive(true);
            shipDeadParticle.Play();
            shipDeadAnimator.SetBool("isDead", true);
            if (GameDataManager.playerDatas.Count > 0)
            {
                spawnPlayers.ResetPlayersPosition();
            }
        }
    }

    public void Heal(float healPoint)
    {
        shipHealParticle.Play();
        if (currentHealth + healPoint >= maxHealth_temp)
            currentHealth = maxHealth_temp;
        else
            currentHealth += healPoint;
    }
    public void ShieldHeal(float healPoint)
    {
        shipShieldHealParticle.Play();
        if (currentShieldHP + healPoint >= maxShieldHP_temp)
            currentShieldHP = maxShieldHP_temp;
        else
            currentShieldHP += healPoint;
    }

    public float GetCurrentHP()
    {
        return currentHealth;
    }
    public float GetCurrentShieldHP()
    {
        return currentShieldHP;
    }

    public void SetCurrentHP(float HP)
    {
        currentHealth = HP;
    }
    public void SetCurrentShieldHP(float HP)
    {
        currentShieldHP = HP;
    }

    public void GetWrench(int number)
    {
        wrenchAnimator.SetTrigger("isGetWrench");
        shipData.wrenchNumber += number;
    }

    public void RefreshUI()
    {
        wrenchText.SetText($"{shipData.wrenchNumber}/{15 + shipData.upgradeTimes * 10}");
        if (shipData.wrenchNumber >= 15 + shipData.upgradeTimes * 10 && !shipUpgradeAnimator.GetBool("isOpen") && shipData.upgradeTimes < 21)
        {
            CameraController.Instance.GetMainCamera().Priority = 30;
            if (isFirstPlaySound)
            {
                SoundManager.Instance.PlaySoundOneShot(SoundManager.SoundType.UpgradeMenuOpen, false);
                isFirstPlaySound = false;
            }
            SoundManager.Instance.PlaySoundOneShot(SoundManager.SoundType.WrenchPickUp, false);
            shipUpgradeAnimator.SetBool("isOpen", true);
            //shipUpgradeAnimator.transform.position = transform.position;
        }
        currentHealth = currentHealth <= 0 ? 0 : currentHealth; 
        currentShieldHP = currentShieldHP <= 0 ? 0 : currentShieldHP; 
        healthBar.value = currentHealth;
        ShieldBar.value = currentShieldHP;
        healthText.SetText($"{(int)currentHealth}/{(int)shipData.maxHealth}");
        shieldText.SetText($"{(int)currentShieldHP}/{(int)sheildData.maxShieldHP}");
    }

    private void ResetMaterial()
    {
        shipSprite.material = originalMaterial;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("ship Hit!!");

        if (other.transform.TryGetComponent<UnderwaterBomb>(out var bomb))
            bomb.GetDamaged(other.relativeVelocity.magnitude);

        if (other.gameObject.layer != 13)
            return;

        Debug.Log("ship Damage: " + other.relativeVelocity.magnitude);


        if (other.relativeVelocity.magnitude > 5f)
            GetDamaged(other.relativeVelocity.magnitude * (shipData.maxHealth / 500f));

    }

}
