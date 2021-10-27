using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBoostController : MonoBehaviour
{
    public Rigidbody2D shipRbody;
    public ShipController shipController;
    public Transform[] boosters;

    private void Update()
    {
        if (shipController.GetMoveInput() == Vector2.zero)
        {
            for (int i = 0; i < 4; i++)
            {
                boosters[i].GetChild(0).gameObject.SetActive(false);
            }
            return;
        }

        var x = shipController.GetMoveInput().x;
        var y = shipController.GetMoveInput().y;

        if (x < 0)
        {
            if (y < 0)
            {
                boosters[0].GetChild(0).gameObject.SetActive(false);
                boosters[1].GetChild(0).gameObject.SetActive(true);
                boosters[1].up = shipController.GetMoveInput().normalized;
                boosters[2].GetChild(0).gameObject.SetActive(false);
                boosters[3].GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                boosters[0].GetChild(0).gameObject.SetActive(false);
                boosters[1].GetChild(0).gameObject.SetActive(false);
                boosters[2].GetChild(0).gameObject.SetActive(false);
                boosters[3].GetChild(0).gameObject.SetActive(true);
                boosters[3].up = shipController.GetMoveInput().normalized;
            }
        }
        else
        {
            if (y < 0)
            {
                boosters[0].GetChild(0).gameObject.SetActive(true);
                boosters[0].up = shipController.GetMoveInput().normalized;
                boosters[1].GetChild(0).gameObject.SetActive(false);
                boosters[2].GetChild(0).gameObject.SetActive(false);
                boosters[3].GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                boosters[0].GetChild(0).gameObject.SetActive(false);
                boosters[1].GetChild(0).gameObject.SetActive(false);
                boosters[2].GetChild(0).gameObject.SetActive(true);
                boosters[2].up = shipController.GetMoveInput().normalized;
                boosters[3].GetChild(0).gameObject.SetActive(false);
            }
        }

    }

}
