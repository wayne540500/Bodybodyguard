using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReadyCountDownController : MonoBehaviour
{
    public CountDownPanel countDownPanel;
    public TextMeshProUGUI countDownText;
    public bool[] isReady;
    public float countDownSec;
    private int playerCount_Temp;
    private int readyNumberConter = 0;

    private void Awake()
    {
        if (countDownPanel == null)
        {
            countDownPanel = transform.GetChild(0).GetComponent<CountDownPanel>();
            countDownText = countDownPanel.GetComponentInChildren<TextMeshProUGUI>();
        }
        countDownPanel.countDownSec = countDownSec;
        countDownPanel.gameObject.SetActive(false);
        playerCount_Temp = GameDataManager.playerDatas.Count;
    }

    private void OnEnable() {
        GameDataManager.playerDatas = new List<PlayerData>();
        playerCount_Temp = GameDataManager.playerDatas.Count;
    }

    private void Update()
    {
        CheckIsNewPlayerJoin();

        CheckIsAllPlayerReady();


    }

    private void CheckIsNewPlayerJoin()
    {
        if (playerCount_Temp == GameDataManager.playerDatas.Count)
            return;

        playerCount_Temp = GameDataManager.playerDatas.Count;

        Debug.Log(playerCount_Temp);

        if (playerCount_Temp == 0)
            return;

        isReady = new bool[playerCount_Temp];
    }

    private void CheckIsAllPlayerReady()
    {
        readyNumberConter = 0;
        foreach (var toggleCheck in isReady)
        {
            readyNumberConter = toggleCheck ? readyNumberConter + 1 : readyNumberConter;
        }

        if (readyNumberConter == isReady.Length && readyNumberConter != 0)
        {
            if (countDownPanel.gameObject.activeInHierarchy != true)
            {
                countDownPanel.countDownSec = countDownSec;
                countDownPanel.gameObject.SetActive(true);
            }
        }
        else
        {
            if (countDownPanel.gameObject.activeInHierarchy != false)
                countDownPanel.gameObject.SetActive(false);
        }
    }
}
