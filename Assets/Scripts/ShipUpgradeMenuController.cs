using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class ShipUpgradeMenuController : MonoBehaviour
{
    public Ship ship;
    public Animator animator;
    public Button[] buttons;//上左中右下

    private MultiplayerEventSystem P1_EventSystem;

    private void Awake()
    {
        if (ship == null)
            ship = GameObject.FindGameObjectWithTag("Ship").GetComponent<Ship>();

        // for (int i = 0; i < buttons.Length; i++)
        // {
        //     buttons[i].onClick.AddListener(delegate { OnUpgradeButtonClick(i); });
        // }

        // buttons[0].onClick.AddListener(delegate { OnUpgradeButtonClick(0); });
        // buttons[1].onClick.AddListener(delegate { OnUpgradeButtonClick(1); });
        // buttons[2].onClick.AddListener(delegate { OnUpgradeButtonClick(2); });
        // buttons[3].onClick.AddListener(delegate { OnUpgradeButtonClick(3); });
        // buttons[4].onClick.AddListener(delegate { OnUpgradeButtonClick(4); });

        for (int i = 0; i < buttons.Length; i++)
        {
            int i2 = i;
            buttons[i].onClick.AddListener(delegate { OnUpgradeButtonClick(i2); });
        }

    }

    private void Start()
    {
        P1_EventSystem = ship.P1_EventSystem;
    }

    private void Update()
    {
        for (int i = 0; i < ship.shipData.ShipPartLevel.Length; i++)
        {
            if (ship.shipData.ShipPartLevel[i] >= 2)
            {
                buttons[i].interactable = false;
            }
        }
    }

    public void SetSelectButtons()
    {
        P1_EventSystem.firstSelectedGameObject = buttons[0].gameObject;
        P1_EventSystem.playerRoot = buttons[0].transform.parent.gameObject;
        P1_EventSystem.UpdateModules();
        P1_EventSystem.SetSelectedGameObject(buttons[0].gameObject);
    }

    public void OnUpgradeButtonClick(int i)
    {
        Debug.Log(ship.shipData.ShipPartLevel.Length);
        Debug.Log("i = " + i);
        ship.shipData.ShipPartLevel[i]++;
        ship.shipData.wrenchNumber -= (15 + ship.shipData.upgradeTimes * 10);
        ship.shipData.upgradeTimes++;
        GameSaveLoadManager.Instance.SaveDataWithOutPosition();
        CameraController.Instance.GetMainCamera().Priority = 10;
        animator.SetBool("isOpen", false);
        ship.isFirstPlaySound = true;
    }

    public void SetTimeScale(float scale)
    {
        transform.position = ship.transform.position;
        Time.timeScale = scale;
    }
}
