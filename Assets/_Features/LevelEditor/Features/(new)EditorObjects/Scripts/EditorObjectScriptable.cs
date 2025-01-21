using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EditorAssets/NewEditorObject")]
public class EditorObjectScriptable: ScriptableObject {
    // Generic info about the object

    public string Name;
    public EditorObjectType EditorObjectType;
    public GameObject Model;
    public Texture2D Texture;
    public Texture2D UITexture; // What you can see in ui
}

public enum EditorObjectType {
    Furniture,
    Wall,
    Door,
    Window
}
