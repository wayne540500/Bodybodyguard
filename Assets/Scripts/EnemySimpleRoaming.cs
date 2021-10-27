using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySimpleRoaming : MonoBehaviour
{
    public float roamingRange;
    public float moveSpeed;
    public float waitTimer;
    private Rigidbody2D Rigidbody2D;
    private Vector3 roamingPosition;
    private float timer = 0f;

    private void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        roamingPosition = GetRoamingPostion();
    }

    private void Update()
    {
        Romaing();
    }

    private Vector3 GetRoamingPostion()
    {
        return transform.parent.position + new Vector3(Random.Range(-1f, 1f) * roamingRange, Random.Range(-1f, 1f) * roamingRange);
    }

    private void MoveTo(Vector3 targetPosition)
    {
        Debug.DrawLine(transform.position, roamingPosition);
        Rigidbody2D.velocity = (targetPosition - transform.position).normalized * moveSpeed;
        transform.up = Rigidbody2D.velocity.normalized;
    }

    private void Romaing()
    {
        var distance = (transform.position - roamingPosition).sqrMagnitude;

        if (distance < 1f)
        {
            Rigidbody2D.velocity = Vector2.zero;
            if (timer > waitTimer)
            {
                roamingPosition = GetRoamingPostion();
                timer = 0;
            }
            else
                timer += Time.deltaTime;
        }
        else
        {
            MoveTo(roamingPosition);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.parent.position, roamingRange);
    }
}
