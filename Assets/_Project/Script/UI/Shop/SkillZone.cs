using System.Collections.Generic;
using UnityEngine;

public class SkillZone : MonoBehaviour
{ 
    [SerializeField] private Transform content;
    private List<SkillIcon> skillIcons = new List<SkillIcon>();

    private void Awake()
    {
        foreach (Transform child in content)
        {
            skillIcons.Add(child.GetComponent<SkillIcon>());
        }
    }

    public void SetSkillIcon()
    {
        var list = SheetDataManager.Inst.GetRandomSkillBaseList(skillIcons.Count);
        for (int i = 0; i < list.Count; i++)
        {
            //skillIcons[i].InitUnitSkill(list[i]);
        }
    }
}
