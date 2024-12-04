using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileWallClickable : MonoBehaviour, IClickable {

    public TileWallPosition position;
    public Material HighlightMaterial;

    Material BaseMaterial;
    MeshRenderer meshRenderer;
    Outline outline;
    public Tile relatedTile;
    public Vector2Int positionInTile;

    void OnEnable() {
        outline = gameObject.GetComponent<Outline>();
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        BaseMaterial = meshRenderer.material;
        relatedTile = transform.root.GetComponent<Tile>();
        ToggleOutline(false);
    }

    public void OnClick() {
        relatedTile.clickedTile = this;
        WallManager.Instance.WallPointClick(relatedTile, this, position);
    }

    public void OnHoverEnter() {
        relatedTile.hoveredTile = this;
        WallManager.Instance.WallPointEnterHover(relatedTile, this, position);
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

    public Vector2Int GetGridPosition()
    {
        return positionInTile;
    }
}

public enum TileWallPosition {
    Null,
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight,
    Top,
    Left,
    Right,
    Bottom,
}