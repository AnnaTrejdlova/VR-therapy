using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class WallManager : Singleton<WallManager> {

    public GameObject WallJointPrefab;
    public GameObject WallFillPrefab;
    public Material WallMaterial;

    bool placingWall = false; // First joint has been selected and now we are previewing path to the second joint
    Tile wallPlacingStartingTile;
    TileWallOrientation wallPlacingStartingOrientation;
    List<Tile> tilesLine;

    protected override void Awake() {
        base.Awake();

    }

    public void WallPointClick(Tile tile, TileWallOrientation orientation) {
        tile.AddWallJointPreview(WallJointPrefab, orientation);
        
        if (!placingWall) {
            // Wall preview
            placingWall = true;
            wallPlacingStartingTile = tile;
            wallPlacingStartingOrientation = orientation;
        } else {
            // Place the previewed wall
            CreateThePreviewWalls();
            tilesLine = new List<Tile>();
            placingWall = false;
        }
    }
    public void WallPointEnterHover(Tile tile, TileWallClickable tileWall, TileWallOrientation orientation) {
        tileWall.ToggleHighlightMaterial(true);
        if (placingWall) ShowWallsPreview(tile, orientation);
    }
    public void WallPointExitHover(Tile tile, TileWallClickable tileWall) {
        tileWall.ToggleHighlightMaterial(false);
    }

    void CreateThePreviewWalls() {
        foreach(Tile tile in tilesLine) {
            tile.CreateWallsBasedOnPreview(WallMaterial);
        }
    }

    #region Crazy wall building algorithm

    void ShowWallsPreview(Tile endTile, TileWallOrientation endOrientation) {
        if (wallPlacingStartingTile == null) return;

        // Clear previous previews
        if (tilesLine != null && tilesLine.Count > 0) ClearWallPreviews();

        // Get all tiles in the line
        tilesLine = TileManager.Instance.GetTilesInLine(wallPlacingStartingTile, endTile);

        // Iterate through the tiles in the line
        for (int i = 0; i < tilesLine.Count; i++) {
            Tile currentTile = tilesLine[i];

            // Determine the fill orientation for this tile
            TileWallOrientation fillOrientation = DetermineFillOrientationWall(
                wallPlacingStartingTile,
                endTile,
                wallPlacingStartingOrientation,
                endOrientation
            );

            if (i == 0) {
                // First tile: Add starting joint and fill
                currentTile.AddWallJointPreview(WallJointPrefab, wallPlacingStartingOrientation);
                currentTile.AddWallFillPreview(WallFillPrefab, fillOrientation);
            } else if (i == tilesLine.Count - 1) {
                // Last tile: Add ending joint and fill
                currentTile.AddWallJointPreview(WallJointPrefab, endOrientation);
                TileWallOrientation lastFillOrientation = DetermineFillOrientationWall(
                    wallPlacingStartingTile,
                    endTile,
                    wallPlacingStartingOrientation,
                    endOrientation
                );
                currentTile.AddWallFillPreview(WallFillPrefab, lastFillOrientation);
            } else {
                // Middle tiles: Add fill and both joints
                currentTile.AddWallFillPreview(WallFillPrefab, fillOrientation);
                TileWallOrientation[] jointOrientations = DetermineJointOrientationWall(
                    wallPlacingStartingTile,
                    endTile,
                    wallPlacingStartingOrientation,
                    endOrientation
                );
                currentTile.AddWallJointPreview(WallJointPrefab, jointOrientations[0]);
                currentTile.AddWallJointPreview(WallJointPrefab, jointOrientations[1]);
            }
        }

        // Handle loop case: Same start and end tile
        if (wallPlacingStartingTile == endTile && wallPlacingStartingOrientation != endOrientation) {
            TileWallOrientation loopFillOrientation = DetermineFillOrientationBasedOnSingleTile(
                wallPlacingStartingOrientation,
                endOrientation
            );
            endTile.AddWallFillPreview(WallFillPrefab, loopFillOrientation);
            endTile.AddWallJointPreview(WallJointPrefab, endOrientation);
        }
    }


    TileWallOrientation DetermineFillOrientationWall(Tile startTile, Tile endTile, TileWallOrientation start, TileWallOrientation end) {
        Vector2Int startPos = startTile.GetGridPosition();
        Vector2Int endPos = endTile.GetGridPosition();

        print($"trying fill orientation for: {startPos} - {start} {endPos} - {end}");

        // Horizontal 
        if (startPos.y == endPos.y) {
            print("is horizontal");
            if (start == TileWallOrientation.TopLeft && end == TileWallOrientation.TopLeft
                || start == TileWallOrientation.TopRight && end == TileWallOrientation.TopRight
                || start == TileWallOrientation.TopRight && end == TileWallOrientation.TopLeft
                || start == TileWallOrientation.TopLeft && end == TileWallOrientation.TopRight) {
                print("top");
                return TileWallOrientation.Top;
            }

            if (start == TileWallOrientation.BottomLeft && end == TileWallOrientation.BottomRight
                || start == TileWallOrientation.BottomRight && end == TileWallOrientation.BottomLeft
                || start == TileWallOrientation.BottomLeft && end == TileWallOrientation.BottomLeft
                || start == TileWallOrientation.BottomRight && end == TileWallOrientation.BottomRight) {
                print("bot");
                return TileWallOrientation.Bottom;
            }
        }

        // Vertical
        if (startPos.x == endPos.x) {
            print("is vertical");
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
