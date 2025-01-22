using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EditorAssets/NewEditorObject")]
public class EditorObjectScriptable : ScriptableObject
{
    // Generic info about the object

    public string Name;
    public EditorObjectCategory EditorObjectType;
    public EditorObjectSubcategory EditorObjectCategory;
    public GameObject Model;
    [Tooltip("Model variant texture")]
    public Texture2D Texture;
    [Tooltip("What you can see in UI")]
    public Texture2D UITexture; // What you can see in ui
}

public enum EditorObjectCategory {
    Furniture,
    Building
}

//TEMP
public enum EditorObjectSubcategory {
    LivingRoom,
    Bathroom,
    Kitchen,
    Wall,
    Floor,
    Door,
    Window
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
    private Subcategory(object activeCategory) {
        Category = activeCategory;
    }

    public object Category { get; }

    public static implicit operator Subcategory<FurnitureSubcategory, BuildingSubcategory>(FurnitureSubcategory category) => new(category);

    public static implicit operator Subcategory<FurnitureSubcategory, BuildingSubcategory>(BuildingSubcategory category) => new(category);

    public override string ToString() {
        return Category switch {
            FurnitureSubcategory furniture => $"{furniture}",
            BuildingSubcategory building => $"{building}",
            _ => "Unknown Category"
        };
    }
}

[System.Serializable]
public struct Category {
    public EditorObjectCategory EditorObjectCategory;
    public Subcategory<FurnitureSubcategory, BuildingSubcategory> Subcategory;

    public Category(EditorObjectCategory editorObjectCategory, Subcategory<FurnitureSubcategory, BuildingSubcategory> subcategory) {
        EditorObjectCategory = editorObjectCategory;
        Subcategory = subcategory;
    }
}
