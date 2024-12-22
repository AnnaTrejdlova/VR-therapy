using UnityEngine;

public abstract class TileInteractionStrategy: MonoBehaviour {
    public virtual void OnTileClick(Tile tile) { }
    public virtual void OnTileClick(Tile tile, TileWallPosition orientation) { }
    public virtual void OnTileHover(Tile tile) { }
    public virtual void OnTileHover(Tile tile, TileWallPosition orientation) { }
    public virtual void OnTileUnhover(Tile tile) { }
    public virtual void OnTileUnhover(Tile tile, TileWallPosition orientation) { }
}

