using System;
using UnityEngine;
using UnityEngine.UI;

public class GameSession : GameManager
{
    public PlayerUILevelUpManager playerUILevelUpManager;
    [Header("Panel")]
    public GameObject UiUpIndex;
    public GameObject UiUpLevel;
    #region Slider
    [Header("Slider")]
    public Slider healthSlider;
    public Slider easeHealthSlider;
    public Slider staminaSlider;
    public Slider easeStaminaSlider;
    public Slider manaSlider;
    public Slider easeManaSlider;
    public Slider expSlider;
    #endregion

    #region Ease
    //Thêm giá trị cho thanh máu, năng lượng và mana
    private float easeHealth;
    private float easeStamina;
    private float easeMana;
    #endregion

    #region lerpSpeed
    //Đây là tốc độ giảm dần cho thanh máu, năng luợng và mana
    [SerializeField] private float lerpSpeedHealth = 0.01f;
    [SerializeField] private float lerpSpeedStamina = 0.01f;
    [SerializeField] private float lerpSpeedMana = 0.01f;
    #endregion


    private void Start()
    {
        UiUpIndex.SetActive(false);
        UiUpLevel.SetActive(false);
        UpdateUpgrade();
        easeHealth = Health;
        easeStamina = Stamina;
        easeMana = Mana;
    }
    void UpdateUpgrade()
    {
        Health = MaxHealth;
        Stamina = MaxStamina;
        Mana = MaxMana;
    }
    void UpdateSlider()
    {
        //Cập nhật UI liên tục
        healthSlider.maxValue = MaxHealth;
        easeHealthSlider.maxValue = MaxHealth;
        staminaSlider.maxValue = MaxStamina;
        easeStaminaSlider.maxValue = MaxStamina;
        manaSlider.maxValue = MaxMana;
        easeManaSlider.maxValue = MaxMana;
        expSlider.maxValue = playerUILevelUpManager.expNeeded;   //EXP cần thiết để lên cấp
    }
    private void Update()
    {
        UpdateSlider();
        HealthSlider();
        StaminaSlider();
        ManaSlider();
        ExpSlider();
        if (Input.GetKeyDown(KeyCode.U))
        {
            UiUpIndex.SetActive(!UiUpIndex.activeSelf);
            UiUpLevel.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            UiUpLevel.SetActive(!UiUpLevel.activeSelf);
            UiUpIndex.SetActive(false);
        }
    }
    void HealthSlider()
    {
        if (healthSlider.value != Health)
        {
            healthSlider.value = Health;
        }
        if (Health <= 0)
        {
            Debug.Log("Da chet");
        }
        if (healthSlider.value != easeHealthSlider.value)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, Health, lerpSpeedHealth);
        }
    }
    void StaminaSlider()
    {
        if (staminaSlider.value != Stamina)
        {
            staminaSlider.value = Stamina;
        }
        if (Stamina != MaxStamina)
        {
            Stamina += Time.deltaTime * 3; // Tăng năng lượng theo thời gian
            Stamina = Mathf.Clamp(Stamina, 0, MaxStamina);
        }
        if (Stamina <= 0)
        {
            Debug.Log("Hết năng lượng");
        }
        if (staminaSlider.value != easeStaminaSlider.value)
        {
            easeStaminaSlider.value = Mathf.Lerp(easeStaminaSlider.value, Stamina, lerpSpeedStamina);
        }
    }
    void ManaSlider()
    {
        if (manaSlider.value != Mana)
        {
            manaSlider.value = Mana;
        }
        if (Mana != MaxMana)
        {
            Mana += Time.deltaTime * 0.3f; // Tăng năng lượng theo thời gian
            Mana = Mathf.Clamp(Mana, 0, MaxMana);
            if (Mana <= 0)
            {
                Debug.Log("Hết mana");
            }
            if (manaSlider.value != easeManaSlider.value)
            {
                easeManaSlider.value = Mathf.Lerp(easeManaSlider.value, Mana, lerpSpeedMana);
            }
        }
    }
    void ExpSlider()
    {
        expSlider.value = playerUILevelUpManager.exp;
    }
}