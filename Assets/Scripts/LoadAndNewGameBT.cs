using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LoadAndNewGameBT : MonoBehaviour
{
    public bool isYesNoBT;
    public bool isLoadBT;
    public GameObject yesNoPanel;
    public Button button;

    private void Awake()
    {
        if (isYesNoBT)
            return;

        button = GetComponent<Button>();
        if (!isLoadBT)
            button.onClick.AddListener(delegate { yesNoPanel.SetActive(true); });

    }

    private void Update()
    {
        if (isYesNoBT)
            return;

        if (isLoadBT)
        {
            if (!File.Exists(Application.persistentDataPath + "/Save.json"))
                button.interactable = false;
            else
                button.interactable = true;
        }
    }

    public void RemoveSaveFile()
    {
        File.Delete(Application.persistentDataPath + "/Save.json");
    }

    public void LoadScene()
    {
        var nextSceneName = JsonUtility.FromJson<GameSaveData>(File.ReadAllText(Application.persistentDataPath + "/Save.json")).LevelName;
        var stateChange = new StateChange(nextSceneName, 0, 0);
        stateChange.CrossScene();
    }

    public void CloseYesNoPanel()
    {
        yesNoPanel.SetActive(false);
    }
}
