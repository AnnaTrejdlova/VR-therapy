using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class WallManager : Singleton<WallManager> {

    public GameObject WallJointPrefab;
    public GameObject WallFillPrefab;

    bool placingWall = false; // First joint has been selected and now we are previewing path to the second joint
    Tile wallPlacingStartingTile;
    TileWallOrientation wallPlacingStartingOrientation;
    List<Tile> tilesLine;

    protected override void Awake() {
        base.Awake();

    }

    public void WallPointClick(Tile tile, TileWallOrientation orientation) {
        tile.AddWallJointPreview(WallJointPrefab, orientation);
        wallPlacingStartingTile = tile;
        wallPlacingStartingOrientation = orientation;
        placingWall = true;
    }
    public void WallPointEnterHover(Tile tile, TileWallClickable tileWall, TileWallOrientation orientation) {
        tileWall.ToggleHighlightMaterial(true);
        if (wallPlacingStartingTile != null) ShowWallsPreview(tile, orientation);
    }
    public void WallPointExitHover(Tile tile, TileWallClickable tileWall) {
        tileWall.ToggleHighlightMaterial(false);
    }

    #region Crazy wall building algorithm

    void ShowWallsPreview(Tile endTile, TileWallOrientation endOrientation) {
        if (wallPlacingStartingTile == null) return;

        if (tilesLine != null && tilesLine.Count > 0) ClearWallPreviews();
        
        tilesLine = TileManager.Instance.GetTilesInLine(wallPlacingStartingTile, endTile);

        for (int i = 0; i < tilesLine.Count; i++) {
            TileWallOrientation fillOrientation = DetermineFillOrientationWall(wallPlacingStartingTile, tilesLine[tilesLine.Count - 1], wallPlacingStartingOrientation, endOrientation);
            Tile currentTile = tilesLine[i];

            if (i == tilesLine.Count - 1) {
                // Last tile
                currentTile.AddWallJointPreview(WallJointPrefab, endOrientation);
                TileWallOrientation lastFillOrientation = DetermineFillOrientationWall(
                    wallPlacingStartingTile,
                    tilesLine[tilesLine.Count - 1],
                    wallPlacingStartingOrientation,
                    endOrientation
                );
                currentTile.AddWallFillPreview(WallFillPrefab, lastFillOrientation);
            } else {
                // not last not first
                currentTile.AddWallFillPreview(WallFillPrefab, fillOrientation);
                TileWallOrientation[] jointOrientation = DetermineJointOrientationWall(wallPlacingStartingTile, tilesLine[tilesLine.Count - 1], wallPlacingStartingOrientation, endOrientation);
                currentTile.AddWallJointPreview(WallJointPrefab, jointOrientation[0]);
                currentTile.AddWallJointPreview(WallJointPrefab, jointOrientation[1]);
            }
        }

        // Handle same-tile looping fills
        if (wallPlacingStartingTile == endTile && wallPlacingStartingOrientation != endOrientation) {
            TileWallOrientation loopFillOrientation = DetermineFillOrientationBasedOnSingleTile(wallPlacingStartingOrientation, endOrientation);
            endTile.AddWallFillPreview(WallFillPrefab, loopFillOrientation);
            endTile.AddWallJointPreview(WallJointPrefab, endOrientation);
        }
    }

    TileWallOrientation DetermineFillOrientationWall(Tile startTile, Tile endTile, TileWallOrientation start, TileWallOrientation end) {
        Vector2Int startPos = startTile.GetGridPosition();
        Vector2Int endPos = endTile.GetGridPosition();

        // Horizontal 
        if (startPos.y == endPos.y) {
            if (start == TileWallOrientation.TopLeft && end == TileWallOrientation.TopLeft
                || start == TileWallOrientation.TopRight && end == TileWallOrientation.TopRight
                || start == TileWallOrientation.TopRight && end == TileWallOrientation.TopLeft
                || start == TileWallOrientation.TopLeft && end == TileWallOrientation.TopRight)
                return TileWallOrientation.Top;

            if (start == TileWallOrientation.BottomLeft && end == TileWallOrientation.BottomRight
                || start == TileWallOrientation.BottomRight && end == TileWallOrientation.BottomLeft
                || start == TileWallOrientation.BottomLeft && end == TileWallOrientation.BottomLeft
                || start == TileWallOrientation.BottomRight && end == TileWallOrientation.BottomRight)
                return TileWallOrientation.Bottom;
        }

        // Vertical
        if (startPos.x == endPos.x) {
            if (start == TileWallOrientation.TopLeft && end == TileWallOrientation.BottomLeft
                || start == TileWallOrientation.BottomLeft && end == TileWallOrientation.TopLeft
                || start == TileWallOrientation.BottomLeft && end == TileWallOrientation.BottomLeft
                || start == TileWallOrientation.TopLeft && end == TileWallOrientation.TopLeft)
                return TileWallOrientation.Left;

            if (start == TileWallOrientation.TopRight && end == TileWallOrientation.BottomRight
                || start == TileWallOrientation.BottomRight && end == TileWallOrientation.TopRight
                || start == TileWallOrientation.TopRight && end == TileWallOrientation.TopRight
                || start == TileWallOrientation.BottomRight && end == TileWallOrientation.BottomRight)
                return TileWallOrientation.Right;
        }

        throw new InvalidOperationException("Unexpected orientation.");
    }

    TileWallOrientation[] DetermineJointOrientationWall(Tile startTile, Tile endTile, TileWallOrientation start, TileWallOrientation end) {
        TileWallOrientation[] returnArr = new TileWallOrientation[2];
        Vector2Int startPos = startTile.GetGridPosition();
        Vector2Int endPos = endTile.GetGridPosition();
        // Horizontal 
        if (startPos.y == endPos.y) {
            if (start == TileWallOrientation.TopLeft && end == TileWallOrientation.TopLeft
                || start == TileWallOrientation.TopRight && end == TileWallOrientation.TopRight
                || start == TileWallOrientation.TopRight && end == TileWallOrientation.TopLeft
                || start == TileWallOrientation.TopLeft && end == TileWallOrientation.TopRight) {
                returnArr[0] = TileWallOrientation.TopLeft;
                returnArr[1] = TileWallOrientation.TopRight;
                return returnArr;
            }


            if (start == TileWallOrientation.BottomLeft && end == TileWallOrientation.BottomRight
                || start == TileWallOrientation.BottomRight && end == TileWallOrientation.BottomLeft
                || start == TileWallOrientation.BottomLeft && end == TileWallOrientation.BottomLeft
                || start == TileWallOrientation.BottomRight && end == TileWallOrientation.BottomRight) {
                returnArr[0] = TileWallOrientation.BottomLeft;
                returnArr[1] = TileWallOrientation.BottomRight;
                return returnArr;
            }
        }

        // Vertical
        if (startPos.x == endPos.x) {
            if (start == TileWallOrientation.TopLeft && end == TileWallOrientation.BottomLeft
                || start == TileWallOrientation.BottomLeft && end == TileWallOrientation.TopLeft
                || start == TileWallOrientation.BottomLeft && end == TileWallOrientation.BottomLeft
                || start == TileWallOrientation.TopLeft && end == TileWallOrientation.TopLeft) {
                returnArr[0] = TileWallOrientation.BottomLeft;
                returnArr[1] = TileWallOrientation.TopLeft;
                return returnArr;
            }
            if (start == TileWallOrientation.TopRight && end == TileWallOrientation.BottomRight
                || start == TileWallOrientation.BottomRight && end == TileWallOrientation.TopRight
                || start == TileWallOrientation.TopRight && end == TileWallOrientation.TopRight
                || start == TileWallOrientation.BottomRight && end == TileWallOrientation.BottomRight) {
                returnArr[0] = TileWallOrientation.BottomRight;
                returnArr[1] = TileWallOrientation.TopRight;
                return returnArr;
            }
        }

        throw new InvalidOperationException("Unexpected orientation.");
    }

    TileWallOrientation DetermineFillOrientationBasedOnSingleTile(TileWallOrientation start, TileWallOrientation end) {
        if (start == TileWallOrientation.TopLeft && end == TileWallOrientation.TopRight ||
            end == TileWallOrientation.TopLeft && start == TileWallOrientation.TopRight)
            return TileWallOrientation.Top;
        if (start == TileWallOrientation.BottomLeft && end == TileWallOrientation.BottomRight ||
            end == TileWallOrientation.BottomLeft && start == TileWallOrientation.BottomRight)
            return TileWallOrientation.Bottom;
        if (start == TileWallOrientation.TopLeft && end == TileWallOrientation.BottomLeft ||
            end == TileWallOrientation.TopLeft && start == TileWallOrientation.BottomLeft)
            return TileWallOrientation.Left;
        if (start == TileWallOrientation.TopRight && end == TileWallOrientation.BottomRight ||
            end == TileWallOrientation.TopRight && start == TileWallOrientation.BottomRight)
            return TileWallOrientation.Right;

        throw new InvalidOperationException("Unexpected same-tile fill orientation.");
    }

    void ClearWallPreviews() {
        foreach(Tile tile in tilesLine) {
            tile.ClearWallPreviews();
        }
    }

    #endregion

}
