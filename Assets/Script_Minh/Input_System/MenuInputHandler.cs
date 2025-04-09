using UnityEngine;
using UnityEngine.InputSystem;

namespace Script_Minh.Input_System
{
    public class MenuInputHandler : MonoBehaviour, PlayerInput.ICharacterInputActions
    {
        // Làm public để các script khác có thể truy cập
        public PlayerInput playerInput { get; private set; }

        // Thêm sự kiện để thông báo khi "LockOn" được kích hoạt
        public event System.Action OnLockOnTriggered;

        public Vector2 moveInput { get; private set; }
        public Vector2 lookInput { get; private set; }
        public bool isSprinting { get; private set; }

        private void Awake()
        {
            playerInput = new PlayerInput();
            playerInput.CharacterInput.SetCallbacks(this);
        }

        private void OnEnable()
        {
            playerInput.CharacterInput.Enable();
        }

        private void OnDisable()
        {
            playerInput.CharacterInput.Disable();
        }

        // Explicit interface implementation
        void PlayerInput.ICharacterInputActions.OnLook(InputAction.CallbackContext context)
        {
            Vector2 tempLookInput = context.ReadValue<Vector2>();
            lookInput = tempLookInput;

            // Buộc nhận input từ chuột nếu có sự kiện chuột
            if (Mouse.current != null && Mouse.current.delta.ReadValue().magnitude > 0)
            {
                lookInput = Mouse.current.delta.ReadValue();
            }

            //Debug.Log("Look Input: " + lookInput); // Kiểm tra giá trị input
        }

        void PlayerInput.ICharacterInputActions.OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }

        void PlayerInput.ICharacterInputActions.OnUI_Button(InputAction.CallbackContext context)
        {
            // Implement if needed
        }

        void PlayerInput.ICharacterInputActions.OnSprint(InputAction.CallbackContext context)
        {
            isSprinting = context.ReadValueAsButton();
        }

        void PlayerInput.ICharacterInputActions.OnSubmit(InputAction.CallbackContext context)
        {
            // Implement if needed
        }

        void PlayerInput.ICharacterInputActions.OnCancel(InputAction.CallbackContext context)
        {
            // Implement if needed
        }

        void PlayerInput.ICharacterInputActions.OnSetting(InputAction.CallbackContext context)
        {
            // Implement if needed
        }

        void PlayerInput.ICharacterInputActions.OnLockOn(InputAction.CallbackContext context)
        {
            // Kích hoạt sự kiện khi "LockOn" được nhấn
            if (context.performed)
            {
                OnLockOnTriggered?.Invoke();
            }
        }
    }
}