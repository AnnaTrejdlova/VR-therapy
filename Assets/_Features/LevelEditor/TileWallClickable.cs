using UnityEngine;
using UnityEngine.UIElements;

public class TileWallClickable: MonoBehaviour, IClickable {

    public Material HighlightMaterial;

    private Material BaseMaterial;
    private MeshRenderer meshRenderer;
    private Outline outline;
    private Tile relatedTile;
    public Vector2Int positionInTile;
    public TileWallPosition position;

    void OnEnable() {
        outline = gameObject.GetComponent<Outline>();
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        BaseMaterial = meshRenderer.material;
        relatedTile = transform.root.GetComponent<Tile>();
        ToggleOutline(false);
    }

    public void OnClick() {
        relatedTile.clickedTile = this;
        //  WallManager.Instance.WallPointClick(relatedTile, this, position);
        TileManager.Instance.TileClickHandle(relatedTile, position);
    }

    public void OnHoverEnter() {
        relatedTile.hoveredTile = this;
        //     WallManager.Instance.WallPointEnterHover(relatedTile, this, position);
        TileManager.Instance.TileHoverEnterHandle(relatedTile, position);
    }

    public void OnHoverExit() {
        //     WallManager.Instance.WallPointExitHover(relatedTile, this);
        TileManager.Instance.TileHoverExitHandle(relatedTile, position);
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

    public Vector2Int GetGridPosition() {
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

