using _Project.Script.Controller;
using Core.Utility;
using UnityEngine;

public static class TamingHelper
{
    public static float TaimgCalculator(Unit targetUnit)
    {
        if (targetUnit == null) return 0;
        if (ApplicationManager.Inst.GetModule<GameFlowController>().GetCurNodeType() == NodeType.Tutorial) return 1;
        float charms = InGameUnitInfo.GetPlayersCharms();
        var targetCharmResist = targetUnit.GetStatContainer().charmResist;
        float resist = targetCharmResist._maxValue - targetCharmResist._baseValue;
        var rate = 1 / (1 + Mathf.Exp((-(charms*3.5f -resist)/20)));
        return rate;
    }
}
