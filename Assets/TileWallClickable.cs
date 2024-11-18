using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileWallClickable : MonoBehaviour, IClickable {

    public TileWallOrientation orientation;

    public void OnClick() {
        WallManager.Instance.WallPointClick(transform.root.GetComponent<Tile>(), orientation);
    }

    public void OnHoverEnter() {
        WallManager.Instance.WallPointEnterHover(transform.root.GetComponent<Tile>(), orientation);
    }

    public void OnHoverExit() {
        WallManager.Instance.WallPointExitHover(transform.root.GetComponent<Tile>(), orientation);
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