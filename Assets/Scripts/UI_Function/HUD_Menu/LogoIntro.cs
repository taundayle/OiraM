using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Script.Input_System
{
    public class LogoIntro : MonoBehaviour
    {
        [SerializeField] CanvasGroup logoGroup;
        [SerializeField] private float fadeInDuration = 3f;
        [SerializeField] private float holdDuration = 3f;
        [SerializeField] private float fadeOutDuration = 3f;
        [SerializeField] private int nextSceneIndex = 1;

        private bool isSkipping = false;
        private MenuInputHandler inputHandler;

        public void Start()
        {
            logoGroup.alpha = 0;

            // Tìm MenuInputHandler
            inputHandler = FindObjectOfType<MenuInputHandler>();

            // Kiểm tra và đăng ký sự kiện
            if (inputHandler != null)
            {
                inputHandler.playerInput.CharacterInput.Setting.performed += ctx => SkipIntro();
            }

            StartCoroutine(LogoSequence());
        }

        void OnDestroy()
        {
            // Hủy đăng ký sự kiện
            if (inputHandler != null)
            {
                inputHandler.playerInput.CharacterInput.Setting.performed -= ctx => SkipIntro();
            }
        }

        private void SkipIntro()
        {
            isSkipping = true;
        }

        IEnumerator LogoSequence()
        {
            // Fade In
            float elapsedTime = 0f;
            while (elapsedTime < fadeInDuration && !isSkipping)
            {
                elapsedTime += Time.deltaTime;
                logoGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
                yield return null;
            }
            logoGroup.alpha = 1f;

            // Hold
            elapsedTime = 0f;
            while (elapsedTime < holdDuration && !isSkipping)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Fade Out
            elapsedTime = 0f;
            while (elapsedTime < fadeOutDuration && !isSkipping)
            {
                elapsedTime += Time.deltaTime;
                logoGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);
                yield return null;
            }
            logoGroup.alpha = 0f;

            yield return new WaitForSeconds(0.5f);

            // Chuyển scene
            SceneManager.LoadScene(nextSceneIndex);
        }
    }
}