using UnityEngine;
using UnityEngine.UI;

public class PlayerUILevelUpManager : MonoBehaviour
{
    private GameSession gameSession;
    //  Chỉ số của người chơi
    public int level = 1;        // Cấp độ hiện tại
    public int exp = 0;          // Kinh nghiệm hiện tại (Runes)
    public int expNeeded = 100;  // Exp cần để lên cấp


    #region Text UI
    // UI
    public Text levelText;
    public Text PreviewlevelText;
    public Text armorText;
    public Text PreviewarmorText;
    public Text damageText;
    public Text PreviewdamageText;
    public Text damagePhysicalText;
    public Text PreviewdamagePhysicalText;
    public Text damageMagicText;
    public Text PreviewdamageMagicText;
    public Text armorMagicText;
    public Text PreviewarmorMagicText;
    public Text armorPoisonText;
    public Text PreviewArmorPoisonText;
    public Text armorFireText;
    public Text PreviewArmorFireText;
    public Text fireText;
    public Text PreviewfireText;
    public Text poisonText;
    public Text PreviewpoisonText;
    public Text magicText;
    public Text PreviewmagicText;
    public Text healthText;
    public Text PreviewhealthText;
    public Text staminaText;
    public Text PreviewstaminaText;
    public Text manaText;
    public Text PreviewmanaText;
    public Text criticalText;
    public Text PreviewcriticalText;
    public Text criticalMagicText;
    public Text PreviewcriticalMagicText;
    public Text criticalPhysicalText;
    public Text PreviewcriticalPhysicalText;
    #endregion

    #region Giá trị dự đoán
    private float tempDamage;
    private float tempDamagePhysical;
    private float tempDamageMagic;
    private float tempArmor;
    private float tempArmorMagic;
    private float tempArmorPoison;
    private float tempArmorFire;
    private float tempTimeFire;
    private float tempTimePoison;
    private float tempTimeMagic;
    private float tempHealth;
    private float tempStamina;
    private float tempMana;
    private float tempCritical;
    private float tempCriticalPhysical;
    private float tempCriticalMagic;
    #endregion

    private void Start()
    {
        gameSession = FindObjectOfType<GameSession>();
    }

    // Nhận Exp từ quái
    public void GainExp(int amount)
    {
        exp += amount;
        if (exp >= expNeeded)
        {
            LevelUp();
        }
    }

    //  Khi lên cấp
    private void LevelUp()
    {
        exp -= expNeeded;
        level++;
        FindObjectOfType<SkillTree>().AddPoints(1); // Thêm điểm kỹ năng khi lên cấp
        expNeeded = Mathf.RoundToInt(expNeeded * 1.15f); // Chi phí tăng 15% mỗi cấp

        #region Nâng cấp
        gameSession.Armor = tempArmor;
        gameSession.Damage = tempDamage;
        gameSession.DamagePhysical = tempDamagePhysical;
        gameSession.DamageMagic = tempDamageMagic;
        gameSession.ArmorMagic = tempArmorMagic;
        gameSession.ArmorPoison = tempArmorPoison;
        gameSession.ArmorFire = tempArmorFire;
        gameSession.TimeFire = tempTimeFire;
        gameSession.TimePoison = tempTimePoison;
        gameSession.TimeMagic = tempTimeMagic;
        gameSession.MaxHealth = tempHealth;
        gameSession.MaxStamina = tempStamina;
        gameSession.MaxMana = tempMana;
        gameSession.Critical = tempCritical;
        gameSession.CriticalMagic = tempCriticalMagic;
        gameSession.CriticalPhysical = tempCriticalPhysical;
        #endregion
    }
    private void Update()
    {
        UpgradeUI();
    }

    public void UpgradeUI()
    {
        //Chuyển từ float sang string để giới hạn số lẻ
        #region Float sang String
        string timeFire = gameSession.TimeFire.ToString("F2");
        string PreviewtimeFire = tempTimeFire.ToString("F2");
        string timePoison = gameSession.TimePoison.ToString("F2");
        string PreviewtimePoison = tempTimePoison.ToString("F2");
        string timeMagic = gameSession.TimeMagic.ToString("F2");
        string PreviewtimeMagic = tempTimeMagic.ToString("F2");
        string armor = gameSession.Armor.ToString("F0");
        string PreviewArmor = tempArmor.ToString("F0");
        string damage = gameSession.Damage.ToString("F0");
        string Previewdamage = tempDamage.ToString("F0");
        string damagePhysical = gameSession.DamagePhysical.ToString("F0");
        string PreviewdamagePhysical = tempDamagePhysical.ToString("F0");
        string damageMagic = gameSession.DamageMagic.ToString("F0");
        string PreviewdamageMagic = tempDamageMagic.ToString("F0");
        string armorMagic = gameSession.ArmorMagic.ToString("F0");
        string PreviewarmorMagic = tempArmorMagic.ToString("F0");
        string armorPoison = gameSession.ArmorPoison.ToString("F0");
        string PreviewarmorPoison = tempArmorPoison.ToString("F0");
        string armorFire = gameSession.ArmorFire.ToString("F0");
        string PreviewarmorFire = tempArmorFire.ToString("F0");
        string health = gameSession.MaxHealth.ToString("F0");
        string PreviewHealth = tempHealth.ToString("F0");
        string stamina = gameSession.MaxStamina.ToString("F0");
        string PreviewStamina = tempStamina.ToString("F0");
        string mana = gameSession.MaxMana.ToString("F0");
        string PreviewMana = tempMana.ToString("F0");
        string critical = gameSession.Critical.ToString("F0");
        string PreviewCritical = tempCritical.ToString("F0");
        string criticalMagic = gameSession.CriticalMagic.ToString("F0");
        string PreviewCriticalMagic = tempCriticalMagic.ToString("F0");
        string criticalPhysical = gameSession.CriticalPhysical.ToString("F0");
        string PreviewCriticalPhysical = tempCriticalPhysical.ToString("F0");
        #endregion

        //Đổi màu số dự đoán
        #region Color
        Color newColor;
        ColorUtility.TryParseHtmlString("#70E7FF", out newColor);
        PreviewlevelText.color = newColor;
        PreviewarmorText.color = newColor;
        PreviewdamageText.color = newColor;
        PreviewdamagePhysicalText.color = newColor;
        PreviewdamageMagicText.color = newColor;
        PreviewarmorMagicText.color = newColor;
        PreviewArmorPoisonText.color = newColor;
        PreviewArmorFireText.color = newColor;
        PreviewfireText.color = newColor;
        PreviewpoisonText.color = newColor;
        PreviewmagicText.color = newColor;
        PreviewhealthText.color = newColor;
        PreviewstaminaText.color = newColor;
        PreviewmanaText.color = newColor;
        PreviewcriticalText.color = newColor;
        PreviewcriticalMagicText.color = newColor;
        PreviewcriticalPhysicalText.color = newColor;
        #endregion


        UpgradeUpdateUINumber2();
        levelText.text = $"{level}";
        PreviewlevelText.text = $"{level + 1}";
        armorText.text = $"{armor}";
        PreviewarmorText.text = $"{PreviewArmor}";
        damageText.text = $"{damage}";
        PreviewdamageText.text = $"{Previewdamage}";
        damagePhysicalText.text = $"{damagePhysical}";
        PreviewdamagePhysicalText.text = $"{PreviewdamagePhysical}";
        damageMagicText.text = $"{damageMagic}";
        PreviewdamageMagicText.text = $"{PreviewdamageMagic}";
        armorMagicText.text = $"{armorMagic}";
        PreviewarmorMagicText.text = $"{PreviewarmorMagic}";
        armorPoisonText.text = $"{armorPoison}";
        PreviewArmorPoisonText.text = $"{PreviewarmorPoison}";
        armorFireText.text = $"{armorFire}";
        PreviewArmorFireText.text = $"{PreviewarmorFire}";
        fireText.text = $"{timeFire}";
        PreviewfireText.text = $"{PreviewtimeFire}";
        poisonText.text = $"{timePoison}";
        PreviewpoisonText.text = $"{PreviewtimePoison}";
        magicText.text = $"{timeMagic}";
        PreviewmagicText.text = $"{PreviewtimeMagic}";
        healthText.text = $"{health}";
        PreviewhealthText.text = $"{PreviewHealth}";
        staminaText.text = $"{stamina}";
        PreviewstaminaText.text = $"{PreviewStamina}";
        manaText.text = $"{mana}";
        PreviewmanaText.text = $"{PreviewMana}";
        criticalText.text = $"{critical}%";
        PreviewcriticalText.text = $"{PreviewCritical}%";
        criticalMagicText.text = $"{criticalMagic}%";
        PreviewcriticalMagicText.text = $"{PreviewCriticalMagic}%";
        criticalPhysicalText.text = $"{criticalPhysical}%";
        PreviewcriticalPhysicalText.text = $"{PreviewCriticalPhysical}%";
    }


    private void UpgradeUpdateUINumber2()
    {
        float damage = gameSession.Damage * 0.05f; 
        tempDamage = gameSession.Damage + damage;
        float damagePhysical = gameSession.DamagePhysical * 0.05f;
        tempDamagePhysical = gameSession.DamagePhysical + damagePhysical;
        float damageMagic = gameSession.DamageMagic * 0.05f;
        tempDamageMagic = gameSession.DamageMagic + damageMagic;
        float armor = gameSession.Armor * 0.05f;
        tempArmor = gameSession.Armor + armor;
        float armorMagic = gameSession.ArmorMagic * 0.05f;
        tempArmorMagic = gameSession.ArmorMagic + armorMagic;
        float armorPoison = gameSession.ArmorPoison * 0.05f;
        tempArmorPoison = gameSession.ArmorPoison + armorPoison;
        float armorFire = gameSession.ArmorFire * 0.05f;
        tempArmorFire = gameSession.ArmorFire + armorFire;
        float timeFire = gameSession.TimeFire * 0.05f;
        tempTimeFire = gameSession.TimeFire - timeFire;
        float timePoison = gameSession.TimePoison * 0.05f;
        tempTimePoison = gameSession.TimePoison - timePoison;
        float timeMagic = gameSession.TimeMagic * 0.05f;
        tempTimeMagic = gameSession.TimeMagic - timeMagic;
        float health = gameSession.MaxHealth * 0.05f;
        tempHealth = gameSession.MaxHealth + health;
        float stamina = gameSession.MaxStamina * 0.05f;
        tempStamina = gameSession.MaxStamina + stamina;
        float mana = gameSession.MaxMana * 0.05f;
        tempMana = gameSession.MaxMana + mana;
        float critical = gameSession.Critical * 0.05f;
        tempCritical = gameSession.Critical + critical;
        float criticalPhysical = gameSession.CriticalPhysical * 0.05f;
        tempCriticalPhysical = gameSession.CriticalPhysical + criticalPhysical;
        float criticalMagic = gameSession.CriticalMagic * 0.05f;
        tempCriticalMagic = gameSession.CriticalMagic + criticalMagic;
    }
}
