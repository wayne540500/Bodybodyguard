using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public bool isNewLaser;
    public ContactFilter2D enemy;
    public float damagePerSec;
    public float maxLenth;
    public LineRenderer lineRenderer;
    public ParticleSystem hitParticle;

    private BoxCollider2D boxCollider2D;
    private List<Collider2D> enemyInLaser;

    private void OnValidate()
    {
        if (isNewLaser)
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.SetPosition(1, transform.InverseTransformPoint(transform.position + transform.right * maxLenth));
        }
    }
    private void Start()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        enemyInLaser = new List<Collider2D>();
        if (isNewLaser)
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.useWorldSpace = false;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, Vector2.zero);
            lineRenderer.SetPosition(1, transform.InverseTransformPoint(transform.position + transform.right * maxLenth));
        }
    }


    private void Update()
    {
        if (!isNewLaser)
        {
            if (boxCollider2D.OverlapCollider(enemy, enemyInLaser) > 0)
            {
                foreach (var enemy in enemyInLaser)
                {
                    if (enemy.TryGetComponent<EnemyAI>(out var enemyAI))
                    {
                        enemyAI.GetDamaged(damagePerSec * Time.deltaTime);
                    }
                    if (enemy.TryGetComponent<EnemyLairAI>(out var enemyLair))
                    {
                        enemyLair.GetDamaged(damagePerSec * Time.deltaTime);
                    }
                }
            }
        }
        else
        {
            var hit = Physics2D.Raycast(transform.position, transform.right, maxLenth, enemy.layerMask);
            Debug.DrawLine(transform.position, transform.position + transform.right * maxLenth, Color.blue);
            if (hit.collider != null)
            {
                lineRenderer.SetPosition(1, transform.InverseTransformPoint(hit.point));
                hitParticle.transform.position = hit.point;
                hitParticle.gameObject.SetActive(true);
                if (hit.collider.TryGetComponent<EnemyAI>(out var enemyAI))
                {
                    if (enemyAI.isBoss)
                        enemyAI.GetDamaged(damagePerSec * Time.deltaTime / 10f);
                    else
                        enemyAI.GetDamaged(damagePerSec * Time.deltaTime);
                    return;
                }
                if (hit.collider.TryGetComponent<EnemyLairAI>(out var enemyLair))
                {
                    enemyLair.GetDamaged(damagePerSec * Time.deltaTime);
                    return;
                }
                if (hit.collider.TryGetComponent<BasicBullet>(out var bullet))
                {
                    if (bullet.isLaserBall)
                        return;
                    bullet.ExplosionHandler();
                    bullet.gameObject.SetActive(false);
                    return;
                }
                if (hit.collider.TryGetComponent<UnderwaterBomb>(out var bomb))
                {
                    bomb.GetDamaged(damagePerSec * Time.deltaTime);
                }
                if (hit.collider.GetComponentInParent<Ship>() != null)
                {
                    var ship = hit.collider.GetComponentInParent<Ship>();
                    ship.GetDamaged(damagePerSec * Time.deltaTime);
                }
                if (hit.collider.transform.parent != null && hit.collider.transform.parent.TryGetComponent<BossEgg>(out var egg))
                {
                    egg.GetDamaged(damagePerSec * Time.deltaTime);
                }

            }
            else
            {
                lineRenderer.SetPosition(1, transform.InverseTransformPoint(transform.position + transform.right * maxLenth));
                hitParticle.gameObject.SetActive(false);
            }
        }





    }
}
