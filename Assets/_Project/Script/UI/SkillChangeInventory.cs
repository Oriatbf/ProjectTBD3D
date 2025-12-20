using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillChangeInventory : MonoBehaviour
{
    [SerializeField] private Transform skillContent;
    [SerializeField] private Panel panel;
    [SerializeField] private List<ChangeIcon> skillIcons = new List<ChangeIcon>();
    private List<int> curSkillIds = new List<int>();
    
    private PointerEventData _pointerEventData;
    private List<RaycastResult> _raycastResults = new List<RaycastResult>();

    private int constId;
    private bool isTargeting = false;
    private bool isActive = false;
    private ChangeIcon curIcon,targetIcon;
    
    private void Awake()
    {
        _pointerEventData = new PointerEventData(EventSystem.current);
        foreach (Transform icon in skillContent)
        {
            skillIcons.Add(icon.GetComponent<ChangeIcon>());
        }
    }

    public void Init(int constId,List<int> skillIds)
    {
        isActive = true;
        this.constId = constId;
        curSkillIds = skillIds;
        var skillBases = SheetDataManager.Inst.GetSkillBaseList(skillIds);
        for (int i = 0; i < skillBases.Count; i++)
        {
            skillIcons[i].Init(skillBases[i]);
        }
    }
    
     private void Update()
     {
         if (!isActive) return;
        if (!isTargeting)
        {
            HandleSkillSelection();
        }
        else
        {
            HandleSkillTargeting();
        }
            
     }
        
    /// <summary>
    /// 스킬을 클릭하여 선택
    /// </summary>
    private void HandleSkillSelection()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        _pointerEventData.position = Input.mousePosition;
        _raycastResults.Clear();
        EventSystem.current.RaycastAll(_pointerEventData, _raycastResults);

        foreach (var result in _raycastResults)
        {
            if (result.gameObject.TryGetComponent(out ChangeIcon skillIcon))
            {
                curIcon = skillIcon;
                isTargeting = true;
                return;
            }
        }
    }
    
    /// <summary>
    /// 타겟 스킬 아이콘을 선택
    /// </summary>
    private void HandleSkillTargeting()
    {
        //우클릭으로 타켓팅 취소
        if (Input.GetMouseButtonDown(1))
        {
            CancelTargeting();
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            _pointerEventData.position = Input.mousePosition;
            _raycastResults.Clear();
            EventSystem.current.RaycastAll(_pointerEventData, _raycastResults);

            foreach (var result in _raycastResults)
            {
                if (result.gameObject.TryGetComponent(out ChangeIcon skillIcon))
                {
                    targetIcon = skillIcon;
                    ChangeSkill();
                    isTargeting = false;
                    return;
                }
            }
        }

    }

    private void ChangeSkill()
    {
        var curSkillBase = curIcon.GetSkillBase();
        var targetSkillBase = targetIcon.GetSkillBase();
        targetIcon.Init(curSkillBase);
        curIcon.Init(targetSkillBase);
        SaveUnitSkills();
        
        
    }

    private void SaveUnitSkills()
    {
        List<int> ids = new List<int>();
        foreach (var skillIcon in skillIcons)
        {
            ids.Add(skillIcon.GetSkillBase().GetData().ID);
        }
        DataManager.Inst.SaveUnitSkills(constId,ids);
    }

    private void CancelTargeting()
    {
        isTargeting = false;
    }
    
    public void Show()
    {
        panel.SetPosition(PanelStates.Show,true);
    }

    public void Hide()
    {
        panel.SetPosition(PanelStates.Hide,true);
    }
}
