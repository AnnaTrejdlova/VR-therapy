using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Rendering.Universal;

public class WallManager : Singleton<WallManager> {

    public GameObject WallJointPrefab;
    public GameObject WallFillPrefab;
    public Material WallMaterial;

    // First joint has been selected and now we are previewing path to the second joint
    private bool _placingWall = false; 
    Tile wallPlacingStartingTile;
    TileWallPosition wallPlacingStartingPosition;
    List<Tile> tilesLine;

    private TileWallClickable _tileClicked;
    private TileWallClickable _tileHovered;

    protected override void Awake() {
        base.Awake();
    }

    /// <summary>
    /// Calling when clicked on <see cref="TileWallClickable"/> on appropriate Tile
    /// </summary>
    /// <param name="tile">Parrent <see cref="Tile"/></param>
    /// <param name="position"><paramref name="clickedTile"/> position in <paramref name="tile"/></param>
    public void WallPointClick(Tile tile, TileWallClickable tileClicked, TileWallPosition position) {
        
        if (!_placingWall) {
            // Wall preview
            tile.AddWallJointPreview(WallJointPrefab, position);
            _tileClicked = tileClicked;
            _placingWall = true;
            wallPlacingStartingTile = tile;
            wallPlacingStartingPosition = position;
        } else {
            // Place the previewed wall
            CreateThePreviewWalls();
            tilesLine = null;
            _placingWall = false;
        }
    }
    public void WallPointEnterHover(Tile tile, TileWallClickable hoveredTile, TileWallPosition position) {
        hoveredTile.ToggleHighlightMaterial(true);
        if (_placingWall)
        {
            ShowWallsPreview(tile, position);
            _tileHovered = hoveredTile;
        }
    }
    public void WallPointExitHover(Tile tile, TileWallClickable hoveredTile) {
        hoveredTile.ToggleHighlightMaterial(false);
    }

    void CreateThePreviewWalls() {
        foreach(Tile tile in tilesLine) {
            tile.CreateWallsBasedOnPreview(WallMaterial);
        }
    }

    #region Crazy wall building algorithm

    void ShowWallsPreview(Tile lastHoveredTile, TileWallPosition endPosition) {
        if (wallPlacingStartingTile == null) 
            return;

        print("-------------------");
        if (tilesLine != null && tilesLine.Count > 0) ClearWallPreviews();

        // Get all tiles in the line
        tilesLine = TileManager.Instance.GetTilesInLine(wallPlacingStartingTile, lastHoveredTile);

        print($"tiles: {tilesLine.Count}");
        // Iterate through the tiles in the line
        for (int i = 0; i < tilesLine.Count; i++) {
            Tile currentTile = tilesLine[i];
            Tile endTile = tilesLine.Last();

            if (i == 0) 
            {
                // First tile: Add starting joint
                currentTile.AddWallJointPreview(WallJointPrefab, wallPlacingStartingPosition);
                if (GetDirection())
                {
                    TileWallPosition fillPosition = DetermineFillOrientationWall(
                        currentTile,
                        endTile
                        );
                    currentTile.AddWallFillPreview(WallFillPrefab, fillPosition);
                }
            }
            else
            {
                // Middle tiles: Add fill and both joints
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

                if (!tilesLine[i - 1].ContainsPreview(jointOrientations[1]))
                {
                    currentTile.AddWallJointPreview(WallJointPrefab, jointOrientations[0]);
                }

                if (!currentTile.ContainsPreview(jointOrientations[1]))
                {
                    currentTile.AddWallJointPreview(WallJointPrefab, jointOrientations[1]);
                }
            }
        }
    }

    private bool GetDirection()
    {
        if (tilesLine.Count() == 0)
            return false;

        Vector2Int startTilePosition = new (0,0);
        Vector2Int endTilePosition = new(0, 0);

        if (tilesLine.Count() > 1)
        {
            startTilePosition = wallPlacingStartingTile.GetGridPosition();
            endTilePosition = tilesLine.Last().GetGridPosition();
        }

        if (tilesLine.Count() == 1)
        {
            startTilePosition = tilesLine[0].clickedTile.GetGridPosition();
            endTilePosition = tilesLine[0].hoveredTile.GetGridPosition();
        }

        print($"Tile Position: {startTilePosition}, {endTilePosition}");
        var keyPosition = tilesLine[0].PreviewWallsDictionary.First().Key;
        if (endTilePosition.x - startTilePosition.x > 0)
        {
            if (keyPosition == TileWallPosition.TopLeft || keyPosition == TileWallPosition.BottomLeft)
            {
                return true;
            }
        }

        if (endTilePosition.x - startTilePosition.x < 0)
        {
            if (keyPosition == TileWallPosition.TopRight || keyPosition == TileWallPosition.BottomRight)
            {
                return true;
            }
        }

        if (endTilePosition.y - startTilePosition.y > 0)
        {
            if (keyPosition == TileWallPosition.BottomRight || keyPosition == TileWallPosition.BottomLeft)
            {
                return true;
            }
        }

        if (endTilePosition.y - startTilePosition.y < 0)
        {
            if (keyPosition == TileWallPosition.TopLeft || keyPosition == TileWallPosition.TopRight)
            {
                return true;
            }
        }

        return false;
    }

    private Vector2Int DetermineOrientation(Vector2Int firstTile,  Vector2Int secondTile)
    {
        return secondTile - firstTile;
    }

    /// <summary>
    /// Find and return the edge positions, at which a wall preview can be created, according to the delta in GridPosition
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

        Vector2Int delta = endTilePosition - startTilePosition;
        print($"Delta wall: {endTilePosition} - {startTilePosition}");
        // Horizontal 
        if (delta.x != 0)
        {
            // Horizontal
            if (startTile.clickedTile.GetGridPosition().y == 0)
            {
                return TileWallPosition.Bottom;
            }

            return TileWallPosition.Top;
        }
        else if (delta.y != 0)
        {
            // Vertical
            if (startTile.clickedTile.GetGridPosition().x == 0)
            {
                return TileWallPosition.Left;
            }

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

        Vector2Int delta = endTile.GetGridPosition() - startTile.GetGridPosition();

        if (delta.x != 0)
        {
            // Horizontal
            if (startTile.clickedTile.GetGridPosition().y == 0)
            {
                returnArr[0] = TileWallPosition.BottomLeft;
                returnArr[1] = TileWallPosition.BottomRight;
                return returnArr;
            }

            returnArr[0] = TileWallPosition.TopLeft;
            returnArr[1] = TileWallPosition.TopRight;
            return returnArr;
        }
        else if (delta.y != 0)
        {
            // Vertical
            if (startTile.clickedTile.GetGridPosition().x == 0)
            {
                returnArr[0] = TileWallPosition.TopLeft;
                returnArr[1] = TileWallPosition.BottomLeft;
                return returnArr;
            }

            returnArr[0] = TileWallPosition.TopRight;
            returnArr[1] = TileWallPosition.BottomRight;
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
