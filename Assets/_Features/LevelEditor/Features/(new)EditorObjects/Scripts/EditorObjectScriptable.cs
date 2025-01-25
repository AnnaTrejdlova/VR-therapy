using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EditorAssets/NewEditorObject")]
public class EditorObjectScriptable: ScriptableObject {
    // Generic info about the object

    public string Name;

    // Category
    public EditorObjectCategory EditorObjectType;
    [ShowIf("IsFurniture")]
    public FurnitureSubcategory furnitureSubcategory;
    [ShowIf("IsBuilding")]
    public BuildingSubcategory buildingSubcategory;
#pragma warning disable IDE0051 // Remove unused private members
    private bool IsFurniture() => EditorObjectType == EditorObjectCategory.Furniture;
    private bool IsBuilding() => EditorObjectType == EditorObjectCategory.Building;
#pragma warning restore IDE0051 // Remove unused private members

    public GameObject Model;
    [Tooltip("Model variant texture")]
    public Texture2D Texture;
    [Tooltip("What you can see in UI")]
    public Texture2D UITexture; // What you can see in ui

    // Dynamically resolve the active subcategory using the Subcategory class
    public Subcategory<FurnitureSubcategory, BuildingSubcategory> Subcategory =>
        EditorObjectType switch {
            EditorObjectCategory.Furniture => furnitureSubcategory,
            EditorObjectCategory.Building => buildingSubcategory,
            _ => null
        };

    // Utility for debugging or usage in-game
    public override string ToString() {
        return $"{Name} ({EditorObjectType}): {Subcategory}";
    }

#if UNITY_EDITOR
    private void OnValidate() {
        // Clear irrelevant subcategories
        if (EditorObjectType == EditorObjectCategory.Furniture) {
            buildingSubcategory = default;
        } else if (EditorObjectType == EditorObjectCategory.Building) {
            furnitureSubcategory = default;
        }
    }
#endif
}

public enum EditorObjectCategory {
    Furniture,
    Building
}

public enum FurnitureSubcategory {
    LivingRoom,
    Bathroom,
    Kitchen
}

public enum BuildingSubcategory {
    Wall,
    Floor,
    Door,
    Window
}

// Subcategory can hold either FurnitureSubcategory or BuildingSubcategory (Discriminated Union)
// Could be divided like before (Git history) to "remember" the last category in a different tab for a better UX
[Serializable]
public sealed class Subcategory<FurnitureSubcategory, BuildingSubcategory> {
    private object _activeCategory;

    private Subcategory(object category) {
        _activeCategory = category;
    }

    public object Category => _activeCategory;

    public static implicit operator Subcategory<FurnitureSubcategory, BuildingSubcategory>(FurnitureSubcategory furniture)
        => new Subcategory<FurnitureSubcategory, BuildingSubcategory>(furniture);

    public static implicit operator Subcategory<FurnitureSubcategory, BuildingSubcategory>(BuildingSubcategory building)
        => new Subcategory<FurnitureSubcategory, BuildingSubcategory>(building);

    public override string ToString() {
        return _activeCategory switch {
            FurnitureSubcategory furniture => furniture.ToString(),
            BuildingSubcategory building => building.ToString(),
            _ => "Unknown Category"
        };
    }
}


[Serializable]
public struct Category {
    public EditorObjectCategory EditorObjectCategory;
    public Subcategory<FurnitureSubcategory, BuildingSubcategory> Subcategory;

    public Category(EditorObjectCategory editorObjectCategory, Subcategory<FurnitureSubcategory, BuildingSubcategory> subcategory) {
        EditorObjectCategory = editorObjectCategory;
        Subcategory = subcategory;
    }

    public override string ToString() {
        return $"{EditorObjectCategory}: {Subcategory}";
    }
}
