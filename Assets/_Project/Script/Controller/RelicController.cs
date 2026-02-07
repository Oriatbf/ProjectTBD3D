using System.Collections.Generic;
using Core.Utility;
using Cysharp.Threading.Tasks;

namespace _Project.Script.Controller
{
    public class RelicController : BaseController
    {
        private List<RelicBase> relics = new List<RelicBase>();
        public override ControllerInfo ControllerInfo { get; } = new()
        {
            ContainSceneNames = new string[] {"GamePlay" },
            Priority = 0,
            UpdateInterval = 0,
            LateUpdateInterval = 0,
            FixedUpdateInterval = 0,
        };

        public override void OnInitialize()
        {
            base.OnInitialize();
            var relicIds = DataManager.Inst.GetRelicSaveData().relicIDList;
            relics = SheetDataManager.Inst.GetRelicDataByIds(relicIds);
        }

        public async UniTask ExcuteAllRelic()
        {
            await UniTask.WaitForSeconds(1f);
            for(int i = 0; i < relics.Count; i++)
            {
                relics[i].Excute();
            }
            await UniTask.WaitForSeconds(0.5f);
        }
    }
}