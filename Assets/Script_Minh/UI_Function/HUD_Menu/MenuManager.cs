using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Script_Minh.Input_System
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] int nextSceneIndex = 1;
        public enum MenuButtonType { NewGame, Quit }
        private System.Collections.Generic.Dictionary<MenuButtonType, System.Action> buttonActions = new System.Collections.Generic.Dictionary<MenuButtonType, System.Action>();
        [SerializeField] public MenuButtonType currentSelectedButton = MenuButtonType.NewGame;
        private MenuInputHandler inputHandler;

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

        void InitializeButtonActions()
        {
            buttonActions[MenuButtonType.NewGame] = HandleNewGame;
            buttonActions[MenuButtonType.Quit] = HandleQuitGame;
        }

        void HandleNewGame()
        {
            Debug.Log("Starting New Game");
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneIndex);
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
            ExecuteCurrentButtonAction();
        }

        private void HandleCancel()
        {
            Debug.Log("Cancel action triggered");
            // Thêm logic thoát menu hoặc hủy thao tác tại đây
        }

        MenuButtonType GetNextButton()
        {
            return currentSelectedButton == MenuButtonType.NewGame ? MenuButtonType.Quit : MenuButtonType.NewGame;
        }

        MenuButtonType GetPreviousButton()
        {
            return currentSelectedButton == MenuButtonType.Quit ? MenuButtonType.NewGame : MenuButtonType.Quit;
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
    }
}