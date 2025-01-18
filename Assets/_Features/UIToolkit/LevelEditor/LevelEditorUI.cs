using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelEditorUI : MonoBehaviour {
    private void OnEnable() {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
    }

    private void Start() {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
    }
}
