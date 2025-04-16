using UnityEngine;
using System.Collections;

namespace Script.Input_System
{
    public class AnyKeyButton : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _anyKeyButtonGroup;
        [SerializeField] private CanvasGroup _mainStartButtonGroup;
        [SerializeField] MenuManager _menuManager;
        [SerializeField] private CanvasGroup _groupMainMenu;
        [SerializeField] private float fadeDuration = 7f;
        [SerializeField] private AudioSource anyKeySound;
        

        Animator _animator;
        private bool isFading = false;
        private Coroutine fadeCoroutine;
        private MenuInputHandler inputHandler;

        void Start()
        {
            _animator = GetComponent<Animator>();

            // Sử dụng CanvasGroup thay vì SetActive
            _anyKeyButtonGroup.alpha = 1;
            _anyKeyButtonGroup.interactable = true;
            _anyKeyButtonGroup.blocksRaycasts = true;

            _mainStartButtonGroup.alpha = 0;
            _mainStartButtonGroup.interactable = false;
            _mainStartButtonGroup.blocksRaycasts = false;

            _menuManager.enabled = false;
            _groupMainMenu.alpha = 0;

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
            if (_groupMainMenu.alpha < 1 || isFading) return;

            // Kiểm tra alpha của AnyKeyButton thay vì activeSelf
            if (_anyKeyButtonGroup.alpha > 0)
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
                _groupMainMenu.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
                yield return null;
            }

            _groupMainMenu.alpha = 1f;
            isFading = false;
        }

        private void SkipFade()
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            _groupMainMenu.alpha = 1f;
            isFading = false;
        }

        IEnumerator ShowMenuFunction()
        {
            _animator.SetTrigger("anyKey");
            anyKeySound.Play();
            yield return new WaitForSeconds(1.31f);

            // Sử dụng CanvasGroup để ẩn/hiện
            _anyKeyButtonGroup.alpha = 0;
            _anyKeyButtonGroup.interactable = false;
            _anyKeyButtonGroup.blocksRaycasts = false;

            _mainStartButtonGroup.alpha = 1;
            _mainStartButtonGroup.interactable = true;
            _mainStartButtonGroup.blocksRaycasts = true;

            _menuManager.enabled = true;
        }
    }
}