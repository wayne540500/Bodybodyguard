using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDead : MonoBehaviour
{
    public void Dead()
    {
        Destroy(transform.parent.gameObject);
    }
}
