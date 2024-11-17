using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour, IClickable {

    public float sizeMultiplicator = 0.1f;

    float tileSize = 1f;
    Outline outline;
    GameObject addedGameObject; // what you put into it

    void Awake() {
        outline = GetComponent<Outline>();
        ToggleOutline(false);
        SetTileSize(tileSize);
    }

    public void OnClick() {
        TileManager.Instance.TileClickHandle(this);
    }

    public void OnHoverEnter() {
        TileManager.Instance.TileHoverEnterHandle(this);
    }

    public void OnHoverExit() {
        TileManager.Instance.TileHoverExitHandle(this);
    }

    public void ToggleOutline(bool toggleOn) {
        outline.enabled = toggleOn;
    }

    public void SetTileSize(float size) {
        tileSize = size;
        transform.localScale = new Vector3(tileSize * sizeMultiplicator, transform.localScale.y, tileSize * sizeMultiplicator);
    }

    public void AddObjectToTile(GameObject obj) {
        InstantiateAddedObject(obj);
    }

    public void RemoveObjectFromTile() {
        Destroy(addedGameObject);
        addedGameObject = null;
    }

    public bool isTileOccupied() {
        return addedGameObject != null;
    }

    void InstantiateAddedObject(GameObject obj) {
        // must not be a child of the tile because of the scale shenanigans
        // I am adding 0.5f because the pivot is in the center of the model, if the pivot would be at the bottom of the model there wouldnt be need to make it go up
        addedGameObject = Instantiate(obj, transform.position + new Vector3(0,0.5f,0), Quaternion.identity);
        addedGameObject.transform.position += new Vector3(0, transform.localScale.y, 0);
    }
}
