using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InputSetting : MonoBehaviour
{
    [Header("Rebinding References")]
    public RebindingInput[] rebindingInputs; // Mảng tất cả các RebindingInput

    [Header("Input Save Setting")]
    public Button saveInput;
    public CanvasGroup saveGroup;
    public Button saveButtonInput;
    public Button cancelButtonInputSave;

    [Header("Input Default Setting")]
    public Button defaultInput;
    public CanvasGroup defaultGroup;
    public Button defaultButtonInput;
    public Button cancelButtonInputDefault;

    [Header("Audio Setting")]
    public AudioSource cancelInput;
    public AudioSource okInput;

    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.2f;

    void Start()
    {
        // Ẩn các canvas group ban đầu
        InitializeCanvasGroups();

        // Thiết lập sự kiện cho các nút
        SetupButtonEvents();
    }

    void InitializeCanvasGroups()
    {
        saveGroup.alpha = 0;
        saveGroup.interactable = false;
        saveGroup.blocksRaycasts = false;

        defaultGroup.alpha = 0;
        defaultGroup.interactable = false;
        defaultGroup.blocksRaycasts = false;
    }

    void SetupButtonEvents()
    {
        // Nút mở hộp thoại lưu
        saveInput.onClick.AddListener(ShowSaveConfirmation);
        saveButtonInput.onClick.AddListener(ConfirmSaveSettings);
        cancelButtonInputSave.onClick.AddListener(() => HideCanvasGroup(saveGroup));

        // Nút mở hộp thoại mặc định
        defaultInput.onClick.AddListener(ShowDefaultConfirmation);
        defaultButtonInput.onClick.AddListener(ConfirmDefaultSettings);
        cancelButtonInputDefault.onClick.AddListener(() => HideCanvasGroup(defaultGroup));
    }

    void ShowSaveConfirmation()
    {
        okInput.PlayOneShot(okInput.clip);
        StartCoroutine(FadeInCanvasGroup(saveGroup));
    }

    void ShowDefaultConfirmation()
    {
        okInput.PlayOneShot(okInput.clip);
        StartCoroutine(FadeInCanvasGroup(defaultGroup));
    }

    void ConfirmSaveSettings()
    {
        okInput.PlayOneShot(okInput.clip);

        // Lưu tất cả các cài đặt rebinding
        foreach (var rebindingInput in rebindingInputs)
        {
            rebindingInput.SaveRebindings();
        }

        // Ẩn hộp thoại
        StartCoroutine(FadeOutCanvasGroup(saveGroup));
    }

    void ConfirmDefaultSettings()
    {
        okInput.PlayOneShot(okInput.clip);

        // Đặt lại mặc định cho tất cả các rebinding input
        foreach (var rebindingInput in rebindingInputs)
        {
            // Nếu bạn muốn thêm logic đặt về mặc định, hãy thêm phương thức vào RebindingInput
            // rebindingInput.ResetToDefaultBinding();
        }

        // Ẩn hộp thoại
        StartCoroutine(FadeOutCanvasGroup(defaultGroup));
    }

    void HideCanvasGroup(CanvasGroup group)
    {
        cancelInput.PlayOneShot(cancelInput.clip);
        StartCoroutine(FadeOutCanvasGroup(group));
    }

    IEnumerator FadeInCanvasGroup(CanvasGroup group)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            group.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            yield return null;
        }

        group.alpha = 1f;
        group.interactable = true;
        group.blocksRaycasts = true;
    }

    IEnumerator FadeOutCanvasGroup(CanvasGroup group)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            group.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;
    }

    public void CancelInput()
    {
        cancelInput.PlayOneShot(cancelInput.clip);
    }

    public void OkInput()
    {
        okInput.PlayOneShot(okInput.clip);
    }
}