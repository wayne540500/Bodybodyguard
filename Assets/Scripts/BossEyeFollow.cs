using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEyeFollow : MonoBehaviour
{
    public Transform ship;
    public Transform eye;
    public float eyeMoveLength;
    public BossAI bossAI;

    private void Awake()
    {
        ship = GameObject.FindGameObjectWithTag("Ship").transform;
        eye = transform.GetChild(0);
    }

    private void Update()
    {
        if (bossAI.isDead)
        {
            var randomDir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            eye.position = transform.position + randomDir * eyeMoveLength;
            return;
        }
        var dir = (ship.position - transform.position).normalized;
        eye.position = transform.position + dir * eyeMoveLength;
    }
}
