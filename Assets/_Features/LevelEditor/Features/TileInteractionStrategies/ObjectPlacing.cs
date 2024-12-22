using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacing : TileInteractionStrategy {
    public override void OnTileClick(Tile tile) {
        PlaceObjectOnTile(tile);
    }

    public override void OnTileHover(Tile tile) {
        tile.ToggleHighlightMaterial(true);
    }

    public override void OnTileUnhover(Tile tile) {
        tile.ToggleHighlightMaterial(false);
    }

    void PlaceObjectOnTile(Tile tile) {
        if (EditorObjectManager.Instance.GetSelectedObject() == null) {
            return;
        }
        tile.AddObjectToTile(EditorObjectManager.Instance.GetSelectedObject());
    }
}
