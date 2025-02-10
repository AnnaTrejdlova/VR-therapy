using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager: Singleton<CameraManager> {

    public GameObject EditingCameraGameObject;


    #region Event Listener

    void HandleStateChange(EditorState newState) {
        if (newState == EditorState.PreviewWalking) {
            EditingCameraGameObject.SetActive(false);
        } else {
            EditingCameraGameObject.SetActive(true);
        }
    }

    void OnEnable() {
        LevelEditorManager.Instance.OnStateChanged.AddListener(HandleStateChange);
    }

    void OnDisable() {
        LevelEditorManager.Instance.OnStateChanged.RemoveListener(HandleStateChange);
    }

    #endregion

}
