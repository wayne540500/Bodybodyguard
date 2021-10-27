using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BasicBullet : MonoBehaviour
{
    public bool isLaserBall;
    public bool isLaserBallChargeUp;
    public bool isMissle;
    public Transform hitPoint;
    public string explosionParicleTag;
    public BulletData bulletData;

    private Rigidbody2D Rbody2D;
    private Vector2 explosionPosition;
    private List<ContactPoint2D> contactPoint2Ds;

    private void Start()
    {
        Rbody2D = GetComponent<Rigidbody2D>();
    }

    private void LateUpdate()
    {
        if (isLaserBallChargeUp)
            return;
        if (Rbody2D.velocity == Vector2.zero)
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isMissle)
            return;

        if (other.TryGetComponent<BossHand>(out var hand) && !gameObject.CompareTag("EnemyBullet"))
        {
            hand.GetDamaged(bulletData.damage);
            ExplosionHandler();
            return;
        }

        if (other.TryGetComponent<UnderwaterBomb>(out var bomb))
        {
            bomb.GetDamaged(bulletData.damage);
            ExplosionHandler();
            return;
        }

        if (other.gameObject.layer == 13 || other.CompareTag("Laser"))
        {
            ExplosionHandler();
            return;
        }

        if (other.gameObject.layer == 12)
        {
            if (other.CompareTag(gameObject.tag))
                return;

            if (other.TryGetComponent<Missle>(out var missle))
            {
                missle.GetDamaged(bulletData.damage);
                if (!isLaserBall)
                    ExplosionHandler();
                return;
            }

            if (other.TryGetComponent<BasicBullet>(out var laserBall) && laserBall.isLaserBall)
            {
                if (this.isLaserBall)
                    laserBall.ExplosionHandler();
                else
                    ExplosionHandler();

                return;
            }
            ExplosionHandler();

        }

        if (other.TryGetComponent<ShieldController>(out var shield) && bulletData.targetTag == "Ship")
        {
            if (shield.isInvincible)
            {
                var hit = Physics2D.Raycast(transform.position, transform.up);
                var newDir = Vector2.Reflect(Rbody2D.velocity, hit.normal);
                Rbody2D.velocity = newDir * 1.5f;
                transform.up = newDir;
                bulletData.targetTag = "Enemy";

            }
            else
            {
                other.GetComponentInParent<Ship>().GetDamaged(bulletData.damage);
                ExplosionHandler();
            }
            return;
        }

        if (other.transform.parent != null && other.transform.parent.TryGetComponent<BossEgg>(out var egg))
        {
            egg.GetDamaged(bulletData.damage);
            ExplosionHandler();
            return;
        }

        if (!other.CompareTag(bulletData.targetTag))
            return;

        if (other.TryGetComponent<EnemyAI>(out var enemy))
        {
            enemy.GetDamaged(bulletData.damage);

        }
        if (other.TryGetComponent<Ship>(out var ship))
        {
            ship.GetDamaged(bulletData.damage);

        }
        if (other.TryGetComponent<EnemyLairAI>(out var lair))
        {
            lair.GetDamaged(bulletData.damage);
        }

        ExplosionHandler();
    }

    public void ExplosionHandler()
    {
        if (!string.IsNullOrEmpty(explosionParicleTag))
        {
            var particle = ObjectPooler.Instance.SpawnFromPool(explosionParicleTag, hitPoint.position, null).GetComponent<ParticleSystem>();
            var particleMain = particle.main;
            if (isLaserBall)
                particle.transform.localScale = transform.localScale;
            if (GetComponentInChildren<SpriteRenderer>() != null)
                particleMain.startColor = GetComponentInChildren<SpriteRenderer>().color;
            if (GetComponentInChildren<ParticleSystem>() != null)
                particleMain.startColor = GetComponentInChildren<ParticleSystem>().main.startColor;
            particle.Play();
        }
        gameObject.SetActive(false);
    }
}
