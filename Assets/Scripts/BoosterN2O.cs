using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterN2O : MonoBehaviour
{
    public ShipController shipController;
    public float duration;

    private void Awake()
    {
        shipController = GameObject.FindGameObjectWithTag("ShipMoveController").GetComponent<ShipController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 8 && !other.gameObject.CompareTag("Laser"))
        {
            SoundManager.Instance.PlaySoundOneShot(SoundManager.SoundType.boostMode, false);
            shipController.SetBoostMode(duration);
            gameObject.SetActive(false);
        }
    }
}
