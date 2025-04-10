using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Script_Minh.Input_System // Nếu thay đổi địa chỉ tệp thì phải thay đổi lại đúng địa chỉ
{
    public class MenuManager : MonoBehaviour
    {
        #region Khai báo biến của từng chức năng nút
        [SerializeField] int nextSceneIndex = 1;
        public enum MenuButtonType  // phát triển thêm chức năng ở đây
        { NewGame, Countinue, Quit }

        private Dictionary<MenuButtonType, Action> buttonActions = new Dictionary<MenuButtonType, 
                    Action>();
        
        [SerializeField] public MenuButtonType currentSelectedButton = MenuButtonType.NewGame;
        
        private MenuInputHandler inputHandler;
        #endregion

        #region Setup chức năng nút
        void Start()
        {
            InitializeButtonActions();
            inputHandler = GetComponent<MenuInputHandler>();
            if (inputHandler == null)
            {
                Debug.LogError("MenuInputHandler component không được tìm thấy!");
                return;
            }

            // Đăng ký các sự kiện từ InputHandler
            inputHandler.playerInput.CharacterInput.UI_Button.performed += HandleUIInput;
            inputHandler.playerInput.CharacterInput.Submit.performed += context => HandleSubmit();
            inputHandler.playerInput.CharacterInput.Cancel.performed += context => HandleCancel();
        }

        // Khai báo chức năng cần làm trong MenuManager (quan trọng)
        void InitializeButtonActions()
        {
            buttonActions[MenuButtonType.NewGame] = HandleNewGame;
            buttonActions[MenuButtonType.Countinue] = HandleCountinueGame;
            buttonActions[MenuButtonType.Quit] = HandleQuitGame;
        }
        #endregion

        #region Các chức năng nút (có thể phát triển các chức năng sau này)

        void HandleNewGame()
        {
            Debug.Log("Starting New Game");
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneIndex);
        }

        void HandleCountinueGame()
        {
            Debug.Log("Countinue Game");
        }

        void HandleQuitGame()
        {
            Debug.Log("Quitting Game");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        #endregion

        #region Logic hoạt động nút trong menu
        private void HandleUIInput(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();
            if (input.y > 0) // Di chuyển lên
            {
                currentSelectedButton = GetPreviousButton();
            }
            else if (input.y < 0) // Di chuyển xuống
            {
                currentSelectedButton = GetNextButton();
            }
        }

        private void HandleSubmit()
        {
            // Tìm tất cả các nút hiện có trong scene
            var buttons = FindObjectsOfType<MenuButtonFunction>();

            // Gọi trigger tương ứng với nút đang được chọn
            foreach (var button in buttons)
            {
                if (button.GetButtonType() == currentSelectedButton)
                {
                    button.TriggerButtonBySubmit();
                    break;
                }
            }
        }

        private void HandleCancel()
        {
            Debug.Log("Cancel action triggered");
            // Thêm logic thoát menu hoặc hủy thao tác tại đây
        }

        MenuButtonType GetNextButton()
        {
            int nextIndex = ((int)currentSelectedButton + 1) % 
                Enum.GetNames(typeof(MenuButtonType)).Length;
            return (MenuButtonType)nextIndex;
        }

        MenuButtonType GetPreviousButton()
        {
            int totalButtons = System.Enum.GetNames(typeof(MenuButtonType)).Length;
            int prevIndex = ((int)currentSelectedButton - 1 + totalButtons) % totalButtons;
            return (MenuButtonType)prevIndex;
        }

        public void ExecuteCurrentButtonAction()
        {
            if (buttonActions.TryGetValue(currentSelectedButton, out var action))
            {
                action?.Invoke();
            }
        }

        void OnDestroy()
        {
            if (inputHandler != null)
            {
                // Hủy đăng ký các sự kiện khi component bị hủy
                inputHandler.playerInput.CharacterInput.UI_Button.performed -= HandleUIInput;
                inputHandler.playerInput.CharacterInput.Submit.performed -= context => HandleSubmit();
                inputHandler.playerInput.CharacterInput.Cancel.performed -= context => HandleCancel();
            }
        }
        #endregion
    }
}