using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRemovingStrategy: TileInteractionStrategy {

    // It actually works by clicking directly on the wall

    #region TileInteractionInterface

    public override void OnTileClick(Tile tile, TileWallPosition position) {}
    public override void OnTileHover(Tile tile, TileWallPosition position) {}
    public override void OnTileUnhover(Tile tile) {}

    #endregion
}
