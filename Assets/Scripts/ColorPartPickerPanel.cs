using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPartPickerPanel : MonoBehaviour
{
    public int playerIndex;
    public ColorPickerPanel colorPickerPanel;
    public ReadyCountDownController readyCountDownController;
    public Button[] partBTs;
    public Button randomColorBT;
    public Toggle isReady;

    private Image[] partBTImages;
    private ChooseColorPlayerController chooseColorPlayerControllers;

    private void Awake()
    {
        List<GameObject> players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));

        foreach (var player in players)
        {
            if (player.activeSelf && player.GetComponent<ChooseColorPlayerController>().GetPlayerIndex() == playerIndex)
                chooseColorPlayerControllers = player.GetComponent<ChooseColorPlayerController>();
        }

        if (partBTs.Length == 0)
            return;

        colorPickerPanel.playerIndex = playerIndex;

        partBTImages = new Image[partBTs.Length];

        for (int i = 0; i < partBTImages.Length; i++)
        {
            partBTImages[i] = partBTs[i].GetComponent<Image>();
        }

        partBTs[0].onClick.AddListener(delegate { OnPartBTClicked(0); });
        partBTs[1].onClick.AddListener(delegate { OnPartBTClicked(1); });
        partBTs[2].onClick.AddListener(delegate { OnPartBTClicked(2); });
        partBTs[3].onClick.AddListener(delegate { OnPartBTClicked(3); });
        partBTs[4].onClick.AddListener(delegate { OnPartBTClicked(4); });
        randomColorBT.onClick.AddListener(() => OnRandomBTClicked());
    }

    private void Update()
    {
        RefreshThePanel();
    }

    private void RefreshThePanel()
    {
        for (int i = 0; i < partBTImages.Length; i++)
        {
            partBTImages[i].color = GameDataManager.playerDatas[playerIndex].colors[i];
        }

        readyCountDownController.isReady[playerIndex] = isReady.isOn;

        foreach (var partBT in partBTs)
        {
            partBT.interactable = isReady.isOn ? false : true;
        }

        randomColorBT.interactable = isReady.isOn ? false : true;
    }

    public void OnPartBTClicked(int partIndex)
    {
        chooseColorPlayerControllers.lastPartIndex = partIndex;
        colorPickerPanel.partColorIndex = partIndex;
        colorPickerPanel.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public void OnRandomBTClicked()
    {
        for (int i = 0; i < partBTImages.Length; i++)
        {
            Color color = new Color(
            Random.Range(0, 51) * 0.02f,
            Random.Range(0, 51) * 0.02f,
            Random.Range(0, 51) * 0.02f
            );

            GameDataManager.playerDatas[playerIndex].colors[i] = color;
        }
    }
}
