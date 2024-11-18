using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorHUDui : MonoBehaviour
{
    public void OnDeleteModeClick() {
        LevelEditorManager.Instance.ChangeState(EditorState.RemovingObjects);
    }
}
