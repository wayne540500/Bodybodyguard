using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarFlickeringSpeed : MonoBehaviour
{
    private Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetFloat("FlickeringSpeed", Random.Range(0.3f, 1f));
    }

}


