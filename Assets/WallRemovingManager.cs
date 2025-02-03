using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRemovingManager: Singleton<WallRemovingManager> {
    /// <summary>
    /// Manages logic for deleting the walls, like click and drag for delete.
    /// </summary>

    bool _isDragDeleteActive = false;
    bool _isMouseHeldDown = false;

    void Update() {
        // Old input system
        _isMouseHeldDown = Input.GetMouseButton(0);
        if (_isMouseHeldDown && !_isDragDeleteActive) {
            BeginDragDelete();
        }
        if(!_isMouseHeldDown && _isDragDeleteActive) {
            StopDragDelete();
        }
    }

    public void BeginDragDelete() {
        _isDragDeleteActive = true;
    }

    public void StopDragDelete() {
        _isDragDeleteActive = false;
    }

    public bool GetDragDelete() {
        return _isDragDeleteActive;
    }

    
   
}
