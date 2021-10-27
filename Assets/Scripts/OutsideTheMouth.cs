using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutsideTheMouth : MonoBehaviour
{
    public Rigidbody2D ship;
    public Ship shipData;
    public float scale;

    private void Update()
    {
        if (ship.velocity.magnitude > shipData.shipData.maxHealth / 10f)
        {
            shipData.SetCurrentHP(0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform != ship.transform)
            return;

        ship.gravityScale = scale;
    }
}
