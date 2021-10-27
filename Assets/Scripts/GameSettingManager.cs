using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System.IO;

public class GameSettingManager : MonoBehaviour
{
    public Toggle fullscreenToggle;
    public Dropdown resolutionDropdown;
    public Dropdown qualityDropdown;
    public Slider musicVolumeSlider;
    public Slider soundEffectVolumeSlider;
    public AudioMixer audioMixer;
    public Button applyButton;

    private Resolution[] resolutions;
    private GameSettings gameSettings;
    private TextMeshProUGUI musicPercentText;
    private TextMeshProUGUI soundEffectPercentText;

    private void OnEnable()
    {
        gameSettings = new GameSettings();

        fullscreenToggle.onValueChanged.AddListener(delegate { OnFullscreenToggle(); });
        resolutionDropdown.onValueChanged.AddListener(delegate { OnResolutionChanged(); });
        qualityDropdown.onValueChanged.AddListener(delegate { OnQualityChanged(); });
        musicVolumeSlider.onValueChanged.AddListener(delegate { OnMusicVolumeChanged(); });
        soundEffectVolumeSlider.onValueChanged.AddListener(delegate { OnSoundEffectVolumeChanged(); });
        applyButton.onClick.AddListener(delegate { OnApplyButtonClick(); });

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        foreach (var resolution in resolutions)
        {
            resolutionDropdown.options.Add(new Dropdown.OptionData(resolution.width + " x " + resolution.height));
            if (Screen.currentResolution.width == resolution.width && Screen.currentResolution.height == resolution.height)
            {
                resolutionDropdown.value = resolutionDropdown.options.Count - 1;
            }
        }

        fullscreenToggle.isOn = Screen.fullScreen;
        musicPercentText = musicVolumeSlider.GetComponentInChildren<TextMeshProUGUI>();
        musicPercentText.SetText($"{Mathf.Round((musicVolumeSlider.value + 80f) / 80f * 100f)}%");
        soundEffectPercentText = soundEffectVolumeSlider.GetComponentInChildren<TextMeshProUGUI>();
        soundEffectPercentText.SetText($"{Mathf.Round((soundEffectVolumeSlider.value + 80f) / 80f * 100f)}%");

        if (File.Exists(Application.persistentDataPath + "/GameSetting.json"))
            LoadSettings();

    }

    public void OnFullscreenToggle()
    {
        applyButton.interactable = true;
        gameSettings.Fullscreen = fullscreenToggle.isOn;
    }

    public void OnResolutionChanged()
    {
        applyButton.interactable = true;
        gameSettings.ResolutionIndex = resolutionDropdown.value;
    }
    public void OnQualityChanged()
    {
        applyButton.interactable = true;
        gameSettings.QualityIndex = qualityDropdown.value;
        QualitySettings.SetQualityLevel(gameSettings.QualityIndex);
    }
    public void OnMusicVolumeChanged()
    {
        applyButton.interactable = true;
        var volume = musicVolumeSlider.value;
        gameSettings.MusicVolume = volume;
        audioMixer.SetFloat("_musicVolume", volume);
        musicPercentText.SetText($"{Mathf.Round((musicVolumeSlider.value + 80f) / 80f * 100f)}%");
    }
    public void OnSoundEffectVolumeChanged()
    {
        applyButton.interactable = true;
        var volume = soundEffectVolumeSlider.value;
        gameSettings.SoundEffectVolume = volume;
        audioMixer.SetFloat("_soundEffectVolume", volume);
        soundEffectPercentText.SetText($"{Mathf.Round((soundEffectVolumeSlider.value + 80f) / 80f * 100f)}%");
    }

    public void OnApplyButtonClick()
    {
        SaveSettings();
    }

    public void SaveSettings()
    {
        Screen.fullScreen = gameSettings.Fullscreen;
        Screen.SetResolution(resolutions[gameSettings.ResolutionIndex].width, resolutions[gameSettings.ResolutionIndex].height, gameSettings.Fullscreen);
        string jsonData = JsonUtility.ToJson(gameSettings, true);
        File.WriteAllText(Application.persistentDataPath + "/GameSetting.json", jsonData);
        applyButton.interactable = false;
    }

    public void LoadSettings()
    {
        gameSettings = JsonUtility.FromJson<GameSettings>(File.ReadAllText(Application.persistentDataPath + "/GameSetting.json"));
        musicVolumeSlider.value = gameSettings.MusicVolume;
        soundEffectVolumeSlider.value = gameSettings.SoundEffectVolume;
        musicPercentText.SetText($"{Mathf.Round((musicVolumeSlider.value + 80f) / 80f * 100f)}%");
        soundEffectPercentText.SetText($"{Mathf.Round((soundEffectVolumeSlider.value + 80f) / 80f * 100f)}%");
        qualityDropdown.value = gameSettings.QualityIndex;
        resolutionDropdown.value = gameSettings.ResolutionIndex;
        fullscreenToggle.isOn = gameSettings.Fullscreen;
        resolutionDropdown.RefreshShownValue();
        qualityDropdown.RefreshShownValue();

    }
}
