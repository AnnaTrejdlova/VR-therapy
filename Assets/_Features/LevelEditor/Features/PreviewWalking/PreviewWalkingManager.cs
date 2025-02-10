using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreviewWalkingManager: Singleton<PreviewWalkingManager> {

    public GameObject PlayerGameObject;

    protected override void Awake() {
        base.Awake();
    }

    void EnterWalkingMode() {
        PlayerGameObject.SetActive(true);
        Camera.main.orthographic = false;
    }

    void ExitWalkingMode() {
        PlayerGameObject.SetActive(false);
        Camera.main.orthographic = true;
    }

    #region Event Listener

    void HandleStateChange(EditorState newState) {
        if (newState == EditorState.PreviewWalking) {
            EnterWalkingMode();
        } else {
            ExitWalkingMode();
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
