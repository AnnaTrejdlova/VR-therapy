using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour, IClickable {

    public TileWallPosition orientation;
    public Tile tile;

    private Material _originalMaterial;
    private MeshRenderer _meshRenderer;

    void Awake() {
        _meshRenderer = GetComponent<MeshRenderer>();
        _originalMaterial = _meshRenderer.material;
    }

    public void OnClick() {
        if(LevelEditorManager.Instance.GetState() == EditorState.RemovingWalls) {
            if (tile == null) return;

            // Remove yourself
            tile.RemoveWall(orientation);
        }
    }

    public void OnHoverEnter() {
        if (LevelEditorManager.Instance.GetState() == EditorState.RemovingWalls) {
            if (_meshRenderer == null) return;
            // if not in remove mode
            if (WallRemovingManager.Instance.GetDragDelete()) {
                tile.RemoveWall(orientation);
                return;
            }
            _meshRenderer.material = EditorObjectManager.Instance.RemovingPreviewMaterial;
        }
    }

    public void OnHoverExit() {
        if (LevelEditorManager.Instance.GetState() == EditorState.RemovingWalls) {
            if (_meshRenderer == null) return;
            // if not in remove mode
            if (WallRemovingManager.Instance.GetDragDelete()) {
                return;
            }
            _meshRenderer.material = _originalMaterial;
        }
    }
}
