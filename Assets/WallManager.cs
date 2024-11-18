using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallManager : Singleton<WallManager> {

    public GameObject WallJointPrefab;
    public GameObject WallJointFill;

    protected override void Awake() {
        base.Awake();

    }

    public void WallPointClick(Tile tile, TileWallOrientation orientation) {
        tile.AddWallJoint(WallJointPrefab, orientation);
    }
    public void WallPointEnterHover(Tile tile, TileWallOrientation orientation) {

    }
    public void WallPointExitHover(Tile tile, TileWallOrientation orientation) {

    }


}
