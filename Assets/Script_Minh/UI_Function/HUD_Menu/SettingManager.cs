using System.Collections;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    public static SettingManager Instance { get; private set; }

    #region Biến khai báo
    [SerializeField] private GameObject _settingMenu;
    private CanvasGroup _settingGroup;

    [SerializeField] private float fadeInDuration = 0.25f; // Thời gian để fade-in hoàn tất (giây)
    [SerializeField] private float fadeOutDuration = 0.1f; // Thời gian để fade-out hoàn tất (giây)
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
        _settingGroup = _settingMenu.GetComponent<CanvasGroup>();
        if (_settingGroup == null)
        {
            Debug.LogError("CanvasGroup component not found on _settingMenu!");
        }
        _settingGroup.alpha = 0;
        _settingGroup.enabled = false;

        _settingMenu.SetActive(false);
    }
    #endregion

    #region Hàm công khai
    public void OpenSettings()
    {
        _settingMenu.SetActive(true);
        _settingGroup.enabled = true;
        StartCoroutine(FadeIn());
    }

    public void CloseSettings()
    {
        _settingGroup.enabled = true; // Đảm bảo CanvasGroup enabled để thay đổi alpha
        StartCoroutine(FadeOut());
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
        _settingGroup.enabled = false;  // Tắt CanvasGroup
        _settingMenu.SetActive(false);  // Deactivate GameObject sau khi fade-out hoàn tất
    }
    #endregion
}