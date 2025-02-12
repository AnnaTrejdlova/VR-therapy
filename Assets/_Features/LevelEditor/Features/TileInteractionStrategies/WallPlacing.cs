using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WallPlacing: TileInteractionStrategy {

    /// <summary>
    /// Wall placing logic
    /// </summary>

    private GameObject _wallJointPrefab;
    private GameObject _wallFillPrefab;
    private Material _wallMaterial;

    private bool _placingWall = false;
    private Tile _wallPlacingStartingTile;
    private TileWallPosition _wallPlacingStartingPosition;
    private List<Tile> _tilesLine;

    void Awake() {
        _wallJointPrefab = WallManager.Instance.WallJointPrefab;
        _wallFillPrefab = WallManager.Instance.WallFillPrefab;
        _wallMaterial = WallManager.Instance.WallMaterial;
    }

    /// <summary>
    /// Calling when clicked on <see cref="TileWallClickable"/> on an appropriate Tile
    /// </summary>
    /// <param name="tile">Parrent <see cref="Tile"/></param>
    /// <param name="position"><paramref name="clickedTile"/> position in <paramref name="tile"/></param>
    public override void OnTileClick(Tile tile, TileWallPosition position) {
        if (!_placingWall) {
            _placingWall = true;
            _wallPlacingStartingTile = tile;
            _wallPlacingStartingPosition = position;
            tile.ClearWallPreviews();
            ShowWallsPreview(tile, position);
        } else {
            CreatePreviewedWalls();
            _placingWall = false;
        }
    }

    /// <summary>
    /// Calling when hover entered over <see cref="TileWallClickable"/> on an appropriate Tile
    /// </summary>
    /// <param name="tile">Parrent <see cref="Tile"/></param>
    /// <param name="position"><paramref name="clickedTile"/> position in <paramref name="tile"/></param>
    public override void OnTileHover(Tile tile, TileWallPosition position) {
        if (_placingWall) {
            ShowWallsPreview(tile, position);
        } else {
            tile.AddWallJointPreview(_wallJointPrefab, position);
        }
    }

    /// <summary>
    /// Calling when hover exited on <see cref="TileWallClickable"/> on an appropriate Tile
    /// </summary>
    /// <param name="tile">Parrent <see cref="Tile"/></param>
    /// <param name="position"><paramref name="clickedTile"/> position in <paramref name="tile"/></param>
    public override void OnTileUnhover(Tile tile) {
        if(!_placingWall)
            tile.ClearWallPreviews();
    }

    #region Create walls / clear wall preview
    private void CreatePreviewedWalls() {
        foreach (Tile tile in _tilesLine) {
            tile.CreateWallsBasedOnPreview(_wallMaterial);
        }
    }
    private void ClearWallPreviews() {
        foreach (Tile tile in _tilesLine) {
            tile.ClearWallPreviews();
        }
    }

    #endregion

    #region Wall building algorithm

    private void ShowWallsPreview(Tile lastHoveredTile, TileWallPosition endPosition) {
        if (_tilesLine != null && _tilesLine.Count > 0)
            ClearWallPreviews();

        // Get all tiles in the line
        _tilesLine = TileManager.Instance.GetTilesInLine(_wallPlacingStartingTile, lastHoveredTile);

        // Iterate through the tiles in the line
        for (int i = 0; i < _tilesLine.Count; i++) {
            Tile currentTile = _tilesLine[i];
            Tile endTile = _tilesLine.Last();
            Direction placementDirection;

            // First tile: Add starting joint, or full wall on multiple tiles
            if (i == 0) {
                Vector2Int startTilePosition;
                Vector2Int endTilePosition;
                if (_tilesLine.Count() > 1) {
                    startTilePosition = _wallPlacingStartingTile.GetGridPosition();
                    endTilePosition = _tilesLine.Last().GetGridPosition();
                } else {
                    startTilePosition = _tilesLine[0].clickedTile.GetGridPosition();
                    endTilePosition = _tilesLine[0].hoveredTile.GetGridPosition();
                }

                placementDirection = GetDirectionFirstTile(startTilePosition, endTilePosition);
                TileWallPosition[] jointOrientations = DetermineJointPositions(placementDirection);

                if (!ContainsPrefab(currentTile, jointOrientations[0])) {
                    currentTile.AddWallJointPreview(_wallJointPrefab, jointOrientations[0]);
                }

                if (placementDirection != Direction.Null) {
                    TileWallPosition fillPosition = DetermineFillPosition(placementDirection);
                    if (!ContainsPrefab(currentTile, fillPosition)) {
                        currentTile.AddWallFillPreview(_wallFillPrefab, fillPosition);
                    }

                    if (!ContainsPrefab(currentTile, jointOrientations[1])) {
                        currentTile.AddWallJointPreview(_wallJointPrefab, jointOrientations[1]);
                    }
                }
            }
            // Middle tiles: Add fill and both joints
            else {
                placementDirection = GetDirection(_wallPlacingStartingTile.GetGridPosition(), endTile.GetGridPosition());
                TileWallPosition fillPosition = DetermineFillPosition(placementDirection);

                TileWallPosition[] jointOrientations = DetermineJointPositions(placementDirection);

                if (!ContainsPrefab(currentTile, fillPosition)) {
                    currentTile.AddWallFillPreview(_wallFillPrefab, fillPosition);
                }

                if (!ContainsPrefab(currentTile, jointOrientations[0])) {
                    currentTile.AddWallJointPreview(_wallJointPrefab, jointOrientations[0]);
                }

                if (!ContainsPrefab(currentTile, jointOrientations[1])) {
                    currentTile.AddWallJointPreview(_wallJointPrefab, jointOrientations[1]);
                }
            }
        }
    }

    #endregion

    #region Checking free space for prefab

    /// <summary>
    /// The Tiles have common space for joints and wall fills. <br/>
    /// Method checks if the common position is not already occupied
    /// </summary>
    /// <param name="centerTile">Checking <see cref="Tile"/> on which the prefab will be created</param>
    /// <param name="prefabPosition">Checking position on which the prefab will be created</param>
    /// <returns>True if some neighbour <see cref="Tile"/> wall or joint already occupy the position</returns>
    private bool NeighbourTilesContainsPreview(Tile centerTile, TileWallPosition prefabPosition) {
        List<Tuple<Tile, TileWallPosition>> tileNeighbours = TileManager.Instance.GetNeighbourTiles(centerTile, prefabPosition);

        foreach (var tuple in tileNeighbours) {
            if (tuple.Item1.ContainsWall(tuple.Item2) || tuple.Item1.ContainsPreview(tuple.Item2)) {
                return true;
            }
        }

        return false;
    }

    private bool ContainsPrefab(Tile currentTile, TileWallPosition prefabPosition) {
        if (currentTile.ContainsWall(prefabPosition)) {
            return true;
        }
        else if (NeighbourTilesContainsPreview(currentTile, prefabPosition)) {
            return true;
        }
        return false;
    }

    #endregion

    #region Determine tile position of prefab

    /// <summary>
    /// Find and return tile position, at which a wall preview can be created, according to the delta in GridPosition
    /// </summary>
    private TileWallPosition DetermineFillPosition(Direction placementDirection) {
        Vector2Int clickTileGridPosition = _wallPlacingStartingTile.clickedTile.GetGridPosition();
        // Horizontal
        if (placementDirection == Direction.Left || placementDirection == Direction.Right) {
            // Bottom
            if (clickTileGridPosition.y == 0) {
                return TileWallPosition.Bottom;
            }

            // Top
            return TileWallPosition.Top;
        }

        // Vertical
        if (placementDirection == Direction.Up || placementDirection == Direction.Down) {
            // Left
            if (clickTileGridPosition.x == 0) {
                return TileWallPosition.Left;
            }

            // Right
            return TileWallPosition.Right;
        }

        throw new Exception("Error in determinating position of wall");
    }

    /// <summary>
    /// Find and return the corner positions, at which a joint previews can be created, according to the delta in GridPosition
    /// </summary>
    private TileWallPosition[] DetermineJointPositions(Direction placementDirection) {
        TileWallPosition[] returnArr = new TileWallPosition[2];
        Vector2Int clickTileGridPosition = _wallPlacingStartingTile.clickedTile.GetGridPosition();

        // Horizontal
        if (placementDirection == Direction.Left) {
            // Bottom
            if (clickTileGridPosition.y == 0) {
                returnArr[0] = TileWallPosition.BottomLeft;
                returnArr[1] = TileWallPosition.BottomRight;
                return returnArr;
            }

            // Top
            returnArr[0] = TileWallPosition.TopLeft;
            returnArr[1] = TileWallPosition.TopRight;
            return returnArr;
        }

        // Horizontal
        if (placementDirection == Direction.Right) {
            // Bottom
            if (clickTileGridPosition.y == 0) {
                returnArr[0] = TileWallPosition.BottomLeft;
                returnArr[1] = TileWallPosition.BottomRight;
                return returnArr;
            }

            // Top
            returnArr[0] = TileWallPosition.TopLeft;
            returnArr[1] = TileWallPosition.TopRight;
            return returnArr;
        }

        // Vertical Up
        if (placementDirection == Direction.Up) {
            // Left
            if (clickTileGridPosition.x == 0) {
                returnArr[0] = TileWallPosition.BottomLeft;
                returnArr[1] = TileWallPosition.TopLeft;
                return returnArr;
            }

            // Right
            returnArr[0] = TileWallPosition.BottomRight;
            returnArr[1] = TileWallPosition.TopRight;
            return returnArr;
        }

        // Vertical Down
        if (placementDirection == Direction.Down) {
            // Left
            if (clickTileGridPosition.x == 0) {
                returnArr[0] = TileWallPosition.TopLeft;
                returnArr[1] = TileWallPosition.BottomLeft;
                return returnArr;
            }

            // Right
            returnArr[0] = TileWallPosition.TopRight;
            returnArr[1] = TileWallPosition.BottomRight;
            return returnArr;
        }

        // Back to start
        if (placementDirection == Direction.Null) {
            returnArr[0] = _wallPlacingStartingPosition;
            return returnArr;
        }

        throw new Exception("Error in determinating position of joint");
    }

    /// <summary>
    /// Return <see cref="Direction"/> according to the delta of <paramref name="startTilePosition"/> and <paramref name="endTilePosition"/> in grid
    /// </summary>
    private Direction GetDirection(Vector2Int startTilePosition, Vector2Int endTilePosition) {
        Vector2Int delta = endTilePosition - startTilePosition;

        if (delta.x > 0) {
            return Direction.Right;
        }

        if (delta.x < 0) {
            return Direction.Left;
        }

        if (delta.y > 0) {
            return Direction.Up;
        }

        if (delta.y < 0) {
            return Direction.Down;
        }

        return Direction.Null;
    }

    /// <summary>
    /// Return <see cref="Direction"/> of the first tile for the purpose of determining whether to create wall and second joint on a first tile
    /// </summary>
    /// <param name="startTilePosition">Grid position of <see cref="TileWallClickable"/>, child of the first tile</param>
    /// <param name="endTilePosition">Grid position of <see cref="TileWallClickable"/>, child of the first tile</param>
    private Direction GetDirectionFirstTile(Vector2Int startTilePosition, Vector2Int endTilePosition) {
        Direction direction = GetDirection(startTilePosition, endTilePosition);
        Vector2Int firstTilePosition = _wallPlacingStartingTile.clickedTile.positionInTile;

        if (direction == Direction.Right && firstTilePosition.x != 1)
            return Direction.Right;
        if (direction == Direction.Left && firstTilePosition.x != 0)
            return Direction.Left;
        if (direction == Direction.Up && firstTilePosition.y != 1)
            return Direction.Up;
        if (direction == Direction.Down && firstTilePosition.y != 0)
            return Direction.Down;

        return Direction.Null;
    }

    private enum Direction {
        Up,
        Down,
        Left,
        Right,
        Null
    }

    #endregion
}
