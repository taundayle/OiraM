using System;
using UnityEngine;
using UnityEngine.UI;

public class GameSession : GameManager
{
    [Header("Panel Upgrade")]
    public GameObject UIUpLevel;
    #region Slider
    [Header("Slider")]
    public Slider healthSlider;
    public Slider easeHealthSlider;
    public Slider staminaSlider;
    public Slider easeStaminaSlider;
    public Slider manaSlider;
    public Slider easeManaSlider;
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
        UIUpLevel.SetActive(false);
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
    }
    private void Update()
    {
        UpdateSlider();
        HealthSlider();
        StaminaSlider();
        ManaSlider();
        if (Input.GetKeyDown(KeyCode.U))
        {
            UIUpLevel.SetActive(!UIUpLevel.activeSelf);
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