using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : Singleton<TileManager> {

    public GameObject TilePrefab;
    public List<TileInteractionStrategyEntry> tileInteractionStrategyEntries = new List<TileInteractionStrategyEntry>();
    float gridYpos = 0f;
    float tileSize = 1f;
    List<Tile> tiles = new List<Tile>();

    protected override void Awake() {
        base.Awake();

        SetUpGrid(10,10);
    }

    # region Tile interaction strategy

    public void TileClickHandle(Tile tile) {
        FindStrategy(LevelEditorManager.Instance.GetState()).OnTileClick(tile);
    }

    public void TileHoverEnterHandle(Tile tile) {
        FindStrategy(LevelEditorManager.Instance.GetState()).OnTileHover(tile);
    }

    public void TileHoverExitHandle(Tile tile) {
        FindStrategy(LevelEditorManager.Instance.GetState()).OnTileUnhover(tile);
    }

    #endregion

    #region Grid setup

    public void SetUpGrid(int width, int height) {
        for (int i = 0; i <= width; i++) {
            for (int j = 0; j <= height; j++) {
                Vector3 position = new Vector3(i* tileSize, gridYpos, j * tileSize);
                SpawnTile(position);
            }
        }
    }

    void SpawnTile(Vector3 position) {
        tiles.Add(Instantiate(TilePrefab, position, Quaternion.identity).GetComponent<Tile>());
    }

    #endregion

    # region Support functions

    ITileInteractionStrategy FindStrategy(EditorState state) {
        foreach(var strat in tileInteractionStrategyEntries) {
            if(strat.state == state) {
                return strat.GetStrategy();
            }
        }
        print("no strategy found for this editor state!");
        return null;
    }

    #endregion

}

// Class for being able to assign strategies in editor

[Serializable]
public class TileInteractionStrategyEntry {
    public EditorState state;
    public MonoBehaviour strategy;

    public ITileInteractionStrategy GetStrategy() {
        return strategy as ITileInteractionStrategy;
    }
}