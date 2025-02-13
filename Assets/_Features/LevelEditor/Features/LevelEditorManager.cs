using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelEditorManager : Singleton<LevelEditorManager> {

    [System.Serializable]
    public class EditorStateChangedEvent : UnityEvent<EditorState> { }

    EditorState _currentState;
    EditorState _previousState;

    // UnityEvent to broadcast the state change
    public EditorStateChangedEvent OnStateChanged = new EditorStateChangedEvent();

    protected override void Awake() {
        base.Awake();

        ChangeState(EditorState.PlacingObjects);
    }

    public void ChangeState(EditorState state) {
        if (_currentState != state) {
            _previousState = _currentState;
            _currentState = state;
            OnStateChanged.Invoke(_currentState); // Trigger the event
        }
    }

    public void ChangeStatePrevious() {
        if (_previousState == null) return;

        ChangeState(_previousState);
    }

    public EditorState GetState() {
        return _currentState;
    }

    public EditorState GetPreviousState() {
        return _previousState;
    }
}

public enum EditorState {
    None,
    PlacingObjects,
    RemovingObjects,
    PlacingWalls,
    RemovingWalls,
    PlacingFloors,
    PreviewWalking
}
