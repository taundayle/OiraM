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
    [SerializeField] private Button defaultButton;

    void Start()
    {
        // Tải cài đặt đã lưu và áp dụng
        LoadSettings();
        ApplyDisplaySettings();

        // Thêm sự kiện lắng nghe khi người dùng thay đổi lựa chọn
        resolutionDropdown.onValueChanged.AddListener(delegate { ApplyDisplaySettings(); });
        fullScreenDropdown.onValueChanged.AddListener(delegate { ApplyDisplaySettings(); });

        // Thêm sự kiện cho các nút
        saveButton.onClick.AddListener(SaveSettings);
        defaultButton.onClick.AddListener(SetDefaultSettings);
    }

    // Tải cài đặt từ PlayerPrefs
    void LoadSettings()
    {
        // Tải chỉ số độ phân giải
        if (PlayerPrefs.HasKey("resolutionIndex"))
        {
            int index = PlayerPrefs.GetInt("resolutionIndex");
            resolutionDropdown.value = index;
        }
        else
        {
            resolutionDropdown.value = 2; // Mặc định là 1920 x 1080
        }

        // Tải chỉ số chế độ toàn màn hình
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

        // Xác định chế độ toàn màn hình
        FullScreenMode mode = fullScreenIndex == 0 ? FullScreenMode.FullScreenWindow : 
            FullScreenMode.Windowed;

        // Áp dụng độ phân giải dựa trên chỉ số
        switch (resolutionIndex)
        {
            case 0:
                Screen.SetResolution(3840, 2160, mode);
                break;
            case 1:
                Screen.SetResolution(2560, 1440, mode);
                break;
            case 2:
                Screen.SetResolution(1920, 1080, mode);
                break;
            case 3:
                Screen.SetResolution(1366, 768, mode);
                break;
            case 4:
                Screen.SetResolution(1280, 720, mode);
                break;
            case 5:
                Screen.SetResolution(800, 600, mode);
                break;
        }
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
        // Đặt dropdown về giá trị mặc định
        resolutionDropdown.value = 2; // 1920 x 1080
        fullScreenDropdown.value = 0; // Fullscreen

        // Áp dụng cài đặt mặc định
        ApplyDisplaySettings();
    }

    // Update is called once per frame
    void Update()
    {

    }
}