using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRemoving : MonoBehaviour, ITileInteractionStrategy {
    public void OnTileClick(Tile tile) {
        RemoveObjectFromTile(tile);
    }

    public void OnTileHover(Tile tile) {
        tile.ToggleOutline(true);
    }

    public void OnTileUnhover(Tile tile) {
        tile.ToggleOutline(false);
    }

    void RemoveObjectFromTile(Tile tile) {
        if (tile.isTileOccupied()) {
            tile.RemoveObjectFromTile();
            return;
        }
    }
}
