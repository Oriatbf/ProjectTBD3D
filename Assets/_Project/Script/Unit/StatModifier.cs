using System;
using UnityEngine;

public enum EStatModifier
{
    Add,
    Multiply,
}

public class StatModfier : MonoBehaviour
{
    [Serializable]
    public readonly struct StatModifier
    {
        public readonly EStatModifier ModifierType;
        public readonly float Value;
        public readonly int SetterID;

        public StatModifier(EStatModifier modifierType, float value, int setterID)
        {
            ModifierType = modifierType;
            Value = value;
            SetterID = setterID;
        }
    }
}
