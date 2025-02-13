using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerStat", menuName = "NewPlayerStat/FloatStat")]
public class PlayerStat : ScriptableObject {
    public float baseValue;
    public float currentValue;

    private List<float> modifiers = new List<float>();

    public void ResetToBase() {
        currentValue = baseValue;
    }

    public void AddModifier(float modifier) {
        modifiers.Add(modifier);
        RecalculateValue();
    }

    public void RemoveModifier(float modifier) {
        modifiers.Remove(modifier);
        RecalculateValue();
    }

    void RecalculateValue() {
        currentValue = baseValue;
        float finalModifier = 0f;
        if (modifiers.Count <= 0) {
            currentValue = baseValue;
            return;
        }
        foreach (var modifier in modifiers) {
            finalModifier += modifier;
        }
        currentValue *= finalModifier;
    }
}
