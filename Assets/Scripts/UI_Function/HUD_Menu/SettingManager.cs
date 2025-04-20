using Script.Input_System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    #region Biến khai báo
    public static SettingManager Instance { get; private set; }

    [Header("Main Menu")]
    public CanvasGroup _startMenuGroup;
    public MenuManager menuManager;

    [Header("Setting General")]
    public GeneralSetting generalSetting; // Thêm tham chiếu này
    public CanvasGroup generalSettingGroup;
    public GameObject generalSettingCanvas;

    public Button generalSettingButton;
    public CanvasGroup generalSettingGroup1;

    [Header("Setting Input")]
    public InputSetting inputSetting;
    public CanvasGroup inputSettingGroup;
    public GameObject inputSettingCanvas;

    public Button inputSettingButton;
    public CanvasGroup inputSettingGroup1;

    [Header("Setting Check")]
    public bool isOpen;

    private CanvasGroup _settingGroup;

    [Header("Setting Animation")]
    [SerializeField] private float fadeInDuration = 0.25f; // Thời gian để fade-in hoàn tất (giây)
    [SerializeField] private float fadeOutDuration = 0.2f; // Thời gian để fade-out hoàn tất (giây)

    [Header("Audio Clip Cancel")]
    [SerializeField] private AudioSource cancelSound;
    [SerializeField] private AudioSource selectSound;

    [Header("Current Active Tab")]
    private bool isGeneralTabActive = true;

    private MenuInputHandler _menuInputHandler;
    #endregion

    #region Singleton Pattern (Setup trên Start)
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (_settingGroup == null)
        {
            _settingGroup = GetComponent<CanvasGroup>();
        }
        _settingGroup.alpha = 0;
        _settingGroup.enabled = true;
        _settingGroup.interactable = false;
        _settingGroup.blocksRaycasts = false;

        _menuInputHandler = GetComponent<MenuInputHandler>();
        _menuInputHandler.playerInput.CharacterInput.Cancel.performed += context => HandleCancel();

        // Initially set up button listeners for tab switching
        generalSettingButton.onClick.AddListener(() => SwitchToTab(true));
        inputSettingButton.onClick.AddListener(() => SwitchToTab(false));

        // Ensure only General tab is visible initially
        ShowGeneralTab();
    }

    private void SwitchToTab(bool toGeneralTab)
    {
        if (isGeneralTabActive == toGeneralTab) return;

        StartCoroutine(AnimateTabSwitch(toGeneralTab));
    }

    private void ShowGeneralTab()
    {
        generalSettingCanvas.SetActive(true);
        inputSettingCanvas.SetActive(false);

        generalSettingGroup.alpha = 1;
        generalSettingGroup.interactable = true;
        generalSettingGroup.blocksRaycasts = true;

        inputSettingGroup.alpha = 0;
        inputSettingGroup.interactable = false;
        inputSettingGroup.blocksRaycasts = false;

        isGeneralTabActive = true;
    }

    private void ShowInputTab()
    {
        generalSettingCanvas.SetActive(false);
        inputSettingCanvas.SetActive(true);

        generalSettingGroup.alpha = 0;
        generalSettingGroup.interactable = false;
        generalSettingGroup.blocksRaycasts = false;

        inputSettingGroup.alpha = 1;
        inputSettingGroup.interactable = true;
        inputSettingGroup.blocksRaycasts = true;

        isGeneralTabActive = false;
    }

    private IEnumerator AnimateTabSwitch(bool toGeneralTab)
    {
        float duration = 0.25f;
        float elapsed = 0f;

        // Fade out current tab
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);

            if (toGeneralTab)
            {
                inputSettingGroup.alpha = alpha;
            }
            else
            {
                generalSettingGroup.alpha = alpha;
            }

            yield return null;
        }

        // Switch tabs
        if (toGeneralTab)
        {
            ShowGeneralTab();
        }
        else
        {
            ShowInputTab();
        }

        // Reset elapsed time for fade in
        elapsed = 0f;

        // Fade in new tab
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / duration);

            if (toGeneralTab)
            {
                generalSettingGroup.alpha = alpha;
            }
            else
            {
                inputSettingGroup.alpha = alpha;
            }

            yield return null;
        }

        // Ensure final alpha is set
        if (toGeneralTab)
        {
            generalSettingGroup.alpha = 1f;
        }
        else
        {
            inputSettingGroup.alpha = 1f;
        }
    }

    public void HandleSelectSettings()
    {
        selectSound.PlayOneShot(selectSound.clip);
    }

    private void HandleCancel()
    {
        Debug.Log("Cancel action triggered");

        UnlockInputTab();

        cancelSound.PlayOneShot(cancelSound.clip);

        // Kiểm tra và ẩn các canvas group thông báo nếu đang hiển thị
        if (generalSetting._saveGroup.alpha > 0)
        {
            generalSetting.HideNoticeSaveSettings();
            return;
        }

        if (generalSetting._defaultGroup.alpha > 0)
        {
            generalSetting.HideNoticeDefaultSettings();
            return;
        }

        if (inputSetting.defaultGroup.alpha > 0)
        {
            inputSetting.HideNoticeDefaultSettings();
            return;
        }

        if (inputSetting.saveGroup.alpha > 0)
        {
            inputSetting.HideNoticeSaveSettings();
            return;
        }

        // Nếu không có thông báo nào đang hiển thị thì mới thực hiện fade out settings
        StartCoroutine(FadeOut());
    }

    private void OnDestroy()
    {
        _menuInputHandler.playerInput.CharacterInput.Cancel.performed -= context => HandleCancel();
    }
    #endregion

    #region Hàm công khai
    public void OpenSettings()
    {
        StartCoroutine(FadeIn());
        _settingGroup.interactable = true;
        _settingGroup.blocksRaycasts = true;
        _menuInputHandler.enabled = true;

        isOpen = true;
    }

    public void CloseSettings()
    {
        StartCoroutine(FadeOut());
        _settingGroup.interactable = false;
        _settingGroup.blocksRaycasts = false;

        isOpen = false;
    }

    public void LockInputTab()
    {
        generalSettingGroup1.interactable = false;
        generalSettingGroup1.blocksRaycasts = false;

        inputSettingGroup1.interactable = false;
        inputSettingGroup1.blocksRaycasts = false;

        generalSettingGroup.interactable = false;
        generalSettingGroup.blocksRaycasts = false;

        inputSettingGroup.interactable = false;
        inputSettingGroup.blocksRaycasts = false;
    }

    public void UnlockInputTab()
    {
        generalSettingGroup1.interactable = true;
        generalSettingGroup1.blocksRaycasts = true;

        inputSettingGroup1.interactable = true;
        inputSettingGroup1.blocksRaycasts = true;

        generalSettingGroup.interactable = true;
        generalSettingGroup.blocksRaycasts = true;

        inputSettingGroup.interactable = true;
        inputSettingGroup.blocksRaycasts = true;
    }
    #endregion

    #region Animation Fade Đậm Nhạt
    private IEnumerator FadeIn()
    {
        float startAlpha = 0f;
        float endAlpha = 1f;
        float elapsed = 0f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeInDuration);
            _settingGroup.alpha = alpha;
            yield return null;
        }

        _settingGroup.alpha = endAlpha; // Đảm bảo alpha đạt giá trị cuối cùng
    }

    private IEnumerator FadeOut()
    {
        float startAlpha = _settingGroup.alpha; // Lấy alpha hiện tại
        float endAlpha = 0f;
        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeOutDuration);
            _settingGroup.alpha = alpha;
            yield return null;
        }

        _settingGroup.alpha = endAlpha; // Đảm bảo alpha đạt giá trị cuối

        _settingGroup.alpha = 0;
        _settingGroup.enabled = true;
        _settingGroup.interactable = false;
        _settingGroup.blocksRaycasts = false;
        _menuInputHandler.enabled = false;

        _startMenuGroup.alpha = 1f;
        _startMenuGroup.blocksRaycasts = true;
        _startMenuGroup.interactable = true;

        menuManager.EnableInput();

        isOpen = false;
    }
    #endregion
}