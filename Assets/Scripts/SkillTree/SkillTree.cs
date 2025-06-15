using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class SkillTree : MonoBehaviour
{
    public static SkillTree skilltree;
    private void Awake() => skilltree = this;

    public int[] SkillLevels;
    public int[] SkillCaps;
    public string[] SkillNames;
    public string[] SkillDescriptions;

    public List<Skill> SkillList;
    public GameObject SkillHolder;

    public List<GameObject> ConnectorList;
    public GameObject ConnectorHolder;

    public int SkillPoint;

    public TMP_Text TextSkillPoint;

    public void AddPoints(int amount)
    {
        SkillPoint += amount;
    }

    private void Start()
    {
        // Có 9 kỹ năng
        SkillLevels = new int[9];

        // Đảm bảo SkillCaps có đủ 9 phần tử
        SkillCaps = new int[] { 1, 5, 5, 2, 10, 10, 3, 4, 5 };

        // Tên kỹ năng (đủ 9 tên)
        SkillNames = new[]
        {
        "Xuất kiếm", "Xung khí", "Cường lực",
        "Upgrade 4", "Booster 1", "Booster 2",
        "Booster 3", "Upgrade 5", "Upgrade 6"
    };

        // Mô tả kỹ năng (đủ 9 mô tả)
        SkillDescriptions = new[]
        {
            "Rút kiếm chém",                   // Upgrade 1
            "Một luồng gió được tạo ra tăng tầm đánh",                 // Upgrade 2
            "2222222222",           // Upgrade 3
            "Thực hiện một hành động vô cùng ấn tượng",         // Upgrade 4
            "Tăng sức mạnh dựa trên các chỉ số toán học",       // Booster 1
            "Tăng hiệu quả dựa trên hệ số cộng dồn",            // Booster 2
            "Tăng sức mạnh hỗ trợ cho toàn bộ kỹ năng",         // Booster 3
            "Mở khóa kỹ năng kết hợp nâng cao",                 // Upgrade 5
            "Nâng cấp cuối cùng với sức mạnh vượt trội"         // Upgrade 6
    };

        // Đảm bảo SkillList đã được khởi tạo
        if (SkillList == null) SkillList = new List<Skill>();
        SkillList.Clear();
        foreach (var skill in SkillHolder.GetComponentsInChildren<Skill>())
            SkillList.Add(skill);

        // Đảm bảo ConnectorList đã được khởi tạo
        if (ConnectorList == null) ConnectorList = new List<GameObject>();
        ConnectorList.Clear();
        foreach (var connector in ConnectorHolder.GetComponentsInChildren<RectTransform>())
            ConnectorList.Add(connector.gameObject);

        // Gán ID cho từng kỹ năng
        for (var i = 0; i < SkillList.Count; i++)
        {
            SkillList[i].id = i;
        }

        // Thiết lập các kỹ năng liên kết (ví dụ)
        SkillList[0].ConnectedSkills = new[] { 1, 3, 6 };         //3 Kĩ năng đầu
        SkillList[1].ConnectedSkills = new[] { 2 };               //Nâng cấp của nó
        SkillList[3].ConnectedSkills = new[] { 4 };               //Nâng cấp của nó
        SkillList[4].ConnectedSkills = new[] { 5 };               //Nâng cấp kĩ năng mới
        SkillList[6].ConnectedSkills = new[] { 7 };               //Nâng cấp của nó
        SkillList[7].ConnectedSkills = new[] { 8 };               //Nâng cấp kĩ năng mới


        // Cập nhật giao diện
        UpdateAllSkillUI();
    }

    private void Update()
    {
        TextSkillPoint.text = $"{SkillPoint}";
        UpdateAllSkillUI();
    }

    public void UpdateAllSkillUI()
    {
        foreach (var skill in SkillList)
        {
            skill.UpdateUI();
        }
    }
}
