using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static SkillTree;
using UnityEngine.UI;

public class Skill : MonoBehaviour
{
    public int id;

    public TMP_Text TitleText;
    public TMP_Text DescriptionText;

    public int[] ConnectedSkills;


    public void ColorUpLevel()
    {
        GetComponent<Image>().color = skilltree.SkillLevels[id] >= skilltree.SkillCaps[id] ? Color.gray :
            skilltree.SkillPoint > 0 ? Color.green : Color.white;
    }
    public void UpdateUI()
    {
        TitleText.text = $"{skilltree.SkillLevels[id]}/{skilltree.SkillCaps[id]}\n{skilltree.SkillNames[id]}";
        DescriptionText.text = $"{skilltree.SkillDescriptions[id]}\nCost: {skilltree.SkillPoint}/1 SP";

        ColorUpLevel();

        foreach (var connectedSkill in ConnectedSkills)
        {
            skilltree.SkillList[connectedSkill].gameObject.SetActive(skilltree.SkillLevels[id] > 0);
            skilltree.ConnectorList[connectedSkill].SetActive(skilltree.SkillLevels[id] > 0);
        }
    }
    public void Buy()
    {
        if (skilltree.SkillPoint < 1 || skilltree.SkillLevels[id] >= skilltree.SkillCaps[id]) return;
        skilltree.SkillPoint -= 1;
        skilltree.SkillLevels[id]++;
        skilltree.UpdateAllSkillUI();
    }
}
