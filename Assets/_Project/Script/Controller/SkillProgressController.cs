using Core.Utility;
using Cysharp.Threading.Tasks;

namespace _Project.Script.Controller
{
    public class SkillProgressController : BaseController
    {
        
        SkillTurnCounter _skillTurnCounter = new SkillTurnCounter();
        SkillStack _skillStack = new SkillStack();
        public override ControllerInfo ControllerInfo { get; } = new()
        {
            ContainSceneNames = new string[] {"GamePlay" },
            Priority = 0,
            UpdateInterval = 1,
            LateUpdateInterval = 1,
            FixedUpdateInterval = 1,
        };

        public override void OnInitialize()
        {
            base.OnInitialize();
            _skillStack.SetPrefab().Forget();
            _skillTurnCounter.SetCanvas();
        }

        public void RegisterTutorial()
        {
            _skillStack.RegisterTutorial();
            _skillTurnCounter.RegisterTutorial();
        }

        public void Reset()
        {
            _skillTurnCounter.ResetAllSkillTurnCounter();
            _skillStack.ResetAllSkillStacks();
        }

        public void DeleteStack(float curStackTurn,float deleteStackTurn)
        {
            _skillTurnCounter.DequeueByTurn(curStackTurn, deleteStackTurn);
            _skillStack.UnstackSkillByTurn( curStackTurn, deleteStackTurn);
        }

        public void Stack(SkillStackInfo skillStackInfo)
        {
            _skillStack.StackSkill(skillStackInfo);
            _skillTurnCounter.Enqueue(skillStackInfo);
        }

        public void UnStack(Tile tile)
        {
            _skillStack.UnstackSkill(tile);
            _skillTurnCounter.DequeueAllByTile(tile);
        }

        public void UnStackAll(Tile tile)
        {
            _skillStack.UnstackAllUnitSkills(tile);
            _skillTurnCounter.DequeueAllByTile(tile);
        }

        public async UniTask ActionSkill()
        {
            foreach (var unit in InGameUnitInfo.EnemyUnits)unit.HideRate();
            await _skillTurnCounter.ActionSkill();
        }
        
        public SkillStack GetSkillStack() => _skillStack;
        public SkillTurnCounter GetSkillTurnCounter() => _skillTurnCounter;
    }
}