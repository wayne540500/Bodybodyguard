using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public int currentLevel_id;
    public int currentState_id;
    //public int HistoryLength;
    int currentState_id_temp;
    public LevelInfo levelInfo;

    void Start()
    {
        currentState_id = GameDataManager.stateDatas.Current_State_id;
        currentLevel_id = GameDataManager.stateDatas.Current_Level_id;
        //currentState_id_temp用來避免state重複呼叫
        currentState_id_temp = currentState_id - 1;
    }

    void Update()
    {
        StateShow();
        //HistoryLength = GameDataManager.stateDatas.LevelAndStateHistory.Count;
    }

    private void StateShow()
    {
        currentState_id = GameDataManager.stateDatas.Current_State_id;
        currentLevel_id = GameDataManager.stateDatas.Current_Level_id;

        if (levelInfo == null)
            return;

        if (currentLevel_id != levelInfo.Level_id)
        {
            levelInfo.Level.SetActive(false);
            return;
        }
        
        levelInfo.Level.SetActive(true);

        if (currentState_id_temp == currentState_id)
            return;


        currentState_id_temp = currentState_id;

        for (int i = 0; i < levelInfo.stateInfo.Length; i++)
        {
            if (levelInfo.stateInfo[i].State_id == currentState_id_temp)
                levelInfo.stateInfo[i].State.SetActive(true);
            else
                levelInfo.stateInfo[i].State.SetActive(false);
        }

    }
    #region 在Editor自動KeyInStateId
    private void OnValidate()
    {
        AutoKeyInStateId();
    }

    private void AutoKeyInStateId()
    {
        if (levelInfo == null)
            return;

        for (int i = 0; i < levelInfo.stateInfo.Length; i++)
        {
            levelInfo.stateInfo[i].State_id = i;
        }
    }
    #endregion
}

