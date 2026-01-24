using System.Collections.Generic;
using Core.Utility;

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
            //relics = SheetDataManager.Inst.GetRelicList.NonDupRandomT(1);
        }

        public void ExcuteAllRelic()
        {
            for(int i = 0; i < relics.Count; i++)
            {
                relics[i].Excute();
            }
        }
    }
}