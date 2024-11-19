using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallManager : Singleton<WallManager> {

    public GameObject WallJointPrefab;
    public GameObject WallFillPrefab;

    protected override void Awake() {
        base.Awake();

    }

    public void WallPointClick(Tile tile, TileWallOrientation orientation) {
        tile.AddWallJoint(WallJointPrefab, orientation);
    }
    public void WallPointEnterHover(TileWallClickable tileWall) {
        tileWall.ToggleOutline(true);
    }
    public void WallPointExitHover(TileWallClickable tileWall) {
        tileWall.ToggleOutline(false);
    }


}
