using SkillData;
using UnityEngine;

public static class SkillExtension 
{
    public static float Calculation<T>(this SkillAttribute skillAttribute,int value,StatContainer statContainer)
    {
        float finalValue = value;
        switch (skillAttribute)
        {
            case SkillAttribute.Physical:
                finalValue += statContainer.str._baseValue;
                break;
            case SkillAttribute.Mage:
                finalValue += statContainer.intelligence._baseValue;
                break;
            case SkillAttribute.All:
                var str = statContainer.str._baseValue;
                var intelligence = statContainer.intelligence._baseValue;
                finalValue += str>intelligence ? str : intelligence;
                break;
        }
        
        return finalValue;
    }
}
