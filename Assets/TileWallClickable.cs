using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileWallClickable : MonoBehaviour, IClickable {

    public TileWallOrientation orientation;

    Outline outline;

    void OnEnable() {
        outline = gameObject.GetComponent<Outline>();
        ToggleOutline(false);
    }

    public void OnClick() {
        WallManager.Instance.WallPointClick(transform.root.GetComponent<Tile>(), orientation);
    }

    public void OnHoverEnter() {
        WallManager.Instance.WallPointEnterHover(this);
    }

    public void OnHoverExit() {
        WallManager.Instance.WallPointExitHover(this);
    }

    public void ToggleOutline(bool toggleOn) {
        if (outline != null) outline.enabled = toggleOn;

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