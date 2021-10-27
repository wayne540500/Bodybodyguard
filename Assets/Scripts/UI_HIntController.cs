using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_HIntController : MonoBehaviour
{
    public GameObject mapPanel;
    public GameObject hintPanel;

    private void Update()
    {
        hintPanel.SetActive(mapPanel.activeSelf ? false : true);
    }
}
