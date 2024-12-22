using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRemovingStrategy: TileInteractionStrategy {

    // IT will actually work by clicking directly on the wall!!!

    public override void OnTileClick(Tile tile, TileWallPosition position) {
        
    }

    
    public override void OnTileHover(Tile tile, TileWallPosition position) {
        //  hoveredTile.ToggleHighlightMaterial(true);

    }

    
    public override void OnTileUnhover(Tile tile) {
        //   hoveredTile.ToggleHighlightMaterial(false);
    }
}
