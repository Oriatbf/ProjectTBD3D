
using System;
using System.Collections.Generic;
using _Project.Script.Controller;
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
    private UnitIcon curUnitIcon;
    
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

    private void Start()
    {
        RegisterTutorial();
    }

    private void InitUnitSkills(int constId,List<int> skillIds)
    {
        this.constId = constId;
        curSkillIds = skillIds;
        var skillBases = SheetDataManager.Inst.GetSkillBaseList(skillIds);
        Debug.Log(skillBases.Count);
        foreach (var skillIcon in skillIcons) skillIcon.Init(null);
        
        for (int i = 0; i < skillBases.Count; i++)
        {
            Debug.Log("Skill Init");
            skillIcons[i].Init(skillBases[i]);
        }

        for (int i = skillBases.Count; i < skillIcons.Count; i++)
        {
            skillIcons[i].SetFrameColor(Color.red,true);
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
         if (_raycastResults[0].gameObject.TryGetComponent(out UnitIcon unitIcon))
         {
             SelectUnit(unitIcon);
         }
     }

     private void SelectUnit(UnitIcon unitIcon)
     {
         if(unitIcon.GetUnitData() == null)return;
         foreach (var u in unitIcons)
         {
             u.SetFrameColor(Color.white,true);
         }
         curUnitIcon = unitIcon;
         unitIcon.SetFrameColor(Color.green,true);
         CancelTargeting();
         var unitSaveData = unitIcon.GetUnitData();
         InitUnitSkills(unitSaveData.constId, unitSaveData.bringSkills);
     }
        
    /// <summary>
    /// 스킬을 클릭하여 선택
    /// </summary>
    private void HandleSkillSelection()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if(curUnitIcon == null) return;
        _pointerEventData.position = Input.mousePosition;
        _raycastResults.Clear();
        EventSystem.current.RaycastAll(_pointerEventData, _raycastResults);
        if (_raycastResults[0].gameObject.TryGetComponent(out ChangeIcon unitIcon))
                SelectSkill(unitIcon);
    }

    private void SelectSkill(ChangeIcon skillIcon)
    {
        if(skillIcon.GetSkillBase() == null)return;
        Debug.Log(skillIcon.GetSkillBase().GetData().Name);
        curIcon = skillIcon;
        curIcon.SetFrameColor(Color.green,true);
        isTargeting = true;
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
            if(curUnitIcon == null) return;
            _pointerEventData.position = Input.mousePosition;
            _raycastResults.Clear();
            EventSystem.current.RaycastAll(_pointerEventData, _raycastResults);

            if (_raycastResults[0].gameObject.TryGetComponent(out ChangeIcon skillIcon))
            {
                SelectChangedSkill(skillIcon);
            }
        }

    }

    private void SelectChangedSkill(ChangeIcon skillIcon)
    {
        if(skillIcon.GetSkillBase() == null)return;
        targetIcon = skillIcon;
        ChangeSkill();
        isTargeting = false;
        return;
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

    #region Tutorial

    private void RegisterTutorial()
    {
        SetTutorial1();
        SetTutorial2();
        SetTutorial3();
    }
    
    private void SetTutorial1()
    {
        var targetRect = unitIcons[0].GetComponent<RectTransform>();
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(targetRect);
        TutorialInfo tutorialInfo = new TutorialInfo()
        {
            order = 1,
            informationTxt = "유닛을 선택하세요",
            tutorialKey = "Loot",
            highLightRect = targetRect,
            transformType = TransformType.Rect,
            highLightSize = targetRect.sizeDelta,
            textOffset = new Vector2(0,100),
            btnAction = ()=>
            {
                SelectUnit(unitIcons[0]);
            }
        };
        ApplicationManager.Inst.GetModule<TutorialController>().SetTutorial(tutorialInfo);
    }
    
    private void SetTutorial2()
    {
        var targetRect = skillIcons[0].GetComponent<RectTransform>();
        TutorialInfo tutorialInfo = new TutorialInfo()
        {
            order = 2,
            informationTxt = "유닛의 스킬 개수를 제한되어 있습니다\n스킬을 선택하세요",
            highLightRect = targetRect,
            tutorialKey = "Loot",
            transformType = TransformType.Rect,
            highLightSize = targetRect.sizeDelta,
            textOffset = new Vector2(0,100),
            btnAction = ()=>
            {
                SelectSkill(skillIcons[0]);
            }
        };
        ApplicationManager.Inst.GetModule<TutorialController>().SetTutorial(tutorialInfo);
    }
    
    private void SetTutorial3()
    {
        var targetRect = changeIcon.GetComponent<RectTransform>();
        TutorialInfo tutorialInfo = new TutorialInfo()
        {
            order = 3,
            informationTxt = "바꿀 스킬을 선택하세요",
            highLightRect = targetRect,
            tutorialKey = "Loot",
            transformType = TransformType.Rect,
            highLightSize = targetRect.sizeDelta,
            textOffset = new Vector2(0,100),
            btnAction = ()=>
            {
                SelectChangedSkill(changeIcon);
            }
        };
        ApplicationManager.Inst.GetModule<TutorialController>().SetTutorial(tutorialInfo);
    }
    

    #endregion
}
