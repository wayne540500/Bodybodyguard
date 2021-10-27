using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public float poolMaxRadious;
    private void Update()
    {
        foreach (Transform bullet in transform)
        {
            var distance = (bullet.position - transform.position).sqrMagnitude;
            if (distance > poolMaxRadious * poolMaxRadious)
                bullet.gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, poolMaxRadious);
    }
}
