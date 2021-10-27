using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShieldController : MonoBehaviour
{
    public bool isInvincible;
    public float invincibleTime;
    public GameObject invincibleUI;
    public TextMeshProUGUI invincibleText;
    public Animator invincibleAnimator;
    public ParticleSystem invincibleParicleUI;
    public GameObject sprite;
    public CircleCollider2D circleCollider2D;
    public float damage;
    public string damageParticleTag;
    public GameObject invincibleParicle;
    public Color particleColor;

    private float damageTemp;
    private bool isInvincibleTemp;
    private float invincibleTimer;

    private void Start()
    {
        if (sprite == null)
            sprite = transform.GetChild(0).gameObject;

        circleCollider2D = GetComponent<CircleCollider2D>();
        particleColor = transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color;
        damageTemp = damage;
        isInvincibleTemp = isInvincible;
        invincibleTimer = invincibleTime;
        invincibleUI.SetActive(false);
    }

    private void Update()
    {
        circleCollider2D.enabled = sprite.activeSelf;

        if (isInvincible)
        {
            if (invincibleTimer > 0f)
            {
                invincibleUI.SetActive(true);
                invincibleText.SetText(invincibleTimer.ToString("0.00") + "s");
                invincibleTimer -= Time.deltaTime;
                if (invincibleTimer < invincibleTime / 2)
                    invincibleAnimator.SetTrigger("isBlinking");
            }
            else
            {
                invincibleUI.SetActive(false);
                invincibleTimer = invincibleTime;
                isInvincible = false;
            }
        }

        if (isInvincible == isInvincibleTemp)
            return;

        isInvincibleTemp = isInvincible;

        if (isInvincible)
        {
            damage *= 1000f;
            invincibleParicle.SetActive(true);
        }
        else
        {
            damage = damageTemp;
            invincibleParicle.SetActive(false);
        }

    }

    public void SetShieldInvincible()
    {
        isInvincible = true;
        invincibleTimer = invincibleTime;
        invincibleParicleUI.Play();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (damage == 0)
            return;

        SoundManager.Instance.PlaySoundOneShot(SoundManager.SoundType.shieldHit, false);
        if (other.transform.TryGetComponent<EnemyAI>(out var enemy))
        {
            if (enemy.isBoss)
                return;
            if (!isInvincible)
            {
                var particle = ObjectPooler.Instance.SpawnFromPool(damageParticleTag, other.GetContact(0).point, null);
                var main = particle.GetComponent<ParticleSystem>().main;
                main.startColor = particleColor;
                var dir = (Vector2)other.transform.position - other.GetContact(0).point;
                particle.transform.up = dir.normalized;
            }
            enemy.GetDamaged(damage);
        }
        if (other.transform.TryGetComponent<EnemyLairAI>(out var enemyLair))
        {
            if (!isInvincible)
            {
                var particle = ObjectPooler.Instance.SpawnFromPool(damageParticleTag, other.GetContact(0).point, null);
                var main = particle.GetComponent<ParticleSystem>().main;
                main.startColor = particleColor;
                var dir = (Vector2)other.transform.position - other.GetContact(0).point;
                particle.transform.up = dir.normalized;
            }
            enemyLair.GetDamaged(damage);
        }

        if (other.transform.parent != null && other.transform.parent.TryGetComponent<BossEgg>(out var egg))
        {
            if (!isInvincible)
            {
                var particle = ObjectPooler.Instance.SpawnFromPool(damageParticleTag, other.GetContact(0).point, null);
                var main = particle.GetComponent<ParticleSystem>().main;
                main.startColor = particleColor;
                var dir = (Vector2)other.transform.position - other.GetContact(0).point;
                particle.transform.up = dir.normalized;
            }
            egg.GetDamaged(damage);
        }
    }
}
