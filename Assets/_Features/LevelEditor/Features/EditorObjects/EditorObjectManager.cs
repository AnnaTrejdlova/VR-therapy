using System.Collections.Generic;
using UnityEngine;

public class EditorObjectManager : Singleton<EditorObjectManager> {

    public GameObject UIButtonPrefab;
    public GameObject UIContainerRefference;
    public List<GameObject> ModelList = new List<GameObject>();
    public Material RemovingPreviewMaterial;
    public Material PreviewMaterial;

    GameObject selectedObject;

    protected override void Awake() {
        base.Awake();
        SetUpButtons();
    }

    public void SelectObject(GameObject model) {
        selectedObject = model;
    }

    public GameObject GetSelectedObject() { 
        return selectedObject;
    }

    void SetUpButtons() {
        foreach (GameObject obj in ModelList) {
            Instantiate(UIButtonPrefab, UIContainerRefference.gameObject.transform).GetComponent<EditorObjectButton>().SetObjectReff(obj);
        }
    }
}
