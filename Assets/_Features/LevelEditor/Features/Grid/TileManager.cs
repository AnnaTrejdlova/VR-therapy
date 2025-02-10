using System;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : Singleton<TileManager> {

    public GameObject TilePrefab;
    public List<TileInteractionStrategyEntry> tileInteractionStrategyEntries = new List<TileInteractionStrategyEntry>();
    public int GridSize = 10; // tileCount x tileCount gridsize
    private float _gridYpos = 0f;
    private float _tileSize = 1f;
    private List<Tile> _tiles = new List<Tile>();
    private Dictionary<Vector2Int, Tile> _tileDictionary = new Dictionary<Vector2Int, Tile>();
    private int _yMax;
    private int _xMax;

    protected override void Awake() {
        base.Awake();

        SetUpGrid(GridSize, GridSize);
    }

    public List<Tile> GetTilesInLine(Vector2Int start, Vector2Int end) {
        List<Tile> tilesToReturn = new List<Tile>();

        Vector2Int startPos = start;
        Vector2Int endPos = end;

        int xDistance = Mathf.Abs(endPos.x - startPos.x);
        int yDistance = Mathf.Abs(endPos.y - startPos.y);

        if (xDistance != 0 && yDistance != 0) {
            // Not in the same row or column; snap to closest valid position
            if (xDistance > yDistance) {
                // Snap to the same row
                endPos = new Vector2Int(endPos.x, startPos.y);
            } else {
                // Snap to the same column
                endPos = new Vector2Int(startPos.x, endPos.y);
            }
        }

        int minX = Mathf.Min(startPos.x, endPos.x);
        int maxX = Mathf.Max(startPos.x, endPos.x);
        int minY = Mathf.Min(startPos.y, endPos.y);
        int maxY = Mathf.Max(startPos.y, endPos.y);

        // Determine line orientation
        bool isHorizontal = startPos.y == endPos.y;
        bool isVertical = startPos.x == endPos.x;

        foreach (Tile tile in _tiles) {
            Vector2Int tilePos = tile.GetGridPosition();

            if (isHorizontal && tilePos.y == startPos.y && tilePos.x >= minX && tilePos.x <= maxX) {
                tilesToReturn.Add(tile);
            } else if (isVertical && tilePos.x == startPos.x && tilePos.y >= minY && tilePos.y <= maxY) {
                tilesToReturn.Add(tile);
            }
        }

        if (startPos.x > endPos.x || startPos.y > endPos.y)
            tilesToReturn.Reverse();

        return tilesToReturn;
    }

    public List<Tile> GetTilesInLine(Tile start, Tile end) {
        List<Tile> tilesToReturn = new List<Tile>();

        Vector2Int startPos = start.GetGridPosition();
        Vector2Int endPos = end.GetGridPosition();

        return GetTilesInLine(startPos, endPos);
    }


    # region Tile interaction strategy

    public void TileClickHandle(Tile tile) {
        FindStrategy(LevelEditorManager.Instance.GetState())?.OnTileClick(tile);
    }

    public void TileHoverEnterHandle(Tile tile) {
        FindStrategy(LevelEditorManager.Instance.GetState())?.OnTileHover(tile);
    }

    public void TileHoverExitHandle(Tile tile) {
        FindStrategy(LevelEditorManager.Instance.GetState())?.OnTileUnhover(tile);
    }
    public void TileClickHandle(Tile tile, TileWallPosition position) {
        FindStrategy(LevelEditorManager.Instance.GetState())?.OnTileClick(tile, position);
    }

    public void TileHoverEnterHandle(Tile tile, TileWallPosition position) {
        FindStrategy(LevelEditorManager.Instance.GetState())?.OnTileHover(tile, position);
    }

    public void TileHoverExitHandle(Tile tile, TileWallPosition position) {
        FindStrategy(LevelEditorManager.Instance.GetState())?.OnTileUnhover(tile, position);
    }

    #endregion

    #region Grid setup

    public void SetUpGrid(int width, int height) {
        _xMax = width;
        _yMax = height;
        for (int i = 0; i <= width; i++) {
            for (int j = 0; j <= height; j++) {
                Vector3 position = new Vector3(i * _tileSize, _gridYpos, j * _tileSize);
                SpawnTile(i, j, position);
            }
        }
    }

    void SpawnTile(int x, int y, Vector3 position) {
        Tile tile = Instantiate(TilePrefab, position, Quaternion.identity).GetComponent<Tile>();
        tile.SetGridPosition(new Vector2Int(x, y));
        _tileDictionary[new Vector2Int(x, y)] = tile;
        _tiles.Add(tile);
    }

    #endregion

    # region Support functions

    TileInteractionStrategy FindStrategy(EditorState state) {
        if (state == EditorState.None) return null;
        foreach (var strat in tileInteractionStrategyEntries) {
            if (strat.state == state) {
                return strat.GetStrategy();
            }
        }
         
        print("no strategy found for this editor state!");
        return null;
    }

    #endregion

    #region Helpers

    public List<Tuple<Tile, TileWallPosition>> GetNeighbourTiles(Tile centerTile, TileWallPosition prefabPosition)
    {
        List<Tuple<Tile, TileWallPosition>> neighbourTiles = new List<Tuple<Tile, TileWallPosition>>();
        Vector2Int gridPosition = centerTile.GetGridPosition();

        // Top Left joint
        if (prefabPosition == TileWallPosition.TopLeft)
        {
            // Left tile
            if (gridPosition.x - 1 >= 0)
            {
                neighbourTiles.Add(new Tuple<Tile, TileWallPosition>(
                    _tileDictionary[new Vector2Int(gridPosition.x - 1, gridPosition.y)],
                    TileWallPosition.TopRight));
            }

            // UpperLeft tile
            if (gridPosition.x - 1 >= 0 && gridPosition.y + 1 <= _yMax)
            {
                neighbourTiles.Add(new Tuple<Tile, TileWallPosition>(
                    _tileDictionary[new Vector2Int(gridPosition.x - 1, gridPosition.y + 1)],
                    TileWallPosition.BottomRight));
            }

            // Upper tile
            if (gridPosition.y + 1 <= _yMax)
            {
                neighbourTiles.Add(new Tuple<Tile, TileWallPosition>(
                _tileDictionary[new Vector2Int(gridPosition.x, gridPosition.y + 1)],
                TileWallPosition.BottomLeft));
            }

            return neighbourTiles;
        }

        // Top Right joint
        if (prefabPosition == TileWallPosition.TopRight)
        {
            // Right tile
            if (gridPosition.x + 1 <= _xMax)
            {
                neighbourTiles.Add(new Tuple<Tile, TileWallPosition>(
                _tileDictionary[new Vector2Int(gridPosition.x + 1, gridPosition.y)],
                TileWallPosition.TopLeft));
            }

            // UpperRight tile
            if (gridPosition.x + 1 <= _xMax && gridPosition.y + 1 <= _yMax)
            {
                neighbourTiles.Add(new Tuple<Tile, TileWallPosition>(
                _tileDictionary[new Vector2Int(gridPosition.x + 1, gridPosition.y + 1)],
                TileWallPosition.BottomLeft));
            }

            // Upper tile
            if (gridPosition.y + 1 <= _yMax)
            {
                neighbourTiles.Add(new Tuple<Tile, TileWallPosition>(
                _tileDictionary[new Vector2Int(gridPosition.x, gridPosition.y + 1)],
                TileWallPosition.BottomRight));
            }

            return neighbourTiles;
        }

        // Bottom Left joint
        if (prefabPosition == TileWallPosition.BottomLeft)
        {
            // Left tile
            if (gridPosition.x - 1 >= 0) {
                neighbourTiles.Add(new Tuple<Tile, TileWallPosition>(
                _tileDictionary[new Vector2Int(gridPosition.x - 1, gridPosition.y)],
                TileWallPosition.BottomRight));
            }

            // LowerLeft tile
            if (gridPosition.x - 1 >= 0 && gridPosition.y - 1 >= 0) {
                neighbourTiles.Add(new Tuple<Tile, TileWallPosition>(
                _tileDictionary[new Vector2Int(gridPosition.x - 1, gridPosition.y - 1)],
                TileWallPosition.TopRight));
            }

            // Lower tile
            if (gridPosition.y - 1 >= 0) {
                neighbourTiles.Add(new Tuple<Tile, TileWallPosition>(
                _tileDictionary[new Vector2Int(gridPosition.x, gridPosition.y - 1)],
                TileWallPosition.TopLeft));
            }

            return neighbourTiles;
        }

        // Bottom Right joint
        if (prefabPosition == TileWallPosition.BottomRight)
        {
            // Right tile
            if (gridPosition.x + 1 <= _xMax) {
                neighbourTiles.Add(new Tuple<Tile, TileWallPosition>(
                _tileDictionary[new Vector2Int(gridPosition.x + 1, gridPosition.y)],
                TileWallPosition.BottomLeft));
            }

            // LowerRight tile
            if (gridPosition.x + 1 <= _xMax && gridPosition.y - 1 >= 0) {
                neighbourTiles.Add(new Tuple<Tile, TileWallPosition>(
                _tileDictionary[new Vector2Int(gridPosition.x + 1, gridPosition.y - 1)],
                TileWallPosition.TopLeft));
            }

            // Lower tile
            if (gridPosition.y - 1 >= 0) {
                neighbourTiles.Add(new Tuple<Tile, TileWallPosition>(
                _tileDictionary[new Vector2Int(gridPosition.x, gridPosition.y - 1)],
                TileWallPosition.TopRight));
            }

            return neighbourTiles;
        }

        // Left wall
        if (prefabPosition == TileWallPosition.Left)
        {
            // Left tile, Right wall position
            if (gridPosition.x - 1 >= 0)
            {
                neighbourTiles.Add(new Tuple<Tile, TileWallPosition>(
                _tileDictionary[new Vector2Int(gridPosition.x - 1, gridPosition.y)],
                TileWallPosition.Right));
            }

            return neighbourTiles;
        }

        // Right wall
        if (prefabPosition == TileWallPosition.Right)
        {
            // Right tile, Left wall position
            if (gridPosition.x + 1 <= _xMax)
            {
                neighbourTiles.Add(new Tuple<Tile, TileWallPosition>(
                _tileDictionary[new Vector2Int(gridPosition.x + 1, gridPosition.y)],
                TileWallPosition.Left));
            }

            return neighbourTiles;
        }

        // Top wall
        if (prefabPosition == TileWallPosition.Top)
        {
            // Upper tile, Bottom wall position
            if (gridPosition.y + 1 <= _yMax)
                {
                neighbourTiles.Add(new Tuple<Tile, TileWallPosition>(
                _tileDictionary[new Vector2Int(gridPosition.x, gridPosition.y + 1)],
                TileWallPosition.Bottom));
            }

            return neighbourTiles;
        }

        // Bottom wall
        if (prefabPosition == TileWallPosition.Bottom)
        {
            // Lower tile, Top wall position
            if (gridPosition.y - 1 >= 0)
                {
                neighbourTiles.Add(new Tuple<Tile, TileWallPosition>(
                _tileDictionary[new Vector2Int(gridPosition.x, gridPosition.y - 1)],
                TileWallPosition.Top));
            }

            return neighbourTiles;
        }

        throw new Exception("Error in determining neighbours of tile");
    }

    #endregion
}

// Class for being able to assign strategies in editor

[Serializable]
public class TileInteractionStrategyEntry {
    public EditorState state;
    public MonoBehaviour strategy;

    public TileInteractionStrategy GetStrategy() {
        return strategy as TileInteractionStrategy;
    }
}
