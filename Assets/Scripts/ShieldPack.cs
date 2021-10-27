using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPack : MonoBehaviour
{
    public bool isPhoenix;
    [Range(0f, 1f)]
    public float healPercent;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<Ship>() != null && !other.CompareTag("Laser"))
        {
            var ship = other.GetComponentInParent<Ship>();
            SoundManager.Instance.PlaySoundOneShot(SoundManager.SoundType.shieldHeal, false);
            ship.ShieldHeal(healPercent * ship.sheildData.maxShieldHP);
            if (isPhoenix)
            {
                SoundManager.Instance.PlaySoundOneShot(SoundManager.SoundType.phoenixMode, false);
                if (ship.sheildData.shieldSprite.transform.parent.TryGetComponent<ShieldController>(out var shield))
                {
                    shield.SetShieldInvincible();
                }
            }
            gameObject.SetActive(false);
        }
    }
}
