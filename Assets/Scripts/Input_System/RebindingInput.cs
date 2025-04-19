using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Script.Input_System;
using UnityEngine.InputSystem;
using TMPro;

public class RebindingInput : MonoBehaviour
{
    [SerializeField] private InputActionReference _actionInput = null;

    [SerializeField] private TMP_Text bindingText = null;

    [SerializeField] GameObject startRebinding = null;
    [SerializeField] GameObject watingRebinding = null;

    private InputActionRebindingExtensions.RebindingOperation _rebindingOperation;

    public void StartRebinding()
    {
        // Lấy reference cho gọn
        var action = _actionInput.action;

        // 1. Disable action để cho phép rebind
        action.Disable();

        // 2. Cập nhật UI
        startRebinding.SetActive(false);
        watingRebinding.SetActive(true);

        // 3. Thực hiện rebind
        _rebindingOperation = action.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(op => RebindComplete())
            .Start();
    }

    private void RebindComplete()
    {
        int bindingIndex = _actionInput.action
            .GetBindingIndexForControl(_actionInput.action.controls[0]);

        bindingText.text = InputControlPath.ToHumanReadableString
            (_actionInput.action.bindings[bindingIndex].effectivePath, 
            InputControlPath.HumanReadableStringOptions.OmitDevice);

        _rebindingOperation.Dispose();

        // Enable lại action sau khi hoàn thành
        _actionInput.action.Enable();

        startRebinding.SetActive(true);
        watingRebinding.SetActive(false);
    }
}
