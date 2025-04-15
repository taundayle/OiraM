using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Linq;

[RequireComponent(typeof(ScrollRect))]
public class ScrollRectAutoScroll : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float scrollSpeed = 10f;
    [SerializeField] private DefaultInputActions inputActions;

    // Cấu hình chi tiết hơn cho việc chuyển đổi giữa các UI
    [Header("Navigation Settings")]
    [SerializeField] private bool enableUINavigation = true;
    [SerializeField] private float navigationDelay = 0.2f;

    private bool mouseOver = false;
    private List<Selectable> m_Selectables = new List<Selectable>();
    private ScrollRect m_ScrollRect;
    private Vector2 m_NextScrollPosition = Vector2.up;
    private float lastNavigationTime;

    void Awake()
    {
        if (inputActions == null)
        {
            inputActions = new DefaultInputActions();
        }

        m_ScrollRect = GetComponent<ScrollRect>();

        // Tự động tìm content nếu chưa được set
        if (m_ScrollRect.content == null)
        {
            RectTransform[] childTransforms = GetComponentsInChildren<RectTransform>();
            foreach (RectTransform childTransform in childTransforms)
            {
                if (childTransform != m_ScrollRect.transform)
                {
                    m_ScrollRect.content = childTransform;
                    break;
                }
            }
        }
    }

    void OnEnable()
    {
        inputActions.Enable();

        // Sử dụng UI.Navigate thay vì Player.Navigate
        inputActions.UI.Navigate.performed += OnNavigate;
    }

    void OnDisable()
    {
        // Sử dụng UI.Navigate thay vì Player.Navigate
        inputActions.UI.Navigate.performed -= OnNavigate;
        inputActions.Disable();
    }

    void Start()
    {
        UpdateSelectables();

    }

    void Update()
    {
        if (!mouseOver)
        {
            m_ScrollRect.verticalNormalizedPosition = Mathf.Lerp(
                m_ScrollRect.verticalNormalizedPosition,
                m_NextScrollPosition.y,
                scrollSpeed * Time.unscaledDeltaTime
            );
        }
        else
        {
            m_NextScrollPosition = new Vector2(0, m_ScrollRect.verticalNormalizedPosition);
        }
    }

    void OnNavigate(InputAction.CallbackContext context)
    {
        // Kiểm tra xem có thể điều hướng không
        if (!enableUINavigation ||
            Time.time - lastNavigationTime < navigationDelay ||
            m_Selectables.Count == 0)
            return;

        // Lấy hướng điều hướng
        Vector2 navigationInput = context.ReadValue<Vector2>();

        // Tìm phần tử được chọn hiện tại
        Selectable currentSelected = EventSystem.current.currentSelectedGameObject
            ? EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>()
            : null;

        if (currentSelected == null) return;

        // Tìm index của phần tử hiện tại
        int currentIndex = m_Selectables.IndexOf(currentSelected);
        if (currentIndex == -1) return;

        // Tìm phần tử tiếp theo dựa trên hướng điều hướng
        Selectable nextSelectable = FindNextSelectableInDirection(currentIndex, navigationInput);

        if (nextSelectable != null)
        {
            // Chọn phần tử tiếp theo
            nextSelectable.Select();

            // Cuộn đến phần tử mới
            ScrollToSelected(false);

            // Cập nhật thời gian điều hướng cuối cùng
            lastNavigationTime = Time.time;
        }
    }

    void UpdateSelectables()
    {
        if (m_ScrollRect?.content != null)
        {
            m_Selectables.Clear();

            // Thu thập tất cả các Selectable trong content và sắp xếp từ trên xuống dưới theo vị trí Y
            m_Selectables = m_ScrollRect.content
                .GetComponentsInChildren<Selectable>()
                .OrderByDescending(s => s.transform.position.y)
                .ToList();
        }
    }

    Selectable FindNextSelectableInDirection(int currentIndex, Vector2 direction)
    {
        // Sắp xếp lại danh sách Selectables theo thứ tự từ trên xuống dưới theo vị trí Y
        m_Selectables = m_ScrollRect.content
            .GetComponentsInChildren<Selectable>()
            .OrderByDescending(s => s.transform.position.y)
            .ToList();

        if (direction.y > 0.5f) // Lên trên
        {
            // Tìm phần tử kế tiếp phía trên, đảm bảo đúng thứ tự
            for (int i = currentIndex + 1; i < m_Selectables.Count; i++)
            {
                if (m_Selectables[i].interactable)
                {
                    return m_Selectables[i];
                }
            }

            // Nếu không tìm thấy, trả về null
            return null;
        }
        else if (direction.y < -0.5f) // Xuống dưới
        {
            // Tìm phần tử kế tiếp phía dưới, đảm bảo đúng thứ tự
            for (int i = currentIndex - 1; i >= 0; i--)
            {
                if (m_Selectables[i].interactable)
                {
                    return m_Selectables[i];
                }
            }

            // Nếu không tìm thấy, trả về null
            return null;
        }

        return null;
    }

    void ScrollToSelected(bool quickScroll)
    {
        if (m_ScrollRect?.content == null || m_Selectables.Count == 0) return;

        Selectable selectedElement = EventSystem.current.currentSelectedGameObject
            ? EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>()
            : null;

        if (selectedElement && m_Selectables.Contains(selectedElement))
        {
            int selectedIndex = m_Selectables.IndexOf(selectedElement);

            float normalizedScrollPosition = 1 - (selectedIndex / ((float)m_Selectables.Count - 1));

            if (quickScroll)
            {
                m_ScrollRect.verticalNormalizedPosition = normalizedScrollPosition;
                m_NextScrollPosition = new Vector2(0, normalizedScrollPosition);
            }
            else
            {
                m_NextScrollPosition = new Vector2(0, normalizedScrollPosition);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
        ScrollToSelected(false);
    }
}