using Script.Input_System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GeneralSetting : MonoBehaviour
{
    #region Variables & Properties
    [Header("UI Elements")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown fullScreenDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private TMP_Dropdown languageDropdown;

    [Header("Audio Elements")]
    [SerializeField] private AudioMixer mixerAudio;
    
    public Slider volumeSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Button Elements")]
    [SerializeField] private Button saveButtonGroup;

    public CanvasGroup _saveGroup;
    [SerializeField] Button _saveButton;
    [SerializeField] Button _cancelButton;

    [SerializeField] private Button defaultButtonGroup;

    public CanvasGroup _defaultGroup;
    [SerializeField] private Button _saveDefaultButton;
    [SerializeField] private Button _cancelDefaultButton;

    [Header("Audio Elements")]
    [SerializeField] private AudioSource okSound;
    [SerializeField] private AudioSource cancelSound;
    
    private MenuManager menuManager;
    #endregion

    #region Setup & Initialization
    void Start()
    {
        // Tải cài đặt đã lưu và áp dụng
        LoadSettings();

        ApplyDisplaySettings();
        ApplyQualitySettings();
        ApplyAudioSettings();
        ApplyLanguageSettings();

        // Thêm sự kiện lắng nghe khi người dùng thay đổi lựa chọn
        resolutionDropdown.onValueChanged.AddListener(delegate { ApplyDisplaySettings(); });
        fullScreenDropdown.onValueChanged.AddListener(delegate { ApplyDisplaySettings(); });
        qualityDropdown.onValueChanged.AddListener(delegate { ApplyQualitySettings(); });
        languageDropdown.onValueChanged.AddListener(delegate { ApplyLanguageSettings(); });
        
        // Thêm sự kiện lắng nghe cho từng slider âm thanh
        volumeSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        // Thêm sự kiện cho các nút
        saveButtonGroup.onClick.AddListener(ShowNoticeSaveSettings);
        _saveButton.onClick.AddListener(SaveAndHide);
        _cancelButton.onClick.AddListener(HideNoticeSaveSettings);

        defaultButtonGroup.onClick.AddListener(ShowNoticeDefaultSettings);
        _saveDefaultButton.onClick.AddListener(DefaultAndHide);
        _cancelDefaultButton.onClick.AddListener(HideNoticeDefaultSettings);

        // Ban đầu ẩn _saveGroup
        _saveGroup.alpha = 0;
        _saveGroup.interactable = false;
        _saveGroup.blocksRaycasts = false;

        // Ẩn _defaultGroup
        _defaultGroup.alpha = 0;
        _defaultGroup.interactable = false;
        _defaultGroup.blocksRaycasts = false;
    }

    // Tải cài đặt từ PlayerPrefs
    void LoadSettings()
    {
        // load độ phân giải
        if (PlayerPrefs.HasKey("resolutionIndex"))
        {
            int index = PlayerPrefs.GetInt("resolutionIndex");
            resolutionDropdown.value = index;
        }
        else
        {
            resolutionDropdown.value = 2; // Mặc định là 1920 x 1080
        }


        // load fullscreen
        if (PlayerPrefs.HasKey("fullScreenIndex"))
        {
            int index = PlayerPrefs.GetInt("fullScreenIndex");
            fullScreenDropdown.value = index;
        }
        else
        {
            fullScreenDropdown.value = 0; // Mặc định là Fullscreen
        }


        // load chất lượng đồ hoạ
        if (PlayerPrefs.HasKey("qualityIndex"))
        {
            int index = PlayerPrefs.GetInt("qualityIndex");
            qualityDropdown.value = index;
        }
        else
        {
            qualityDropdown.value = 2; // Mặc định là Medium
        }


        // load ngôn ngữ
        if (PlayerPrefs.HasKey("languageIndex"))
        {
            int index = PlayerPrefs.GetInt("languageIndex");
            languageDropdown.value = index;
        }
        else
        {
            languageDropdown.value = 0; // Mặc định là Tiếng Anh
        }


        // Load các cài đặt âm thanh
        if (PlayerPrefs.HasKey("masterVolume"))
        {
            float masterVolume = PlayerPrefs.GetFloat("masterVolume");
            volumeSlider.value = masterVolume;
            SetMasterVolume(masterVolume);
        }
        else
        {
            volumeSlider.value = 5f; // Mặc định là 50% (giữa 0 và 10)
            SetMasterVolume(5f);
        }

        if (PlayerPrefs.HasKey("musicVolume"))
        {
            float musicVolume = PlayerPrefs.GetFloat("musicVolume");
            musicSlider.value = musicVolume;
            SetMusicVolume(musicVolume);
        }
        else
        {
            musicSlider.value = 5f; // Mặc định là 50%
            SetMusicVolume(5f);
        }

        if (PlayerPrefs.HasKey("sfxVolume"))
        {
            float sfxVolume = PlayerPrefs.GetFloat("sfxVolume");
            sfxSlider.value = sfxVolume;
            SetSFXVolume(sfxVolume);
        }
        else
        {
            sfxSlider.value = 5f; // Mặc định là 50%
            SetSFXVolume(5f);
        }
    }
    #endregion

    #region Apply Settings
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

    // Áp dụng cài đặt chất lượng đồ hoạ
    void ApplyQualitySettings()
    {
        int qualityIndex = qualityDropdown.value;
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    // Phương thức riêng để điều chỉnh từng loại âm thanh
    public void SetMasterVolume(float volume)
    {
        // Chuyển đổi giá trị slider (0-10) sang decibel
        // Sử dụng logarit để điều chỉnh âm lượng một cách tự nhiên
        float volumeDB = (volume == 0) ? -80f : 20f * Mathf.Log10(volume / 10f);
        mixerAudio.SetFloat("masterAudio", volumeDB);
    }

    public void SetMusicVolume(float volume)
    {
        float volumeDB = (volume == 0) ? -80f : 20f * Mathf.Log10(volume / 10f);
        mixerAudio.SetFloat("musicAudio", volumeDB);
    }

    public void SetSFXVolume(float volume)
    {
        float volumeDB = (volume == 0) ? -80f : 20f * Mathf.Log10(volume / 10f);
        mixerAudio.SetFloat("sfxAudio", volumeDB);
    }

    public void OkSoundFX()
    {
        okSound.PlayOneShot(okSound.clip);
    }

    public void CancelSoundFX()
    {
        cancelSound.PlayOneShot(cancelSound.clip);
    }

    // Điều chỉnh lại ApplyAudioSettings
    void ApplyAudioSettings()
    {
        SetMasterVolume(volumeSlider.value);
        SetMusicVolume(musicSlider.value);
        SetSFXVolume(sfxSlider.value);
    }

    // Áp dụng cài đặt ngôn ngữ
    void ApplyLanguageSettings()
    {
        int languageIndex = languageDropdown.value;

        // Thêm logic chuyển đổi ngôn ngữ tại đây
        // Ví dụ: 
        // LocalizationManager.Instance.SetLanguage(languageIndex);
    }
    #endregion

    #region Save and Default Settings

    // Hiển thị hộp thoại lưu cài đặt
    void ShowNoticeSaveSettings()
    {
        StartCoroutine(FadeIn(_saveGroup, 0.15f));
    }

    void ShowNoticeDefaultSettings()
    {
        StartCoroutine(FadeIn(_defaultGroup, 0.15f));
    }

    // Ẩn hộp thoại lưu cài đặt
    public void HideNoticeSaveSettings()
    {
        StartCoroutine(FadeOut(_saveGroup, 0.15f));
    }

    public void HideNoticeDefaultSettings()
    {
        StartCoroutine(FadeOut(_defaultGroup, 0.15f));
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
        int qualityIndex = qualityDropdown.value;
        int languageIndex = languageDropdown.value;

        float masterVolume = volumeSlider.value;
        float musicVolume = musicSlider.value;
        float sfxVolume = sfxSlider.value;
        
        PlayerPrefs.SetInt("resolutionIndex", resolutionIndex);
        PlayerPrefs.SetInt("fullScreenIndex", fullScreenIndex);
        PlayerPrefs.SetInt("qualityIndex", qualityIndex);
        PlayerPrefs.SetInt("languageIndex", languageIndex);

        PlayerPrefs.SetFloat("masterVolume", masterVolume);
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);

        PlayerPrefs.Save();

        Debug.Log("Cài đặt đã được lưu!");
    }
    
    void DefaultAndHide()
    {
        SetDefaultSettings();
        HideNoticeDefaultSettings();
    }

    // Đặt về cài đặt mặc định
    void SetDefaultSettings()
    {
        resolutionDropdown.value = 2; // 1920 x 1080
        fullScreenDropdown.value = 0; // Fullscreen
        qualityDropdown.value = 2; // Medium
        languageDropdown.value = 0; // Tiếng Anh

        volumeSlider.value = 10f; // âm lượng master
        musicSlider.value = 7f; // âm lượng nhạc
        sfxSlider.value = 7f; // âm lượng SFX

        ApplyDisplaySettings();
        ApplyQualitySettings();
        ApplyAudioSettings();
        ApplyLanguageSettings();

        Debug.Log("Cài đặt mặc định đã được đặt!");
    }
    #endregion

    #region FadeIn and FadeOut Animation
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
    #endregion
}