using System.Numerics;

public interface ITileInteractionStrategy {
    public void OnTileClick(Tile tile) {

    }

    public void OnTileClick(Tile tile, Vector2 orientation) {

    }

    public void OnTileHover(Tile tile) {

    }

    public void OnTileHover(Tile tile, Vector2 orientation) {

    }

    public void OnTileUnhover(Tile tile) {

    }

    public void OnTileUnhover(Tile tile, Vector2 orientation) {

    }
}

/*
 * 
 Pokud by se sem muselo přidat dalších 300 polymorfismů tak je lepší tenhle approach:

public class TileInteractionContext {
    public Tile Tile { get; set; }
    public Vector2? Orientation { get; set; }
    public Vector3? ExtraParameter1 { get; set; }
    public string ExtraParameter2 { get; set; }
    // Add more parameters as needed
}

public interface ITileInteractionStrategy {
    void OnTileClick(TileInteractionContext context);
    void OnTileHover(TileInteractionContext context);
    void OnTileUnhover(TileInteractionContext context);
}
 *
 */
