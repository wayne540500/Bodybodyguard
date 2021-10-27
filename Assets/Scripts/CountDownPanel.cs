using System.IO;
using UnityEngine;
using TMPro;

public class CountDownPanel : MonoBehaviour
{
    public float countDownSec;
    public TextMeshProUGUI countDownText;
    public StateChange stateChange;
    private float counter;

    private void OnEnable()
    {
        counter = countDownSec;
    }

    private void Update()
    {
        if (counter > 0f)
        {
            countDownText.SetText($"Starting in {(int)counter + 1}...");
            counter -= Time.deltaTime;
            if (counter <= 0f)
            {
                counter = 0f;
                Debug.Log("Game Start!!!");
                if (File.Exists(Application.persistentDataPath + "/Save.json"))
                {
                    var load = gameObject.AddComponent<LoadAndNewGameBT>();
                    load.LoadScene();
                }
                else
                {
                    stateChange.CrossScene();
                }
            }
        }
    }

}
