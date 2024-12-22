using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRemoving : TileInteractionStrategy {
    public override void OnTileClick(Tile tile) {
        RemoveObjectFromTile(tile);
    }

    public override void OnTileHover(Tile tile) {
        tile.ChangeObjectMaterial(EditorObjectManager.Instance.RemovingPreviewMaterial);
    }

    public override void OnTileUnhover(Tile tile) {
        tile.ChangeObjectMaterialToOriginal();
    }

    void RemoveObjectFromTile(Tile tile) {
        if (tile.isTileOccupied()) {
            tile.RemoveObjectFromTile();
            return;
        }
    }
}
