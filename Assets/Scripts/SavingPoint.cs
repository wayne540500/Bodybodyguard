using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavingPoint : MonoBehaviour
{
    public Transform ship;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform != ship)
            return;

        GameSaveLoadManager.Instance.SaveData();
        gameObject.SetActive(false);
    }
}
