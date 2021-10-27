using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Droop : MonoBehaviour
{
    public Animator animator;

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.TryGetComponent<BasicBullet>(out var bullet)){
            animator.SetTrigger("isDamaged");
        }
    }
}
