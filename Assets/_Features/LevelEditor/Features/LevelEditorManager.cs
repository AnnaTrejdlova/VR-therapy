using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelEditorManager : Singleton<LevelEditorManager> {

    [System.Serializable]
    public class EditorStateChangedEvent : UnityEvent<EditorState> { }

    private EditorState currentState;

    // UnityEvent to broadcast the state change
    public EditorStateChangedEvent OnStateChanged = new EditorStateChangedEvent();

    protected override void Awake() {
        base.Awake();

        ChangeState(EditorState.PlacingObjects);
    }

    public void ChangeState(EditorState state) {
        if (currentState != state) {
            currentState = state;
            OnStateChanged.Invoke(currentState); // Trigger the event
        }
    }

    public EditorState GetState() {
        return currentState;
    }
}

public enum EditorState {
    PlacingObjects,
    RemovingObjects,
    PlacingWalls,
    RemovingWalls,
    PlacingFloors,
    None,
}
