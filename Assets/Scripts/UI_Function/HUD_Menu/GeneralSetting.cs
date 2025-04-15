using Script.Input_System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneralSetting : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown fullScreenDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private TMP_Dropdown languageDropdown;

    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider sfxSlider;

    [SerializeField] private Button saveButton;

    [SerializeField] CanvasGroup _saveGroup;
    [SerializeField] Button _saveButton;
    [SerializeField] Button _cancelButton;

    [SerializeField] private Button defaultButton;

    private MenuManager menuManager;

    void Start()
    {
        // Tải cài đặt đã lưu và áp dụng
        LoadSettings();
        ApplyDisplaySettings();

        // Thêm sự kiện lắng nghe khi người dùng thay đổi lựa chọn
        resolutionDropdown.onValueChanged.AddListener(delegate { ApplyDisplaySettings(); });
        fullScreenDropdown.onValueChanged.AddListener(delegate { ApplyDisplaySettings(); });

        // Thêm sự kiện cho các nút
        saveButton.onClick.AddListener(ShowNoticeSaveSettings);
        _saveButton.onClick.AddListener(SaveAndHide);
        _cancelButton.onClick.AddListener(HideNoticeSaveSettings);
        defaultButton.onClick.AddListener(SetDefaultSettings);

        // Ban đầu ẩn _saveGroup
        _saveGroup.alpha = 0;
        _saveGroup.interactable = false;
        _saveGroup.blocksRaycasts = false;
    }

    // Tải cài đặt từ PlayerPrefs
    void LoadSettings()
    {
        if (PlayerPrefs.HasKey("resolutionIndex"))
        {
            int index = PlayerPrefs.GetInt("resolutionIndex");
            resolutionDropdown.value = index;
        }
        else
        {
            resolutionDropdown.value = 2; // Mặc định là 1920 x 1080
        }

        if (PlayerPrefs.HasKey("fullScreenIndex"))
        {
            int index = PlayerPrefs.GetInt("fullScreenIndex");
            fullScreenDropdown.value = index;
        }
        else
        {
            fullScreenDropdown.value = 0; // Mặc định là Fullscreen
        }
    }

    // Áp dụng cài đặt hiển thị (không lưu)
    void ApplyDisplaySettings()
    {
        int resolutionIndex = resolutionDropdown.value;
        int fullScreenIndex = fullScreenDropdown.value;

        FullScreenMode mode = fullScreenIndex == 0 ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;

        switch (resolutionIndex)
        {
            case 0: Screen.SetResolution(3840, 2160, mode); break;
            case 1: Screen.SetResolution(2560, 1440, mode); break;
            case 2: Screen.SetResolution(1920, 1080, mode); break;
            case 3: Screen.SetResolution(1366, 768, mode); break;
            case 4: Screen.SetResolution(1280, 720, mode); break;
            case 5: Screen.SetResolution(800, 600, mode); break;
        }
    }

    // Hiển thị hộp thoại lưu cài đặt
    void ShowNoticeSaveSettings()
    {
        StartCoroutine(FadeIn(_saveGroup, 0.15f));
    }

    // Ẩn hộp thoại lưu cài đặt
    void HideNoticeSaveSettings()
    {
        StartCoroutine(FadeOut(_saveGroup, 0.15f));
    }

    // Lưu cài đặt và ẩn hộp thoại
    void SaveAndHide()
    {
        SaveSettings();
        HideNoticeSaveSettings();
    }

    // Lưu cài đặt vào PlayerPrefs
    void SaveSettings()
    {
        int resolutionIndex = resolutionDropdown.value;
        int fullScreenIndex = fullScreenDropdown.value;

        PlayerPrefs.SetInt("resolutionIndex", resolutionIndex);
        PlayerPrefs.SetInt("fullScreenIndex", fullScreenIndex);
        PlayerPrefs.Save();

        Debug.Log("Cài đặt đã được lưu!");
    }

    // Đặt về cài đặt mặc định
    void SetDefaultSettings()
    {
        resolutionDropdown.value = 2; // 1920 x 1080
        fullScreenDropdown.value = 0; // Fullscreen
        ApplyDisplaySettings();
    }

    // Coroutine để hiện dần CanvasGroup
    IEnumerator FadeIn(CanvasGroup group, float duration)
    {
        float startAlpha = group.alpha;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            group.alpha = Mathf.Lerp(startAlpha, 1, time / duration);
            yield return null;
        }

        group.alpha = 1;
        group.interactable = true;
        group.blocksRaycasts = true;
    }

    // Coroutine để ẩn dần CanvasGroup
    IEnumerator FadeOut(CanvasGroup group, float duration)
    {
        float startAlpha = group.alpha;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            group.alpha = Mathf.Lerp(startAlpha, 0, time / duration);
            yield return null;
        }

        group.alpha = 0;
        group.interactable = false;
        group.blocksRaycasts = false;
    }

    void Update()
    {

    }
}