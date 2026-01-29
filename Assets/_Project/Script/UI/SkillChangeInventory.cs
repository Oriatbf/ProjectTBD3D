
using System;
using System.Collections.Generic;
using SkillData;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillChangeInventoryCanvas : BaseCanvas
{
    [SerializeField] private Transform skillContent;
    [SerializeField] private Transform unitContent;
    [SerializeField] private Button closeBtn;
    [SerializeField] private ChangeIcon changeIcon;
    
    private List<ChangeIcon> skillIcons = new List<ChangeIcon>();
    private List<int> curSkillIds = new List<int>();
    private List<UnitIcon> unitIcons = new List<UnitIcon>();
    
    private PointerEventData _pointerEventData;
    private List<RaycastResult> _raycastResults = new List<RaycastResult>();

    private int constId;
    private bool isTargeting = false;
    private bool isActive = false;
    private ChangeIcon curIcon,targetIcon;
    
    protected override void Awake()
    {
        base.Awake();
        _pointerEventData = new PointerEventData(EventSystem.current);
        foreach (Transform icon in skillContent)
        {
            skillIcons.Add(icon.GetComponent<ChangeIcon>());
        }

        foreach (Transform unitIcon in unitContent)
        {
            var u = unitIcon.GetComponent<UnitIcon>();
            unitIcons.Add(u);
        }
    }

    private void Initialize(int constId,List<int> skillIds)
    {
        this.constId = constId;
        curSkillIds = skillIds;
        var skillBases = SheetDataManager.Inst.GetSkillBaseList(skillIds);
        Debug.Log(skillBases.Count);
        for (int i = 0; i < skillBases.Count; i++)
        {
            Debug.Log("Skill Init");
            skillIcons[i].Init(skillBases[i]);
        }
    }
    
    private void SetUnitIcon()
    {
        var savedUnits = DataManager.Inst.GetAllSavedUnits();
        Debug.Log(savedUnits.Count);
        for (int i = 0; i < savedUnits.Count; i++)
        {
            unitIcons[i].Init(savedUnits[i]);
        }
    }

    
     private void Update()
     {
         ClickUnitIcon();

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

     #region IconSelect

      private void ClickUnitIcon()
     {
         if (!Input.GetMouseButtonDown(0) || !isActive) return;
         _pointerEventData.position = Input.mousePosition;
         _raycastResults.Clear();
         EventSystem.current.RaycastAll(_pointerEventData, _raycastResults);

         foreach (var result in _raycastResults)
         {
             if (result.gameObject.TryGetComponent(out UnitIcon unitIcon))
             {
                 if(unitIcon.GetUnitData() == null)return;
                 foreach (var u in unitIcons)
                 {
                     u.SetFrameColor(Color.white,true);
                 }
                 unitIcon.SetFrameColor(Color.green,true);
                 CancelTargeting();
                 var unitSaveData = unitIcon.GetUnitData();
                 Initialize(unitSaveData.constId, unitSaveData.bringSkills);
             }
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
                if(skillIcon.GetSkillBase() == null)return;
                curIcon = skillIcon;
                curIcon.SetFrameColor(Color.green,true);
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


     #endregion
    
    public void InitChangeSkill(SkillBase skillBase)
    {
        changeIcon.Init(skillBase);
    }

    private void ChangeSkill()
    {
        var curSkillBase = curIcon.GetSkillBase();
        var targetSkillBase = targetIcon.GetSkillBase();
        targetIcon.Init(curSkillBase);
        curIcon.Init(targetSkillBase);
        targetIcon.SetFrameColor(Color.white,true);
        curIcon.SetFrameColor(Color.white,true);
        SaveUnitSkills();
        
        
    }

    private void SaveUnitSkills()
    {
        List<int> ids = new List<int>();
        foreach (var skillIcon in skillIcons)
        {
            if(skillIcon.GetSkillBase() != null)
                ids.Add(skillIcon.GetSkillBase().GetData().ID);
        }
        DataManager.Inst.SaveUnitSkills(constId,ids);
    }

    private void CancelTargeting()
    {
        AllSkillFrameWhite();
        isTargeting = false;
    }

    private void AllSkillFrameWhite()
    {
        foreach (var skillIcon in skillIcons)
        {
            skillIcon.SetFrameColor(Color.white,true);
        }
        changeIcon.SetFrameColor(Color.white,true);
    }
    
    public void Show()
    {
        isActive = true;
        SetUnitIcon();
        ChangeState(true,true,true);
    }

    public void Hide()
    {
        isActive = false;
        ChangeState(false,true,false);
    }

    public void SetCloseAction(Action closeAction)
    {
        closeBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.AddListener(()=>closeAction?.Invoke());
    }
}
