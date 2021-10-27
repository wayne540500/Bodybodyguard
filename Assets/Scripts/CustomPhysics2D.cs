using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPhysics2D : MonoBehaviour
{
    void Start()
    {
        Physics2D.IgnoreLayerCollision(9, 8);
        Physics2D.IgnoreLayerCollision(8, 8);
        Physics2D.IgnoreLayerCollision(9, 9);
        Physics2D.IgnoreLayerCollision(10, 8);
        Physics2D.IgnoreLayerCollision(14, 8);
        Physics2D.IgnoreLayerCollision(11, 11);
        Physics2D.IgnoreLayerCollision(11, 22);
        Physics2D.IgnoreLayerCollision(9, 13);
        Physics2D.IgnoreLayerCollision(19, 12);
        Physics2D.IgnoreLayerCollision(0, 16);
        Physics2D.IgnoreLayerCollision(0, 0);
        Physics2D.IgnoreLayerCollision(16, 16);
        Physics2D.IgnoreLayerCollision(16, 20);
        Physics2D.IgnoreLayerCollision(0, 20);
        Physics2D.IgnoreLayerCollision(13, 20);
        Physics2D.IgnoreLayerCollision(13, 16);
        Physics2D.IgnoreLayerCollision(13, 13);
        Physics2D.IgnoreLayerCollision(13, 0);
    }


    void Update()
    {

    }
}
