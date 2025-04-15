using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSessionRemake : MonoBehaviour
{
    // Các Slider cho máu và thể lực
    [SerializeField] Slider playerHealthSlider;    // Slider cho máu hiện tại
    [SerializeField] Slider playerDamagedSlider;   // Slider cho thanh tổn thương

    [SerializeField] Slider playerStaminaSlider;   // Slider cho thể lực hiện tại
    [SerializeField] Slider playerStaminaDamagedSlider; // Slider cho thể lực tiêu hao

    // Biến cho máu
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;   // Máu tối đa, có thể chỉnh trong Inspector
    private int currentHealth;                      // Máu hiện tại
    private Coroutine damageCoroutine;              // Quản lý hiệu ứng giảm dần của thanh tổn thương
    private float lastDamageTime;                   // Thời gian lần sát thương cuối

    private Coroutine healCoroutine;                // Quản lý hiệu ứng hồi máu
    private float healCooldown = 0.75f;             // Thời gian chờ giữa các lần hồi máu
    private float lastHealTime;                     // Thời gian lần hồi máu cuối

    // Biến cho thể lực (stamina)
    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 100f; // Thể lực tối đa, có thể chỉnh trong Inspector
    [SerializeField] private float recoveryTime = 2f; // Thời gian hồi phục đầy thể lực (giây)
    private float currentStamina;                     // Thể lực hiện tại
    private float lastStaminaUseTime;                 // Thời gian lần cuối sử dụng thể lực
    private Coroutine staminaRecoveryCoroutine;       // Quản lý hiệu ứng hồi phục thể lực
    private float staminaRecoveryDelay = 0.8f;        // Thời gian chờ trước khi hồi phục
    private float staminaRecoveryRate = 40f;          // Tốc độ hồi phục thể lực mỗi giây (sẽ được tính lại)

    // Biến RectTransform cho thanh máu và thể lực
    [Header("RectTransforms Settings")]
    [SerializeField] RectTransform healthBarRect;   // RectTransform của thanh máu
    [SerializeField] RectTransform staminaBarRect;  // RectTransform của thanh thể lực

    [Header("Bar Width Settings")]
    [SerializeField] private float healthBarBaseWidth = 100f;   // Chiều rộng cơ bản cho thanh máu
    [SerializeField] private float staminaBarBaseWidth = 60f;  // Chiều rộng cơ bản cho thanh thể lực

    // Các hằng số giới hạn
    private const int HEALTH_THRESHOLD = 1300;          // Ngưỡng máu để đạt 95% width
    private const int HEALTH_MAX = 1500;                // Máu tối đa
    private const float HEALTH_WIDTH_MAX = 200f;        // Width tối đa cho thanh máu
    private const float HEALTH_WIDTH_AT_THRESHOLD = 190f; // Width tại ngưỡng 1300 máu
    private float targetHealthWidth; // Giá trị width mục tiêu của thanh máu
    private Coroutine healthBarWidthCoroutine; // Theo dõi Coroutine điều chỉnh width

    private const float STAMINA_THRESHOLD = 750f;       // Ngưỡng thể lực để đạt 95% width
    private const float STAMINA_MAX = 900f;             // Thể lực tối đa
    private const float STAMINA_WIDTH_MAX = 130f;       // Width tối đa cho thanh thể lực
    private const float STAMINA_WIDTH_AT_THRESHOLD = 123.5f; // Width tại ngưỡng 750 thể lực
    void Start()
    {
        // Khởi tạo máu
        currentHealth = maxHealth;
        playerHealthSlider.maxValue = 1f;    // Đặt maxValue cho Slider máu là 1 (100%)
        playerHealthSlider.value = 1f;       // Đặt giá trị ban đầu là 100%
        playerDamagedSlider.maxValue = 1f;   // Đặt maxValue cho Slider tổn thương là 1
        playerDamagedSlider.value = 1f;
        lastDamageTime = Time.time;
        lastHealTime = -healCooldown;        // Cho phép hồi máu ngay từ đầu

        // Khởi tạo thể lực
        currentStamina = maxStamina;
        playerStaminaSlider.maxValue = 1f;   // Đặt maxValue cho Slider thể lực là 1
        playerStaminaSlider.value = 1f;
        playerStaminaDamagedSlider.maxValue = 1f; // Đặt maxValue cho Slider thể lực tiêu hao là 1
        playerStaminaDamagedSlider.value = 1f;
        lastStaminaUseTime = Time.time;

        // Tính staminaRecoveryRate ban đầu
        staminaRecoveryRate = maxStamina / recoveryTime;

        // Khởi tạo chiều rộng ban đầu
        UpdateHealthBarWidth();
        UpdateStaminaBarWidths();
    }

    void Update()
    {
        // Xử lý input từ người chơi
        if (Input.GetMouseButtonDown(0))    // Chuột trái: 30 sát thương, 20 thể lực
        {
            TakeDamage(30);
            UseStamina(20);
        }

        if (Input.GetMouseButtonDown(1))    // Chuột phải: 60 sát thương, 40 thể lực
        {
            TakeDamage(60);
            UseStamina(40);
        }

        if (Input.GetKeyDown(KeyCode.Space)) // Phím Space: hồi 20 máu
        {
            Heal(20);
        }

        if (Input.GetKeyDown(KeyCode.M))    // Phím M: hồi 50 máu
        {
            Heal(50);
        }

        if (Input.GetKeyDown(KeyCode.L)) // Phím L: nâng cấp máu và thể lực
        {
            UpgradeHealth(50);
            UpgradeStamina(20);
        }

        // Tự động hồi phục thể lực sau 0.8s không sử dụng
        if (Time.time - lastStaminaUseTime > staminaRecoveryDelay)
        {
            if (staminaRecoveryCoroutine == null)
            {
                staminaRecoveryCoroutine = StartCoroutine(RecoverStamina());
            }
        }
        else if (staminaRecoveryCoroutine != null)
        {
            StopCoroutine(staminaRecoveryCoroutine);
            staminaRecoveryCoroutine = null;
        }
    }

    // Hàm nhận sát thương
    public void TakeDamage(int damage)
    {
        lastDamageTime = Time.time;
        float oldHealthPercentage = playerDamagedSlider.value; // Lưu giá trị cũ của thanh tổn thương
        currentHealth = Mathf.Max(currentHealth - damage, 0);  // Giảm máu, không âm
        float newHealthPercentage = (float)currentHealth / maxHealth; // Tính phần trăm mới
        playerHealthSlider.value = newHealthPercentage;        // Cập nhật thanh máu ngay lập tức

        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
        }
        damageCoroutine = StartCoroutine(DamageEffect(oldHealthPercentage, newHealthPercentage));
    }

    // Hiệu ứng giảm dần thanh tổn thương
    IEnumerator DamageEffect(float startFill, float targetFill)
    {
        playerDamagedSlider.value = startFill; // Đặt thanh tổn thương về giá trị cũ
        while (Time.time - lastDamageTime < 0.47f) // Chờ 0.47s nếu không nhận thêm sát thương
        {
            yield return null;
        }

        float duration = 1f; // Thời gian giảm dần
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime * 1.55f; // Tốc độ giảm
            playerDamagedSlider.value = Mathf.Lerp(startFill, targetFill, elapsed / duration);
            yield return null;
        }
        playerDamagedSlider.value = targetFill; // Đảm bảo khớp với thanh máu
        damageCoroutine = null;
    }

    // Hàm hồi máu
    public void Heal(int healAmount)
    {
        if (Time.time - lastHealTime < healCooldown)
        {
            Debug.Log("Đang trong thời gian chờ hồi máu!");
            return;
        }
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth); // Tăng máu, không vượt tối đa
        float targetFill = (float)currentHealth / maxHealth; // Tính phần trăm mới

        if (healCoroutine != null)
        {
            StopCoroutine(healCoroutine);
        }
        healCoroutine = StartCoroutine(HealEffect(targetFill));
        lastHealTime = Time.time;
    }

    // Hiệu ứng hồi máu
    IEnumerator HealEffect(float targetFill)
    {
        yield return new WaitForSeconds(0.2f); // Chờ 0.2s trước khi bắt đầu
        float startFill = playerHealthSlider.value; // Giá trị ban đầu của thanh máu
        float duration = 0.65f; // Thời gian hiệu ứng
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime * 1.8f; // Tốc độ tăng
            float currentFill = Mathf.Lerp(startFill, targetFill, elapsed / duration);
            playerHealthSlider.value = currentFill;    // Cập nhật cả hai thanh cùng lúc
            playerDamagedSlider.value = currentFill;
            yield return null;
        }
        playerHealthSlider.value = targetFill;    // Đảm bảo đạt giá trị cuối
        playerDamagedSlider.value = targetFill;
        healCoroutine = null;
    }

    // Hàm sử dụng thể lực
    public void UseStamina(float amount)
    {
        if (currentStamina <= 0) return;

        lastStaminaUseTime = Time.time;
        float oldStaminaPercentage = playerStaminaDamagedSlider.value; // Lưu giá trị cũ
        currentStamina = Mathf.Max(currentStamina - amount, 0f);      // Giảm thể lực
        float newStaminaPercentage = currentStamina / maxStamina;     // Tính phần trăm mới
        playerStaminaSlider.value = newStaminaPercentage;             // Cập nhật ngay thanh thể lực
        playerStaminaDamagedSlider.value = oldStaminaPercentage;      // Giữ thanh tiêu hao ở giá trị cũ
    }

    // Hiệu ứng hồi phục thể lực
    IEnumerator RecoverStamina()
    {
        while (currentStamina < maxStamina)
        {
            float recoveryPerFrame = staminaRecoveryRate * Time.deltaTime * 2f;
            currentStamina = Mathf.Min(currentStamina + recoveryPerFrame, maxStamina);
            float newStaminaPercentage = currentStamina / maxStamina;
            playerStaminaSlider.value = newStaminaPercentage; // Cập nhật thanh thể lực

            // Giảm dần thanh tiêu hao để khớp với thanh thể lực
            if (playerStaminaDamagedSlider.value > newStaminaPercentage)
            {
                playerStaminaDamagedSlider.value = Mathf.Lerp(playerStaminaDamagedSlider.value, newStaminaPercentage, Time.deltaTime * 5.58f);
            }
            else
            {
                playerStaminaDamagedSlider.value = newStaminaPercentage;
            }
            yield return null;
        }
        playerStaminaDamagedSlider.value = 1f; // Đảm bảo đầy khi hồi phục xong
        staminaRecoveryCoroutine = null;
    }

    public void UpgradeHealth(int healthIncrease)
    {
        if (maxHealth >= HEALTH_MAX)
        {
            Debug.Log("Máu đã đạt tối đa!");
            return;
        }
        maxHealth = Mathf.Min(maxHealth + healthIncrease, HEALTH_MAX);
        currentHealth = Mathf.Min(currentHealth + healthIncrease, maxHealth);
        UpdateHealthUI();
        UpdateHealthBarWidth();
    }
    public void UpgradeStamina(float staminaIncrease)
    {
        if (maxStamina >= STAMINA_MAX)
        {
            Debug.Log("Thể lực đã đạt tối đa!");
            return;
        }
        maxStamina = Mathf.Min(maxStamina + staminaIncrease, STAMINA_MAX);
        currentStamina = Mathf.Min(currentStamina + staminaIncrease, maxStamina);
        staminaRecoveryRate = maxStamina / recoveryTime; // Cập nhật tốc độ hồi phục
        UpdateStaminaUI();
        UpdateStaminaBarWidths();
    }

    private void UpdateHealthUI()
    {
        float healthPercentage = (float)currentHealth / maxHealth;
        playerHealthSlider.value = healthPercentage;
        playerDamagedSlider.value = healthPercentage;
    }

    private void UpdateStaminaUI()
    {
        float staminaPercentage = currentStamina / maxStamina;
        playerStaminaSlider.value = staminaPercentage;
        playerStaminaDamagedSlider.value = staminaPercentage;
    }

    private void UpdateStaminaBarWidths()
    {
        // Tính width cho thanh thể lực
        float staminaWidth;
        if (maxStamina <= STAMINA_THRESHOLD)
        {
            // Từ 100 đến 750: scale tuyến tính từ staminaBarBaseWidth đến STAMINA_WIDTH_AT_THRESHOLD
            staminaWidth = staminaBarBaseWidth + (STAMINA_WIDTH_AT_THRESHOLD - staminaBarBaseWidth) * ((maxStamina - 100) / (STAMINA_THRESHOLD - 100));
        }
        else
        {
            // Từ 750 đến 900: scale tuyến tính từ STAMINA_WIDTH_AT_THRESHOLD đến STAMINA_WIDTH_MAX
            staminaWidth = STAMINA_WIDTH_AT_THRESHOLD + (STAMINA_WIDTH_MAX - STAMINA_WIDTH_AT_THRESHOLD) * ((maxStamina - STAMINA_THRESHOLD) / (STAMINA_MAX - STAMINA_THRESHOLD));
        }
        staminaBarRect.sizeDelta = new Vector2(staminaWidth, staminaBarRect.sizeDelta.y);
    }

    // Hàm cập nhật width của thanh máu dựa trên maxHealth mới
    private void UpdateHealthBarWidth()
    {
        float newWidth = 0f;
        // Nếu maxHealth từ 100 đến 1200: width tăng tuyến tính từ baseWidth đến HEALTH_WIDTH_AT_THRESHOLD
        if (maxHealth <= HEALTH_THRESHOLD)
        {
            newWidth = healthBarBaseWidth +
                       (HEALTH_WIDTH_AT_THRESHOLD - healthBarBaseWidth) * ((maxHealth - 100f) / (HEALTH_THRESHOLD - 100f));
        }
        else // Nếu maxHealth từ 1200 đến 1500: width tăng tuyến tính từ HEALTH_WIDTH_AT_THRESHOLD đến HEALTH_WIDTH_MAX
        {
            newWidth = HEALTH_WIDTH_AT_THRESHOLD +
                       (HEALTH_WIDTH_MAX - HEALTH_WIDTH_AT_THRESHOLD) * ((maxHealth - HEALTH_THRESHOLD) / (HEALTH_MAX - HEALTH_THRESHOLD));
        }
        targetHealthWidth = newWidth;

        if (healthBarWidthCoroutine != null)
        {
            StopCoroutine(healthBarWidthCoroutine);
        }
        healthBarWidthCoroutine = StartCoroutine(SmoothlyUpdateHealthBarWidth());
    }
    private IEnumerator SmoothlyUpdateHealthBarWidth()
    {
        float startWidth = healthBarRect.sizeDelta.x;
        float duration = 0f; // thời gian cập nhật width (giây)
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float currentWidth = Mathf.Lerp(startWidth, targetHealthWidth, elapsed / duration);
            healthBarRect.sizeDelta = new Vector2(currentWidth, healthBarRect.sizeDelta.y);
            yield return null;
        }
        healthBarRect.sizeDelta = new Vector2(targetHealthWidth, healthBarRect.sizeDelta.y);
    }
}