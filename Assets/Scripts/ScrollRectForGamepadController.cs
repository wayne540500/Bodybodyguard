using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ScrollRectForGamepadController : MonoBehaviour
{
    public Dropdown dropdown;
    public int itemShowNum;
    private RectTransform contentPanel;
    private float itemHeight;
    private float selectedY;
    private GameObject lastSelected;
    private float topPosition;
    private float bottomPosition;
    private float delta;

    void Start()
    {
        contentPanel = GetComponent<RectTransform>();
        itemHeight = contentPanel.transform.GetChild(0).GetComponent<RectTransform>().rect.height;
        delta = -itemHeight;
        for (int i = 0; i < dropdown.value - itemShowNum + 1; i++)
        {
            contentPanel.anchoredPosition += new Vector2(0f, itemHeight);
        }
        topPosition = contentPanel.transform.GetChild(dropdown.value - itemShowNum + 2).GetComponent<RectTransform>().anchoredPosition.y;
        bottomPosition = contentPanel.transform.GetChild(dropdown.value + 1).GetComponent<RectTransform>().anchoredPosition.y;

    }

    void Update()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;

        if (selected == null)
        {
            return;
        }

        if (selected.transform.parent != contentPanel.transform)
        {
            return;
        }

        if (selected == lastSelected)
        {
            return;
        }

        selectedY = selected.GetComponent<RectTransform>().anchoredPosition.y;

        if (selectedY > topPosition)
        {
            contentPanel.anchoredPosition -= new Vector2(0f, itemHeight);
            topPosition = selectedY;
            bottomPosition = topPosition + (itemShowNum - 1) * delta;
        }
        if (selectedY < bottomPosition)
        {
            contentPanel.anchoredPosition += new Vector2(0f, itemHeight);
            bottomPosition = selectedY;
            topPosition = bottomPosition - (itemShowNum - 1) * delta;
        }



        lastSelected = selected;
    }

}
