using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealKit : MonoBehaviour
{
    [Range(0f, 1f)]
    public float healPercent;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<Ship>() != null && !other.CompareTag("Laser"))
        {
            var ship = other.GetComponentInParent<Ship>();
            SoundManager.Instance.PlaySoundOneShot(SoundManager.SoundType.shipHeal, false);
            ship.Heal(healPercent * ship.shipData.maxHealth);
            gameObject.SetActive(false);
        }
    }
}
