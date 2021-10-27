using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wrench : MonoBehaviour
{
    public int wrenchNumber;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<Ship>() != null && !other.CompareTag("Laser"))
        {
            var ship = other.GetComponentInParent<Ship>();
            SoundManager.Instance.PlaySoundOneShot(SoundManager.SoundType.WrenchPickUp, false);
            ship.GetWrench(wrenchNumber);
            gameObject.SetActive(false);
        }
    }


}
