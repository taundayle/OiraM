using Script.Input_System;
using System.Collections;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    #region Biến khai báo
    public static SettingManager Instance { get; private set; }
    public CanvasGroup _startMenuGroup;
    public MenuManager menuManager;

    public GeneralSetting generalSetting; // Thêm tham chiếu này

    public bool isOpen;

    private CanvasGroup _settingGroup;

    [SerializeField] private float fadeInDuration = 0.25f; // Thời gian để fade-in hoàn tất (giây)
    [SerializeField] private float fadeOutDuration = 0.1f; // Thời gian để fade-out hoàn tất (giây)

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
    }

    private void HandleCancel()
    {
        Debug.Log("Cancel action triggered");

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