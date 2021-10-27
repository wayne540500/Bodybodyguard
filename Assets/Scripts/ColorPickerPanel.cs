using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPickerPanel : MonoBehaviour
{
    public int playerIndex;
    public int partColorIndex;
    public ColorPartPickerPanel colorPartPickerPanel;
    public Image previewColor;
    public Slider h_Slider;
    public Slider s_Slider;
    public Slider v_Slider;
    public Button backBT;

    private Image s_SliderImage;
    private Image v_SliderImage;
    private Color color;
    private float h, s, v;

    private void Awake()
    {
        s_SliderImage = s_Slider.transform.GetChild(1).GetComponent<Image>();
        v_SliderImage = v_Slider.transform.GetChild(0).GetComponent<Image>();

        backBT.onClick.AddListener(delegate { OnBackBTClicked(); });
        h_Slider.onValueChanged.AddListener(delegate { OnSliderValueChanged(); });
        s_Slider.onValueChanged.AddListener(delegate { OnSliderValueChanged(); });
        v_Slider.onValueChanged.AddListener(delegate { OnSliderValueChanged(); });

        // h_Slider.maxValue = 360f;
        // s_Slider.maxValue = 100f;
        // v_Slider.maxValue = 100f;

        // h_Slider.minValue = 0f;
        // s_Slider.minValue = 0f;
        // v_Slider.minValue = 0f;
    }
    private void OnEnable()
    {
        color = GameDataManager.playerDatas[playerIndex].colors[partColorIndex];
        Color.RGBToHSV(color, out h, out s, out v);
        SetSlidersValue(h, s, v);
        previewColor.color = color;
        s_SliderImage.color = Color.HSVToRGB(h, 1f, 1f);
        v_SliderImage.color = Color.HSVToRGB(h, 1f, 1f);


    }

    private void Update()
    {
        RefreshThePanel();
    }

    private void RefreshThePanel()
    {
        previewColor.color = color;
        GameDataManager.playerDatas[playerIndex].colors[partColorIndex] = color;
        s_SliderImage.color = Color.HSVToRGB(h, 1f, 1f);
        v_SliderImage.color = Color.HSVToRGB(h, s, 1f);;
    }

    public void OnSliderValueChanged()
    {
        SetHsvValue();
        color = Color.HSVToRGB(h, s, v);
    }

    private void SetSlidersValue(float h, float s, float v)
    {
        h_Slider.value = h * h_Slider.maxValue;
        s_Slider.value = s * s_Slider.maxValue;
        v_Slider.value = v * v_Slider.maxValue;
    }

    private void SetHsvValue()
    {
        h = h_Slider.value / h_Slider.maxValue;
        s = s_Slider.value / s_Slider.maxValue;
        v = v_Slider.value / v_Slider.maxValue;
    }

    public void OnBackBTClicked()
    {
        colorPartPickerPanel.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

}
