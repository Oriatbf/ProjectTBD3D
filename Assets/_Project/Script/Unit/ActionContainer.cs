using System;
using SkillData;
using UnityEngine;

public class ActionContainer
{
    public Action<SkillContext> attackAction; //데미지를 줄 때
    public Action<SkillContext> hurtAction; // 데미지를 받을 때
    public Action<SkillContext> healAction; //힐을 받을 때
    public Action turnStartAction; //턴 시작할 때
    public Action turnEndAction; // 턴 끝날 때
}
