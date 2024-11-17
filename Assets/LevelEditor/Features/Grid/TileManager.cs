using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class TileManager : Singleton<TileManager> {

    public GameObject TilePrefab;
    float gridYpos = 0f;
    float tileSize = 1f;
    List<Tile> tiles = new List<Tile>();

    protected override void Awake() {
        base.Awake();

        SetUpGrid(10,10);
    }

    public void TileClickHandle(Tile tile) {
        // Place selected object or remove one if occupied
        if (tile.isTileOccupied()) {
            tile.RemoveObjectFromTile();
            return;
        }

        if (EditorObjectManager.Instance.GetSelectedObject() == null) {
            return;
        }
        tile.AddObjectToTile(EditorObjectManager.Instance.GetSelectedObject());
    }

    public void TileHoverEnterHandle(Tile tile) {
        // Add Outline to the tile
        tile.ToggleOutline(true);   
    }

    public void TileHoverExitHandle(Tile tile) {
        // Remove outline from the tile
        tile.ToggleOutline(false);
    }

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

}
