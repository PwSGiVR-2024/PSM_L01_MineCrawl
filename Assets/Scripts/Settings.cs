using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    [SerializeField] private Slider musicVolumeSlider;
    private MusicClass musicClass;
    private Resolution[] availableResolutions;
    [SerializeField] private GameObject settingsPanel;  // Panel ustawieñ
    [SerializeField] private GameObject mainMenuPanel;  // Panel menu g³ównego
    [SerializeField] private Button backButton;         // Przycisk „Wróæ”

    private const string ResolutionIndexKey = "ResolutionIndex";
    private const string FullscreenKey = "Fullscreen";

    void Start()
    {
        if (backButton != null)
            backButton.onClick.AddListener(ReturnToMainMenu);
        musicClass = GameObject.FindGameObjectWithTag("Music").GetComponent<MusicClass>();
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        // Rozdzielczoœci ekranu
        availableResolutions = Screen.resolutions;
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < availableResolutions.Length; i++)
        {
            string option = $"{availableResolutions[i].width} x {availableResolutions[i].height} @ {availableResolutions[i].refreshRate}Hz";
            options.Add(option);

            if (availableResolutions[i].width == Screen.currentResolution.width &&
                availableResolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);

        int savedResolutionIndex = PlayerPrefs.GetInt(ResolutionIndexKey, currentResolutionIndex);
        resolutionDropdown.value = savedResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);

        // Tryb pe³noekranowy
        bool isFullscreen = PlayerPrefs.GetInt(FullscreenKey, Screen.fullScreen ? 1 : 0) == 1;
        fullscreenToggle.isOn = isFullscreen;
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggle);

        // Zastosuj ustawienia na start
        ApplySettings();
    }
    private void OnMusicVolumeChanged(float value)
    {
        musicClass.SetVolume(value);
    }
    private void OnResolutionChanged(int index)
    {
        PlayerPrefs.SetInt(ResolutionIndexKey, index);
        ApplyResolution(index);
    }

    private void OnFullscreenToggle(bool isFullscreen)
    {
        PlayerPrefs.SetInt(FullscreenKey, isFullscreen ? 1 : 0);
        Screen.fullScreen = isFullscreen;
        Debug.Log($"Pe³ny ekran: {isFullscreen}");
    }

    private void ApplyResolution(int index)
    {
        Resolution resolution = availableResolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen, resolution.refreshRate);
        Debug.Log($"Ustawiono rozdzielczoœæ: {resolution.width}x{resolution.height} @ {resolution.refreshRate}Hz");
    }

    private void ApplySettings()
    {
        // Zastosuj zapisane ustawienia po starcie
        int resIndex = PlayerPrefs.GetInt(ResolutionIndexKey, 0);
        bool isFullscreen = PlayerPrefs.GetInt(FullscreenKey, 1) == 1;

        ApplyResolution(resIndex);
        Screen.fullScreen = isFullscreen;
    }
    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    public void ReturnToMainMenu()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}
