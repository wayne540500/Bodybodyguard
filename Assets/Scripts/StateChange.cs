using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StateChange : MonoBehaviour
{
    public string nextSceneName;
    public int nextLevel_id;
    public int nextState_id;

    public StateChange(string nextSceneName, int nextLevel_id, int nextState_id)
    {
        this.nextSceneName = nextSceneName;
        this.nextLevel_id = nextLevel_id;
        this.nextState_id = nextState_id;
    }

    int Current_Level_id;
    int Current_State_id;

    List<int> tempList;

    public void ToNextState()
    {
        Current_Level_id = GameDataManager.stateDatas.Current_Level_id;
        Current_State_id = GameDataManager.stateDatas.Current_State_id;

        tempList = new List<int>
        {
            Current_Level_id,
            Current_State_id
        };

        GameDataManager.stateDatas.Current_State_id++;

        GameDataManager.stateDatas.LevelAndStateHistory.Add(tempList);

    }

    public void ToAnyState()
    {
        Current_Level_id = GameDataManager.stateDatas.Current_Level_id;
        Current_State_id = GameDataManager.stateDatas.Current_State_id;

        tempList = new List<int>
        {
            Current_Level_id,
            Current_State_id
        };

        GameDataManager.stateDatas.Current_Level_id = nextLevel_id;
        GameDataManager.stateDatas.Current_State_id = nextState_id;

        GameDataManager.stateDatas.LevelAndStateHistory.Add(tempList);
    }

    public void CrossScene()
    {
        if (string.IsNullOrEmpty(nextSceneName))
            return;

        Time.timeScale = Time.timeScale == 0f ? 1f : Time.timeScale;

        GameDataManager.stateDatas.Current_Level_id = nextLevel_id;
        GameDataManager.stateDatas.Current_State_id = nextState_id;

        GameDataManager.nextSceneName = nextSceneName;

        SceneManager.LoadScene("LoadingScene", LoadSceneMode.Single);//把loadingScene加進來
    }
    public void CrossScene_Direct()
    {
        if (string.IsNullOrEmpty(nextSceneName))
            return;

        Time.timeScale = Time.timeScale == 0f ? 1f : Time.timeScale;

        SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
    }

    public void ToPreviousState()
    {
        if (GameDataManager.stateDatas.LevelAndStateHistory.Count == 0)
            return;

        int lastindex = GameDataManager.stateDatas.LevelAndStateHistory.Count - 1;

        GameDataManager.stateDatas.Current_Level_id = GameDataManager.stateDatas.LevelAndStateHistory[lastindex][0];
        GameDataManager.stateDatas.Current_State_id = GameDataManager.stateDatas.LevelAndStateHistory[lastindex][1];
        GameDataManager.stateDatas.LevelAndStateHistory.RemoveAt(lastindex);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }


}