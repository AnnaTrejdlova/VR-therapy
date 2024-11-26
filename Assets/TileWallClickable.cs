using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileWallClickable : MonoBehaviour, IClickable {

    public TileWallOrientation orientation;
    public Material HighlightMaterial;

    Material BaseMaterial;
    MeshRenderer meshRenderer;
    Outline outline;
    Tile relatedTile;

    void OnEnable() {
        outline = gameObject.GetComponent<Outline>();
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        BaseMaterial = meshRenderer.material;
        relatedTile = transform.root.GetComponent<Tile>();
        ToggleOutline(false);
    }

    public void OnClick() {
        WallManager.Instance.WallPointClick(relatedTile, orientation);
    }

    public void OnHoverEnter() {
        WallManager.Instance.WallPointEnterHover(relatedTile, this, orientation);
    }

    public void OnHoverExit() {
        WallManager.Instance.WallPointExitHover(relatedTile, this);
    }

    public void ToggleOutline(bool toggleOn) {
        if (outline != null) outline.enabled = toggleOn;

    }
    public void ToggleHighlightMaterial(bool toggleOn) {
        if (toggleOn) {
            meshRenderer.material = HighlightMaterial;
        } else {
            meshRenderer.material = BaseMaterial;
        }
    }
}

public enum TileWallOrientation {
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight,
    Top,
    Left,
    Right,
    Bottom,
}