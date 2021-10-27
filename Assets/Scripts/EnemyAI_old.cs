using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI_old : MonoBehaviour
{
    public bool isVirus;
    public float virusAttackMoveSpeed;
    public Transform Ship;
    public ContactFilter2D shipLayer;
    public float currentHealth;
    public float avoidShaking = 1f;
    public float NextRoamingPositionDelay = 1f;
    public LayerMask obstaclesLayer;
    public Transform BulletPrefab;
    [Range(0, 180)] public float findPathScanDegreeRange = 90f;
    [Range(1, 10)] public float colliderRadiusMulti = 2f;
    public float scanPerDegree = 5f;

    [Header("敵人資料")]
    public EnemyData enemyData = new EnemyData();


    private enum State
    {
        Roaming,
        ChaseTarget,
        Attacking,
        Dead,
    }
    private Vector3 startingPosotion;
    private Vector3 roamPosotion;
    private Vector3 targetPosition;
    private bool isReachTarget = false;
    private State state;
    private float nextShootTimer = 0f;
    private float nextMoveTimer = 0f;
    private Transform bulletPool;
    private Animator animator;
    private CircleCollider2D circleCollider2D;
    private Rigidbody2D Rbody2D;
    private Vector2 velocityLastFrame;


    private void Awake()
    {
        Ship = GameObject.FindGameObjectWithTag("Ship").transform;
        if (!isVirus)
            bulletPool = GetComponentInChildren<BulletManager>().transform;
        animator = GetComponentInChildren<Animator>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        Rbody2D = GetComponent<Rigidbody2D>();
        state = State.Roaming;
    }

    private void Start()
    {
        currentHealth = enemyData.maxHealth;
        startingPosotion = transform.position;
        roamPosotion = GetRoamingPostion();
        targetPosition = Vector3.zero;
    }

    private void Update()
    {

        switch (state)
        {

            default:

            case State.Roaming:
                Debug.DrawLine(transform.position, roamPosotion, Color.green);
                Roaming();
                FindTarget();




                break;

            case State.ChaseTarget:
                //if (Vector3.Distance(transform.position, Ship.position) > enemyData.detectShipRange)
                //    state = State.Roaming;

                Debug.DrawLine(transform.position, Ship.position, Color.red);

                velocityLastFrame = Rbody2D.velocity;

                FindThePath();
                //AvoidStuckAtCorner();

                break;

            case State.Attacking:
                Attack();
                break;

            case State.Dead:
                animator.SetBool("isDead", true);
                break;
        }

        if (currentHealth <= 0)
        {
            state = State.Dead;
        }

    }

    private Vector3 GetRoamingPostion()
    {
        return startingPosotion + new Vector3(Random.Range(-1f, 1f) * enemyData.roamRange, Random.Range(-1f, 1f) * enemyData.roamRange);
    }

    private void MoveTo(Vector3 targetPosition, bool isApproaching, bool isAttacking)
    {
        var speedMultiply = isAttacking ? virusAttackMoveSpeed : enemyData.moveSpeed;

        if (isApproaching)
        {
            if (isVirus)
            {
                var spriteUpDir = (targetPosition - transform.position).normalized;
                GetComponentInChildren<SpriteRenderer>().transform.up = spriteUpDir;
            }
            Rbody2D.velocity = (targetPosition - transform.position).normalized * speedMultiply;
            //transform.Translate((targetPosition - transform.position).normalized * speedMultiply * Time.deltaTime);
        }
        else
        {
            Rbody2D.velocity = (transform.position - targetPosition).normalized * speedMultiply;
            //transform.Translate((transform.position - targetPosition).normalized * speedMultiply * Time.deltaTime);
        }
    }

    private void FindTarget()
    {
        if (Vector3.Distance(transform.position, Ship.position) < enemyData.detectShipRange)
        {
            state = State.ChaseTarget;


        }
        else
        {
            state = State.Roaming;
        }
    }

    private void Roaming()
    {
        var dir = roamPosotion - transform.position;

        RaycastHit2D hit2D = Physics2D.Raycast(transform.position, dir.normalized, dir.magnitude + circleCollider2D.radius * 1.5f, obstaclesLayer);

        if (hit2D.collider != null)
        {
            roamPosotion = new Vector3(hit2D.point.x, hit2D.point.y) - dir.normalized * circleCollider2D.radius * 1.5f;

        }
        if (dir.magnitude <= 0.5f)
        {
            Rbody2D.velocity = Vector2.zero;
            if (nextMoveTimer > NextRoamingPositionDelay)
                roamPosotion = GetRoamingPostion();
            else
                nextMoveTimer += Time.deltaTime;

        }
        else
        {
            MoveTo(roamPosotion, true, false);
            nextMoveTimer = 0;
        }
    }

    private void Attack()
    {
        if (isVirus)
        {
            List<Collider2D> collider2Ds = new List<Collider2D>();
            if (transform.GetComponent<Rigidbody2D>().GetContacts(shipLayer, collider2Ds) > 0)
            {
                //damage the ship
                DamageShip();
                state = State.ChaseTarget;
            }
            else
            {
                MoveTo(Ship.position, true, true);
                Debug.DrawLine(transform.position, Ship.position, Color.blue);
            }

        }
        else
        {
            var bullet = Instantiate(BulletPrefab, transform.position, Quaternion.identity, bulletPool);
            var bulletRbody = bullet.GetComponent<Rigidbody2D>();
            bulletRbody.velocity = (Ship.position - transform.position).normalized * enemyData.BulletSpeed;
            bullet.transform.up = bulletRbody.velocity.normalized;
            bullet.GetComponent<BasicBullet>().bulletData.targetTag = "Ship";
            bullet.GetComponent<BasicBullet>().bulletData.damage = enemyData.attackDamage;
            state = State.ChaseTarget;
        }

    }

    private void FindThePath()
    {


        var toShipDir = Ship.position - transform.position;

        RaycastHit2D hit2D = Physics2D.Raycast(transform.position, toShipDir.normalized, toShipDir.magnitude, obstaclesLayer);

        //Plan B
        /*if (targetPosition != Vector3.zero)
        {
            MoveTo(targetPosition, true, false);
            Debug.DrawLine(transform.position, targetPosition, Color.red);
            if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
                targetPosition = Vector3.zero;

            return;
        }*/


        if (hit2D.collider == null)
        {

            if (Vector3.Distance(transform.position, targetPosition) >= 0.5f && targetPosition != Ship.position)
            {
                MoveTo(targetPosition, true, false);
                return;
            }
            else
                targetPosition = Ship.position;

            if (Vector3.Distance(transform.position, Ship.position) < enemyData.ClosestDistanceToShip - avoidShaking)
            {
                //move back
                MoveTo(Ship.position, false, false);
            }
            else if (Vector3.Distance(transform.position, Ship.position) > enemyData.ClosestDistanceToShip + avoidShaking)
            {
                //move toward
                MoveTo(Ship.position, true, false);
            }
            else
            {
                //attcak
                Rbody2D.velocity = Vector2.zero;

                //transform.RotateAround(Ship.position, Vector3.forward, enemyData.inAttackRangeMoveSpeed * Time.deltaTime);

                //transform.rotation = Quaternion.identity;

                if (isVirus)
                {
                    var spriteUpDir = (Ship.position - transform.position).normalized;
                    GetComponentInChildren<SpriteRenderer>().transform.up = spriteUpDir;
                }

                if (nextShootTimer > enemyData.FireRate)
                {
                    //Attack();
                    state = State.Attacking;
                    nextShootTimer = 0;
                }
                else
                    nextShootTimer += Time.deltaTime;
            }

            return;
        }

        Rbody2D.velocity = Vector2.zero;

        var firstCollider_temp = hit2D.collider;

        var contactPoint_N = Vector2.zero;
        var contactPoint_P = Vector2.zero;

        var nextNodePosition = Vector2.zero;

        var firstHitPoint = hit2D.point;

        for (float i = 0; i < findPathScanDegreeRange; i += scanPerDegree)
        {
            var RotatedX_N = (toShipDir.x * Mathf.Cos(-i * Mathf.Deg2Rad) - toShipDir.y * Mathf.Sin(-i * Mathf.Deg2Rad));
            var RotatedY_N = (toShipDir.x * Mathf.Sin(-i * Mathf.Deg2Rad) + toShipDir.y * Mathf.Cos(-i * Mathf.Deg2Rad));
            var RotatedX_P = toShipDir.x * Mathf.Cos(i * Mathf.Deg2Rad) - toShipDir.y * Mathf.Sin(i * Mathf.Deg2Rad);
            var RotatedY_P = toShipDir.x * Mathf.Sin(i * Mathf.Deg2Rad) + toShipDir.y * Mathf.Cos(i * Mathf.Deg2Rad);

            Vector3 Rotate_N = new Vector2(RotatedX_N, RotatedY_N);
            Vector3 Rotate_P = new Vector2(RotatedX_P, RotatedY_P);

            //Debug.DrawLine(transform.position, transform.position + Rotate_N, Color.blue);
            //Debug.DrawLine(transform.position, transform.position + Rotate_P, Color.yellow);

            hit2D = Physics2D.Raycast(transform.position, Rotate_N.normalized, toShipDir.magnitude, obstaclesLayer);
            RaycastHit2D hit2D1 = Physics2D.Raycast(transform.position, Rotate_P.normalized, toShipDir.magnitude, obstaclesLayer);

            //Plan C
            /*if (hit2D.collider != null)
                contactPoint_N = hit2D.point;

            if (hit2D1.collider != null)
                contactPoint_P = hit2D1.point;*/

            //Plan B
            if (hit2D.collider != null)
                contactPoint_N = hit2D.point;

            if (hit2D1.collider != null)
                contactPoint_P = hit2D1.point;

            if (hit2D.collider == null && hit2D1.collider == null)
            {
                var distance_N = ProjectPointConverter(transform.position, transform.position + Rotate_N, contactPoint_N) - (Vector2)transform.position;
                var distance_P = ProjectPointConverter(transform.position, transform.position + Rotate_P, contactPoint_P) - (Vector2)transform.position;

                nextNodePosition = distance_N.magnitude > distance_P.magnitude ? distance_P + (Vector2)transform.position : distance_N + (Vector2)transform.position;
                Debug.DrawLine(transform.position, nextNodePosition, Color.grey);
                MoveTo(nextNodePosition, true, false);
                targetPosition = nextNodePosition;
                break;
            }


            //Plan A
            /*if (hit2D.collider != null)
                contactPoint_N = hit2D.point;

            if (hit2D1.collider != null)
                contactPoint_P = hit2D1.point;

            if (hit2D.collider == null && hit2D1.collider == null)
            {
                var distance_N = ProjectPointConverter(transform.position, transform.position + Rotate_N, contactPoint_N) - (Vector2)transform.position;
                var distance_P = ProjectPointConverter(transform.position, transform.position + Rotate_P, contactPoint_P) - (Vector2)transform.position;

                nextNodePosition = distance_N.magnitude > distance_P.magnitude ? distance_P + (Vector2)transform.position : distance_N + (Vector2)transform.position;
                Debug.DrawLine(transform.position, nextNodePosition, Color.grey);
                MoveTo(nextNodePosition, true, false);
                targetPosition = nextNodePosition;
                break;
            }
            else
            {
                if (hit2D.collider == null)
                {
                    nextNodePosition = ProjectPointConverter(transform.position, transform.position + Rotate_N, contactPoint_N);
                    Debug.DrawLine(transform.position, nextNodePosition, Color.green);
                    MoveTo(nextNodePosition, true, false);
                    targetPosition = nextNodePosition;
                    break;
                }

                if (hit2D1.collider == null)
                {
                    nextNodePosition = ProjectPointConverter(transform.position, transform.position + Rotate_P, contactPoint_P);
                    Debug.DrawLine(transform.position, nextNodePosition, Color.white);
                    MoveTo(nextNodePosition, true, false);
                    targetPosition = nextNodePosition;
                    break;
                }

            }*/

        }

        /*if (nextNodePosition == Vector2.zero)
        {
            targetPosition = Vector3.zero;
            state = State.Roaming;
        }*/

        //Plan C
        /*if (contactPoint_N == Vector2.zero || contactPoint_P == Vector2.zero)
        {
            Rbody2D.velocity = velocityLastFrame;
            return;
        }

        var vectorLength_N = (contactPoint_N - firstHitPoint);
        var vectorLength_P = (contactPoint_P - firstHitPoint);

        Debug.DrawLine(firstHitPoint, contactPoint_N, Color.black);
        Debug.DrawLine(firstHitPoint, contactPoint_P, Color.black);

        Rbody2D.velocity = vectorLength_N.magnitude < vectorLength_P.magnitude ? vectorLength_N.normalized * enemyData.moveSpeed
        : vectorLength_P.normalized * enemyData.moveSpeed;

        Rbody2D.velocity = vectorLength_N.magnitude == vectorLength_P.magnitude ? vectorLength_N.normalized * enemyData.moveSpeed
        : Rbody2D.velocity;

        Debug.DrawLine(transform.position, (Vector2)transform.position + Rbody2D.velocity, Color.black);*/
    }

    //找出投影點並轉換成新向量
    private Vector2 ProjectPointConverter(Vector2 startPoint, Vector2 endPoint, Vector2 anyPoint)
    {
        float a = endPoint.y - startPoint.y;
        float b = startPoint.x - endPoint.x;
        float c = endPoint.x * startPoint.y - startPoint.x * endPoint.y;
        float denominator = (a * a + b * b);
        float t = (a * anyPoint.x + b * anyPoint.y + c) / denominator;

        var theProjectedPoint = new Vector2(anyPoint.x - a * t, anyPoint.y - b * t);

        Debug.DrawLine(anyPoint, anyPoint + (theProjectedPoint - anyPoint).normalized * circleCollider2D.radius * colliderRadiusMulti);
        return anyPoint + (theProjectedPoint - anyPoint).normalized * circleCollider2D.radius * colliderRadiusMulti;
    }

    //防止卡在牆角
    private void AvoidStuckAtCorner()
    {
        var toShipDir = (Ship.position - transform.position).normalized * circleCollider2D.radius * colliderRadiusMulti;

        for (float i = scanPerDegree; i < findPathScanDegreeRange; i += scanPerDegree)
        {
            var RotatedX_N = (toShipDir.x * Mathf.Cos(-i * Mathf.Deg2Rad) - toShipDir.y * Mathf.Sin(-i * Mathf.Deg2Rad));
            var RotatedY_N = (toShipDir.x * Mathf.Sin(-i * Mathf.Deg2Rad) + toShipDir.y * Mathf.Cos(-i * Mathf.Deg2Rad));
            var RotatedX_P = toShipDir.x * Mathf.Cos(i * Mathf.Deg2Rad) - toShipDir.y * Mathf.Sin(i * Mathf.Deg2Rad);
            var RotatedY_P = toShipDir.x * Mathf.Sin(i * Mathf.Deg2Rad) + toShipDir.y * Mathf.Cos(i * Mathf.Deg2Rad);

            Vector3 Rotate_N = new Vector2(RotatedX_N, RotatedY_N);
            Vector3 Rotate_P = new Vector2(RotatedX_P, RotatedY_P);

            Debug.DrawLine(transform.position, transform.position + Rotate_N, Color.blue);
            Debug.DrawLine(transform.position, transform.position + Rotate_P, Color.yellow);

            RaycastHit2D hit2D = Physics2D.Raycast(transform.position, Rotate_N.normalized, toShipDir.magnitude, obstaclesLayer);
            RaycastHit2D hit2D1 = Physics2D.Raycast(transform.position, Rotate_P.normalized, toShipDir.magnitude, obstaclesLayer);

            if (hit2D.collider != null)
                Rbody2D.velocity += ((Vector2)transform.position - hit2D.point);

            if (hit2D1.collider != null)
                Rbody2D.velocity += ((Vector2)transform.position - hit2D1.point);
        }
    }

    public void GetDamaged(float damage)
    {
        currentHealth -= damage;
    }

    private void DamageShip()
    {
        Debug.Log("hit Ship/Virus");
    }


    private void OnDrawGizmosSelected()
    {

        Gizmos.DrawWireSphere(transform.position, enemyData.detectShipRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(startingPosotion, enemyData.roamRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, enemyData.ClosestDistanceToShip);

    }


}
