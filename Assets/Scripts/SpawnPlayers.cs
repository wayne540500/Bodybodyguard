using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform[] playerPositions;

    private bool isFound;
    private List<GameObject> players;
    private Ship ship;

    private void Awake()
    {
        ship = GameObject.FindGameObjectWithTag("Ship").GetComponent<Ship>();
    }

    private void OnEnable() {
        Spawn();
    }

    public void Spawn()
    {
        if (GameDataManager.playerDatas.Count == 0)
            return;

        Debug.Log("player number: " + GameDataManager.playerDatas.Count);
        players = new List<GameObject>();

        for (int i = 0; i < 4; i++)
        {
            GameObject player = Instantiate(playerPrefab, transform);
            players.Add(player);

        }

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetComponent<PlayerInput>().devices.Count == 0)
            {
                Debug.Log("destroy player with no user");
                Destroy(players[i]);
                players.Remove(players[i]);
                continue;
            }
            var playerDeviceId = players[i].GetComponent<PlayerInput>().devices[0].deviceId;
            var playerDeviceName = players[i].GetComponent<PlayerInput>().devices[0].name;
            Debug.Log("player " + i + " " + playerDeviceId + " " + playerDeviceName);
            isFound = false;
            for (int j = 0; j < GameDataManager.playerDatas.Count; j++)
            {
                var id_temp = GameDataManager.playerDatas[j].deviceId;
                var deviceName_temp = GameDataManager.playerDatas[j].deviceName;
                if (playerDeviceId == id_temp && playerDeviceName == deviceName_temp)
                {
                    if (j == 0)
                        ship.P1_EventSystem = players[i].GetComponentInChildren<MultiplayerEventSystem>();
                    players[i].GetComponent<PlayerController>().playerIndex = j;
                    players[i].transform.position = playerPositions[j].position;
                    isFound = true;
                    break;
                }
            }
            Debug.Log(isFound, players[i]);
            if (!isFound)
            {
                Debug.Log("destroy player with no user/2");
                Destroy(players[i]);
                players.Remove(players[i]);
            }

        }

    }

    public void ResetPlayersPosition()
    {
        foreach (var player in players)
        {
            var playerController = player.GetComponent<PlayerController>();
            switch (playerController.OnWhichController)
            {
                case 0:
                    player.transform.position = playerPositions[playerController.playerIndex].position;
                    break;
                case 1:
                    playerController.ExitShipMoveController();
                    player.transform.position = playerPositions[playerController.playerIndex].position;
                    break;
                case 2:
                    playerController.ExitShooterController();
                    player.transform.position = playerPositions[playerController.playerIndex].position;
                    break;

            }
        }
    }
}
