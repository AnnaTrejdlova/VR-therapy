using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EditorObjectButton : MonoBehaviour {

    public TextMeshProUGUI text;
    GameObject objectRefference;

    public void onClick() {
        EditorObjectManager.Instance.SelectObject(objectRefference);
    }

    public void SetObjectReff(GameObject go) {
        objectRefference = go;
        text.text = go.name;
    }

}
