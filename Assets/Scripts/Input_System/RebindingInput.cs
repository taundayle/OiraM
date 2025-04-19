using UnityEngine;
using Script.Input_System;
using UnityEngine.InputSystem;
using TMPro;

public class RebindingInput : MonoBehaviour
{
    [SerializeField] private InputActionReference _actionInput;

    [SerializeField] private TMP_Text bindingText = null;

    [SerializeField] GameObject startRebinding = null;
    [SerializeField] GameObject waitingRebinding = null;

    private InputActionRebindingExtensions.RebindingOperation _rebindingOperation;

    public void StartRebinding()
    {
        // Kiểm tra tính hợp lệ của input action
        if (_actionInput == null || _actionInput.action == null)
        {
            Debug.LogError("Input Action Reference là null!");
            return;
        }

        var action = _actionInput.action;

        // 1. Vô hiệu hóa action để cho phép rebind
        action.Disable();

        // 2. Cập nhật giao diện người dùng
        startRebinding.SetActive(false);
        waitingRebinding.SetActive(true);

        // 3. Thực hiện rebind
        _rebindingOperation = action.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => HandleRebindComplete(operation))
            .Start();
    }

    private void HandleRebindComplete(InputActionRebindingExtensions.RebindingOperation operation)
    {
        try
        {
            // Kiểm tra xem action có controls hay không
            if (_actionInput.action.controls.Count == 0)
            {
                Debug.LogWarning("Không có control nào được tìm thấy cho action.");
                ResetRebindUI();
                return;
            }

            // Lấy path của binding mới
            string newBindingPath = GetNewBindingPath(operation);

            // Cập nhật văn bản binding
            if (!string.IsNullOrEmpty(newBindingPath))
            {
                bindingText.text = InputControlPath.ToHumanReadableString(
                    newBindingPath,
                    InputControlPath.HumanReadableStringOptions.OmitDevice
                );
            }
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

    private string GetNewBindingPath(InputActionRebindingExtensions.RebindingOperation operation)
    {
        // Thử lấy path từ operation
        if (operation != null && operation.action != null && operation.action.bindings.Count > 0)
        {
            return operation.action.bindings[operation.bindingIndex].effectivePath;
        }

        // Nếu không được, thử lấy từ action
        if (_actionInput.action.bindings.Count > 0)
        {
            return _actionInput.action.bindings[0].effectivePath;
        }

        Debug.LogWarning("Không thể tìm thấy binding path.");
        return string.Empty;
    }

    private void ResetRebindUI()
    {
        startRebinding.SetActive(true);
        waitingRebinding.SetActive(false);
    }

    // Phương thức lưu rebinding
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

    // Phương thức tải rebinding
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