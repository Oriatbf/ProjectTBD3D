
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using SkillData;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;

public class SkillCheatCanvas : BaseCanvas
{
    private readonly string skillIconPath = "Assets/_Project/Prefab/UI/Skill/SkillIcon.prefab";
    [SerializeField] private Transform content;
    private List<Icon> icons = new List<Icon>();
    private Icon iconPrefab;
    private Queue<SkillBase> _selectedSkills = new Queue<SkillBase>();
    private PointerEventData _pointerEventData;
    private List<RaycastResult> _raycastResults = new List<RaycastResult>();

    private readonly int MaxSkillCnt = 4;
    private int currentSkillCnt = 0;

    protected override void Awake()
    {
        base.Awake();
        _pointerEventData = new PointerEventData(EventSystem.current);
        ChangeState(false);
        SetIconPrefab();
    }

    private async void Start()
    {
        await SetIconPrefab();
        SetAllSkills();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TBDLogger.CommandLog(KeyCode.P, this);
            if(!isShow)ChangeState(true,true,true);
            else ChangeState(false,true);
        }

        if (isShow)
        {
            SelectIcon();
            ApplySkillChange();   
        }
    }
    

    private void SelectIcon()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        _pointerEventData.position = Input.mousePosition;
        _raycastResults.Clear();
        EventSystem.current.RaycastAll(_pointerEventData, _raycastResults);

        foreach (var result in _raycastResults)
        {
            if (result.gameObject.TryGetComponent(out Icon selectedIcon))
            {
                if (currentSkillCnt == MaxSkillCnt)
                {
                    _selectedSkills.Dequeue();
                    _selectedSkills.Enqueue(selectedIcon.GetSkillBase());
                }
                else
                {
                    _selectedSkills.Enqueue(selectedIcon.GetSkillBase());
                }

                string name = "";
                foreach (var skillBase in _selectedSkills)
                {
                    name += skillBase.GetData().Name + " ";
                }
                Debug.Log($"현재 선택된 스킬은 {name}");
                currentSkillCnt = _selectedSkills.Count;
            }
        }
    }

    private void ApplySkillChange()
    {
        if (!Input.GetKeyDown(KeyCode.KeypadEnter)) return;
        var list = _selectedSkills.Select(x=>x.GetData().ID).ToList();
        FactoryManager.Inst.GetPlayerUnits()[0].SetBringSkills(list);
        Debug.Log("치트 스킬 변경");
    }

    private async UniTask SetIconPrefab()
    {
        var _skillIcon = await Addressables.LoadAssetAsync<GameObject>(skillIconPath).ToUniTask();
        if(_skillIcon.TryGetComponent(out Icon skillIcon))iconPrefab = skillIcon;
    }

    private void SetAllSkills()
    {
        if(iconPrefab == null)Debug.LogError("스킬아이콘이 없음");
        var skills = SheetDataManager.Inst.GetAllSkills();
        foreach (var skillBase in skills)
        {
            var _icon = Instantiate(iconPrefab, content);
            _icon.Init(skillBase);
            icons.Add(_icon);
        }
    }
}
