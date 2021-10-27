using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBodyDamgeController : MonoBehaviour
{
    public GameObject[] sprites;

    private float shipHP;
    private float shipMaxHP;
    private Ship ship;

    private void Start()
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].SetActive(false);
        }
        ship = GetComponentInParent<Ship>();
        shipHP = ship.GetCurrentHP();
        shipMaxHP = ship.shipData.maxHealth;
    }

    private void FixedUpdate()
    {
        shipHP = ship.GetCurrentHP();
        shipMaxHP = ship.shipData.maxHealth;
        var ratio = shipHP / shipMaxHP;

        if (ratio > 0.8f)
            ShowAndOff(-1);
        else if (ratio > 0.6f)
            ShowAndOff(0);
        else if (ratio > 0.4f)
            ShowAndOff(1);
        else if (ratio > 0.2f)
            ShowAndOff(2);
        else if (ratio > 0f)
            ShowAndOff(3);
    }

    private void ShowAndOff(int showIndex)
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            if (i == showIndex)
            {
                sprites[i].SetActive(true);
                continue;
            }

            sprites[i].SetActive(false);
        }
    }
}