public class ObjectRemoving : TileInteractionStrategy {
    public override void OnTileClick(Tile tile) {
        tile.RemoveObjectFromTile();
    }

    public override void OnTileHover(Tile tile) {
        tile.ChangeObjectMaterial(EditorObjectManager.Instance.RemovingPreviewMaterial);
    }

    public override void OnTileUnhover(Tile tile) {
        tile.ChangeObjectMaterialToOriginal();
    }
}
