using System.Collections;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    public static SettingManager Instance { get; private set; }

    #region Biến khai báo
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
        if (_settingGroup == null)
        {
            _settingGroup = GetComponent<CanvasGroup>();
        }
        _settingGroup.alpha = 0;
        _settingGroup.enabled = true;
        _settingGroup.interactable = false;
        _settingGroup.blocksRaycasts = false;

    }
    #endregion

    #region Hàm công khai
    public void OpenSettings()
    {
        StartCoroutine(FadeIn());
        _settingGroup.interactable = true;
        _settingGroup.blocksRaycasts = true;
    }

    public void CloseSettings()
    {
        StartCoroutine(FadeOut());
        _settingGroup.interactable = false;
        _settingGroup.blocksRaycasts = false;
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
    }
    #endregion
}