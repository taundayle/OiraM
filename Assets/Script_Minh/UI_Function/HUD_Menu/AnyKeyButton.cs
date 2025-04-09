using UnityEngine;
using System.Collections;

namespace Script_Minh.Input_System
{
    public class AnyKeyButton : MonoBehaviour
    {
        [SerializeField] private GameObject _anyKeyButton;
        [SerializeField] private GameObject _mainMenu;
        [SerializeField] MenuManager _menuManager;
        [SerializeField] CanvasGroup _mainMenuScene;
        [SerializeField] private float fadeDuration = 7f;

        Animator _animator;
        private bool isFading = false;
        private Coroutine fadeCoroutine;
        private MenuInputHandler inputHandler;

        void Start()
        {
            _animator = GetComponent<Animator>();
            _anyKeyButton.SetActive(true);
            _mainMenu.SetActive(false);
            _menuManager.enabled = false;
            _mainMenuScene.alpha = 0;

            inputHandler = FindObjectOfType<MenuInputHandler>();
            if (inputHandler != null)
            {
                inputHandler.playerInput.CharacterInput.Setting.performed += ctx => SkipFade();
            }

            fadeCoroutine = StartCoroutine(FadeInCanvas());
        }

        void OnDestroy()
        {
            if (inputHandler != null)
            {
                inputHandler.playerInput.CharacterInput.Setting.performed -= ctx => SkipFade();
            }
        }

        void Update()
        {
            // Chỉ cho phép nhấn phím khi fade hoàn tất (alpha = 1) và không đang fade
            if (_mainMenuScene.alpha < 1 || isFading) return;

            if (_anyKeyButton.activeSelf)
            {
                if (Input.anyKeyDown)
                {
                    StartCoroutine(ShowMenuFunction());
                }
            }
        }

        IEnumerator FadeInCanvas()
        {
            isFading = true;
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                _mainMenuScene.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
                yield return null;
            }

            _mainMenuScene.alpha = 1f;
            isFading = false;
        }

        private void SkipFade()
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            _mainMenuScene.alpha = 1f;
            isFading = false;
        }

        IEnumerator ShowMenuFunction()
        {
            _animator.SetTrigger("anyKey");
            yield return new WaitForSeconds(1.31f);
            _anyKeyButton.SetActive(false);
            _mainMenu.SetActive(true);
            _menuManager.enabled = true;
        }
    }
}