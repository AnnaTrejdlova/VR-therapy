using UnityEngine;

public class ObjectPlacing: TileInteractionStrategy {
    private bool _placingObject = false;
    public override void OnTileClick(Tile tile) {
        _placingObject = true;
        if (EditorObjectManager.Instance.GetSelectedObject() != null) {
            tile.AddObjectToTile(EditorObjectManager.Instance.GetSelectedObject());
        }
    }

    public override void OnTileHover(Tile tile) {
        _placingObject = false;
        if (EditorObjectManager.Instance.GetSelectedObject() != null) {
            tile.AddObjectPreviewToTile(EditorObjectManager.Instance.GetSelectedObject());
        }
    }

    public override void OnTileUnhover(Tile tile) {
        if (!_placingObject) {
            tile.RemoveObjectPreviewFromTile();
        }
    }
}
