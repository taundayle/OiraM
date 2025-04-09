using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace Script_Minh.Input_System
{
    public class MenuButtonFunction : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
    {
        [SerializeField] private MenuManager menuManager; // Tham chiếu đến MenuManager
        [SerializeField] private Animator animator;       // Animator để xử lý animation
        [SerializeField] private MenuManager.MenuButtonType buttonType; // Loại nút (NewGame, Quit, v.v.)
        [SerializeField] private float durationAnimation = 0.35f;       // Thời gian animation

        void Update()
        {
            // Cập nhật trạng thái "selected" dựa trên nút hiện tại được chọn trong MenuManager
            bool isSelected = menuManager.currentSelectedButton == buttonType;
            if (animator != null)
            {
                animator.SetBool("Selected", isSelected);
            }
        }

        // Trả về loại nút của button này
        public MenuManager.MenuButtonType GetButtonType()
        {
            return buttonType;
        }

        // Xử lý khi chuột hover vào button
        public void OnPointerEnter(PointerEventData eventData)
        {
            // Cập nhật nút hiện tại được chọn trong MenuManager
            menuManager.currentSelectedButton = buttonType;

            // Phát âm thanh select (nếu có AudioManager)
            //AudioManager.Instance?.PlaySelectSound();
        }

        // Xử lý khi click vào button
        public void OnPointerClick(PointerEventData eventData)
        {
            // Đảm bảo nút được chọn là nút này
            menuManager.currentSelectedButton = buttonType;

            // Phát âm thanh press (nếu có AudioManager)
            //AudioManager.Instance?.PlayPressSound();

            // Bắt đầu coroutine để chạy animation và thực thi hành động
            StartCoroutine(ExecuteButtonActionWithAnimation());
        }

        // Coroutine để chạy animation và thực thi hành động
        private IEnumerator ExecuteButtonActionWithAnimation()
        {
            // Kích hoạt trigger "pressed"
            if (animator != null)
            {
                animator.SetTrigger("Pressed");
            }

            // Đợi thời gian animation hoàn thành
            yield return new WaitForSeconds(durationAnimation);

            // Gọi MenuManager để thực thi hành động của nút hiện tại
            // Không cần reset trigger vì nó tự động reset trong Animator
            menuManager.ExecuteCurrentButtonAction();
        }
    }
}