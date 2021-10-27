using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class ChooseColorPlayerController : MonoBehaviour
{
    public PauseMenuController pauseMenuController;
    public MultiplayerEventSystem multiplayerEventSystem;
    public PlayerInput playerInput;
    public GameObject[] pressToJoinPanels;
    public GameObject[] partPickerPanels;
    public GameObject[] colorPickerPanels;
    public float startPosition;
    public float positionMultiDelta;
    public int lastPartIndex;
    public string[] controllers;
    public SpriteRenderer controllerSpriteRenderer;
    public Sprite[] controllersSprites;

    private int playerIndex;
    private bool isColorPickerPanelOpened_temp;
    private bool isActived;
    private bool pauseMenuOpenTemp;
    [SerializeField]private string deviceName;

    private void Awake()
    {
        multiplayerEventSystem = GetComponentInChildren<MultiplayerEventSystem>();
        playerInput = GetComponent<PlayerInput>();
        playerIndex = playerInput.playerIndex;
        multiplayerEventSystem.playerRoot = partPickerPanels[playerIndex];
        multiplayerEventSystem.firstSelectedGameObject = partPickerPanels[playerIndex].transform.GetChild(0).GetChild(0).GetChild(1).gameObject;
    }
    private void OnEnable()
    {
        transform.position = new Vector3(startPosition + positionMultiDelta * playerInput.playerIndex, -3.05f, 0f);
    }
    private void Start()
    {
        pressToJoinPanels[playerIndex].SetActive(false);
        partPickerPanels[playerIndex].SetActive(true);
        colorPickerPanels[playerIndex].SetActive(false);
        isColorPickerPanelOpened_temp = false;
        deviceName = playerInput.devices[0].name;
        for (int i = 0; i < controllers.Length; i++)
        {
            if(controllers[i].Contains(deviceName)){
                controllerSpriteRenderer.sprite = controllersSprites[i];
            }
        }
    }

    private void Update()
    {
        isActived = colorPickerPanels[playerIndex].activeInHierarchy; 
        if (isActived == isColorPickerPanelOpened_temp)
            return;

        isColorPickerPanelOpened_temp = isActived;

        CheckWhichPanelActive(isActived);
    }

    private void CheckWhichPanelActive(bool isActived){
        if (isActived)
        {
            multiplayerEventSystem.playerRoot = colorPickerPanels[playerIndex];
            multiplayerEventSystem.firstSelectedGameObject = colorPickerPanels[playerIndex].transform.GetChild(0).GetChild(0).GetChild(1).gameObject;
            multiplayerEventSystem.SetSelectedGameObject(colorPickerPanels[playerIndex].transform.GetChild(0).GetChild(0).GetChild(1).gameObject);
            multiplayerEventSystem.UpdateModules();
        }
        else
        {
            multiplayerEventSystem.playerRoot = partPickerPanels[playerIndex];
            multiplayerEventSystem.firstSelectedGameObject = partPickerPanels[playerIndex].transform.GetChild(0).GetChild(lastPartIndex).GetChild(1).gameObject;
            multiplayerEventSystem.SetSelectedGameObject(partPickerPanels[playerIndex].transform.GetChild(0).GetChild(lastPartIndex).GetChild(1).gameObject);
            multiplayerEventSystem.UpdateModules();
        }
    }

    public int GetPlayerIndex(){
        return playerIndex;
    }

    public void Pause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            bool isPaused = pauseMenuController.isPaused;
            if (isPaused){
                pauseMenuController.Resume(playerIndex + 1);
            }
            else
                pauseMenuController.Pause(playerIndex + 1, multiplayerEventSystem);
        }
    }
}
