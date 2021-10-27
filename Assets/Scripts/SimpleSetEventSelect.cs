using UnityEngine;
using UnityEngine.InputSystem.UI;

public class SimpleSetEventSelect : MonoBehaviour
{
    public MultiplayerEventSystem playerEventSystem;
    public GameObject target;

    public void SetSelect()
    {
        playerEventSystem.SetSelectedGameObject(target);
    }
}
