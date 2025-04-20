using UnityEngine;
using Script.Input_System;
using UnityEngine.InputSystem;
using TMPro;
using System.Linq;

public class RebindingInput : MonoBehaviour
{
    [SerializeField] private InputActionReference _actionInput;

    [SerializeField] private TMP_Text bindingText = null;

    [SerializeField] GameObject startRebinding = null;
    [SerializeField] GameObject waitingRebinding = null;

    private InputActionRebindingExtensions.RebindingOperation _rebindingOperation;
    private int _bindingIndex = -1;

    void Start()
    {
        // Kiểm tra và in thông tin chi tiết về action
        ValidateInputAction();
    }

    void ValidateInputAction()
    {
        if (_actionInput == null || _actionInput.action == null)
        {
            Debug.LogError("Input Action Reference là null!");
            return;
        }

        // In thông tin chi tiết về action
        Debug.Log($"Action Name: {_actionInput.action.name}");
        Debug.Log($"Total Bindings: {_actionInput.action.bindings.Count}");

        // Tìm index của binding đầu tiên phù hợp
        _bindingIndex = FindFirstNonCompositeBindingIndex();

        if (_bindingIndex != -1)
        {
            // Cập nhật văn bản binding ban đầu
            UpdateBindingText(_bindingIndex);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy binding phù hợp.");
        }
    }

    int FindFirstNonCompositeBindingIndex()
    {
        var action = _actionInput.action;
        for (int i = 0; i < action.bindings.Count; i++)
        {
            // Bỏ qua các binding là composite
            if (!action.bindings[i].isPartOfComposite)
            {
                return i;
            }
        }
        return -1;
    }

    void UpdateBindingText(int bindingIndex)
    {
        if (bindingText == null) return;

        var bindingPath = _actionInput.action.bindings[bindingIndex].effectivePath;
        bindingText.text = InputControlPath.ToHumanReadableString(
            bindingPath,
            InputControlPath.HumanReadableStringOptions.OmitDevice
        );
    }

    public void StartRebinding()
    {
        // Kiểm tra tính hợp lệ của input action
        if (_actionInput == null || _actionInput.action == null)
        {
            Debug.LogError("Input Action Reference là null!");
            return;
        }

        if (_bindingIndex == -1)
        {
            Debug.LogError("Không có binding nào để rebind!");
            return;
        }

        var action = _actionInput.action;

        // 1. Vô hiệu hóa action để cho phép rebind
        action.Disable();

        // 2. Cập nhật giao diện người dùng
        startRebinding.SetActive(false);
        waitingRebinding.SetActive(true);

        // 3. Thực hiện rebind
        _rebindingOperation = action.PerformInteractiveRebinding(_bindingIndex)
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => HandleRebindComplete(operation))
            .Start();
    }

    private void HandleRebindComplete(InputActionRebindingExtensions.RebindingOperation operation)
    {
        try
        {
            // Cập nhật văn bản binding
            UpdateBindingText(_bindingIndex);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lỗi trong quá trình hoàn tất rebind: {e.Message}");
        }
        finally
        {
            // Dọn dẹp và khôi phục trạng thái ban đầu
            _rebindingOperation?.Dispose();
            _actionInput.action.Enable();
            ResetRebindUI();
        }
    }

    private void ResetRebindUI()
    {
        startRebinding.SetActive(true);
        waitingRebinding.SetActive(false);
    }

    // Các phương thức SaveRebindings, LoadRebindings giữ nguyên như trước
    public void SaveRebindings()
    {
        if (_actionInput?.action == null) return;

        try
        {
            string bindingJson = _actionInput.action.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString($"{_actionInput.action.name}_Binding", bindingJson);
            PlayerPrefs.Save();
            Debug.Log($"Đã lưu rebinding cho {_actionInput.action.name}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lỗi khi lưu rebinding: {e.Message}");
        }
    }

    public void LoadRebindings()
    {
        if (_actionInput?.action == null) return;

        try
        {
            string savedBindingJson = PlayerPrefs.GetString($"{_actionInput.action.name}_Binding", "");
            if (!string.IsNullOrEmpty(savedBindingJson))
            {
                _actionInput.action.LoadBindingOverridesFromJson(savedBindingJson);
                Debug.Log($"Đã tải rebinding cho {_actionInput.action.name}");

                // Cập nhật lại văn bản binding sau khi tải
                if (_bindingIndex != -1)
                {
                    UpdateBindingText(_bindingIndex);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lỗi khi tải rebinding: {e.Message}");
        }
    }

    void OnEnable()
    {
        LoadRebindings();
    }
}