using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WallManager : Singleton<WallManager> {

    public GameObject WallJointPrefab;
    public GameObject WallFillPrefab;
    public Material WallMaterial;

    // First joint has been selected and now we are previewing path to the second joint
    private bool _placingWall = false;
    private Tile wallPlacingStartingTile;
    private TileWallPosition wallPlacingStartingPosition;
    private List<Tile> tilesLine;

    protected override void Awake() {
        base.Awake();
    }

    /// <summary>
    /// Calling when clicked on <see cref="TileWallClickable"/> on an appropriate Tile
    /// </summary>
    /// <param name="tile">Parrent <see cref="Tile"/></param>
    /// <param name="position"><paramref name="clickedTile"/> position in <paramref name="tile"/></param>
    public void WallPointClick(Tile tile, TileWallClickable tileClicked, TileWallPosition position)
    {
        if (!_placingWall)
        {
            // Start with placing
            tile.AddWallJointPreview(WallJointPrefab, position);
            _placingWall = true;
            wallPlacingStartingTile = tile;
            wallPlacingStartingPosition = position;
        }
        else
        {
            // Place the previewed wall
            CreateThePreviewWalls();
            tilesLine = null;
            _placingWall = false;
        }
    }
    public void WallPointEnterHover(Tile tile, TileWallClickable hoveredTile, TileWallPosition position)
    {
        hoveredTile.ToggleHighlightMaterial(true);

        if (_placingWall)
            ShowWallsPreview(tile, position);
    }
    public void WallPointExitHover(Tile tile, TileWallClickable hoveredTile)
    {
        hoveredTile.ToggleHighlightMaterial(false);
    }

    void CreateThePreviewWalls()
    {
        foreach(Tile tile in tilesLine)
            tile.CreateWallsBasedOnPreview(WallMaterial);
    }

    #region Crazy wall building algorithm

    void ShowWallsPreview(Tile lastHoveredTile, TileWallPosition endPosition)
    {
        if (tilesLine != null && tilesLine.Count > 0)
            ClearWallPreviews();

        // Get all tiles in the line
        tilesLine = TileManager.Instance.GetTilesInLine(wallPlacingStartingTile, lastHoveredTile);

        // Iterate through the tiles in the line
        for (int i = 0; i < tilesLine.Count; i++)
        {
            Tile currentTile = tilesLine[i];
            Tile endTile = tilesLine.Last();

            // First tile: Add starting joint
            if (i == 0)
            {
                TileWallPosition[] jointOrientations = DetermineJointOrientationWall(
                   wallPlacingStartingTile,
                   endTile
                );

                currentTile.AddWallJointPreview(WallJointPrefab, jointOrientations[0]);

                Vector2Int startTilePosition;
                Vector2Int endTilePosition;

                if (tilesLine.Count() > 1)
                {
                    startTilePosition = wallPlacingStartingTile.GetGridPosition();
                    endTilePosition = tilesLine.Last().GetGridPosition();
                }
                else
                {
                    startTilePosition = tilesLine[0].clickedTile.GetGridPosition();
                    endTilePosition = tilesLine[0].hoveredTile.GetGridPosition();
                }

                if (GetDirection(startTilePosition, endTilePosition) != Direction.Null)
                {
                    TileWallPosition fillPosition = DetermineFillOrientationWall(
                        currentTile,
                        endTile
                        );
                    currentTile.AddWallFillPreview(WallFillPrefab, fillPosition);
                    currentTile.AddWallJointPreview(WallJointPrefab, jointOrientations[1]);
                }
            }
            // Middle tiles: Add fill and both joints
            else
            {
                TileWallPosition fillPosition = DetermineFillOrientationWall(
                    wallPlacingStartingTile,
                    endTile
                );

                TileWallPosition[] jointOrientations = DetermineJointOrientationWall(
                    wallPlacingStartingTile,
                    endTile
                );

                if (!currentTile.ContainsPreview(fillPosition))
                {
                    currentTile.AddWallFillPreview(WallFillPrefab, fillPosition);
                }

                if (!tilesLine[i - 1].ContainsPreview(jointOrientations[0]))
                {
                    currentTile.AddWallJointPreview(WallJointPrefab, jointOrientations[1]);
                }

                if (!currentTile.ContainsPreview(jointOrientations[0]))
                {
                    currentTile.AddWallJointPreview(WallJointPrefab, jointOrientations[0]);
                }
            }
        }
    }

    /// <summary>
    /// Return <see cref="Direction"/> according to the delta of <paramref name="startTilePosition"/> and <paramref name="endTilePosition"/> in grid
    /// </summary>
    /// <returns></returns>
    private Direction GetDirection(Vector2Int startTilePosition, Vector2Int endTilePosition)
    {
        Vector2Int firstTilePosition = tilesLine[0].clickedTile.positionInTile;
        if (endTilePosition.x - startTilePosition.x > 0)
        {
            if (firstTilePosition.x != 1)
            {
                return Direction.Right;
            }
        }

        if (endTilePosition.x - startTilePosition.x < 0)
        {
            if (firstTilePosition.x != 0)
            {
                return Direction.Left;
            }
        }

        if (endTilePosition.y - startTilePosition.y > 0)
        {
            if (firstTilePosition.y != 1)
            {
                return Direction.Up;
            }
        }

        if (endTilePosition.y - startTilePosition.y < 0)
        {
            if (firstTilePosition.y != 0)
            {
                return Direction.Down;
            }
        }

        return Direction.Null;
    }

    /// <summary>
    /// Find and return tile position, at which a wall preview can be created, according to the delta in GridPosition
    /// </summary>
    private TileWallPosition DetermineFillOrientationWall(Tile startTile, Tile endTile)
    {
        Vector2Int startTilePosition = startTile.GetGridPosition();
        Vector2Int endTilePosition = endTile.GetGridPosition();

        if (startTilePosition == endTilePosition)
        {
            startTilePosition = startTile.clickedTile.positionInTile;
            endTilePosition = startTile.hoveredTile.positionInTile;
        }

        var delta = endTilePosition - startTilePosition;

        // Horizontal
        if (delta.x != 0)
        {
            // Bottom
            if (startTile.clickedTile.GetGridPosition().y == 0)
            {
                return TileWallPosition.Bottom;
            }

            // Top
            return TileWallPosition.Top;
        }

        // Vertical
        if (delta.y != 0)
        {
            // Left
            if (startTile.clickedTile.GetGridPosition().x == 0)
            {
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
    private TileWallPosition[] DetermineJointOrientationWall(Tile startTile, Tile endTile)
    {
        TileWallPosition[] returnArr = new TileWallPosition[2];

        Vector2Int startTilePosition = startTile.GetGridPosition();
        Vector2Int endTilePosition = endTile.GetGridPosition();

        if (startTilePosition == endTilePosition)
        {
            startTilePosition = startTile.clickedTile.positionInTile;
            endTilePosition = startTile.hoveredTile.positionInTile;
        }

        Direction delta = GetDirection(startTilePosition, endTilePosition);

        // Horizontal
        if (delta == Direction.Left)
        {
            // Bottom
            if (startTile.clickedTile.GetGridPosition().y == 0)
            {
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
        if (delta == Direction.Right)
        {
            // Bottom
            if (startTile.clickedTile.GetGridPosition().y == 0)
            {
                returnArr[0] = TileWallPosition.BottomRight;
                returnArr[1] = TileWallPosition.BottomLeft;
                return returnArr;
            }

            // Top
            returnArr[0] = TileWallPosition.TopRight;
            returnArr[1] = TileWallPosition.TopLeft;
            return returnArr;
        }

        // Vertical Up
        if (delta == Direction.Up)
        {
            // Left
            if (startTile.clickedTile.GetGridPosition().x == 0)
            {
                returnArr[0] = TileWallPosition.TopLeft;
                returnArr[1] = TileWallPosition.BottomLeft;
                return returnArr;
            }

            // Right
            returnArr[0] = TileWallPosition.TopRight;
            returnArr[1] = TileWallPosition.BottomRight;
            return returnArr;
        }

        // Vertical Down
        if (delta == Direction.Down)
        {
            // Left
            if (startTile.clickedTile.GetGridPosition().x == 0)
            {
                returnArr[0] = TileWallPosition.BottomLeft;
                returnArr[1] = TileWallPosition.TopLeft;
                return returnArr;
            }

            // Right
            returnArr[0] = TileWallPosition.BottomRight;
            returnArr[1] = TileWallPosition.TopRight;
            return returnArr;
        }

        // Back to start
        if (delta == Direction.Null)
        {
            returnArr[0] = wallPlacingStartingPosition;
            return returnArr;
        }

        throw new Exception("Error in determinating position of joint");
    }

    void ClearWallPreviews() {
        foreach(Tile tile in tilesLine) {
            tile.ClearWallPreviews();
        }
    }

    private enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        Null
    }

    #endregion
}
