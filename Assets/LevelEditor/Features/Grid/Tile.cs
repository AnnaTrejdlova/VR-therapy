using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour, IClickable {

    [Header("Misc")]
    public float sizeMultiplicator = 0.1f;
    [Header("Walls")]
    public GameObject wallColliders;
    public List<TileOrientationPosition> tileOrientationPositions = new List<TileOrientationPosition>();

    BoxCollider classicCollider;
    float tileSize = 1f;
    Outline outline;
    GameObject addedGameObject; // what you put into it
    Vector2Int gridPosition;
    Dictionary<TileWallOrientation, GameObject> AddedWallsDictionary = new Dictionary<TileWallOrientation, GameObject>();
    Dictionary<TileWallOrientation, GameObject> PreviewWallsDictionary = new Dictionary<TileWallOrientation, GameObject>();

    void Awake() {
        classicCollider = GetComponent<BoxCollider>();  
        outline = GetComponent<Outline>();
        SetTileSize(tileSize);
        ToggleOutline(false);   
    }

    public void ToggleOutline(bool toggleOn) {
        outline.enabled = toggleOn;
    }

    #region Wall management

    public void AddWallJoint(GameObject wallJointPrefab, TileWallOrientation orientation) {
        GameObject go = Instantiate(wallJointPrefab, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
        addedGameObject.transform.position += new Vector3(0, transform.localScale.y, 0);
        AddedWallsDictionary.Add(orientation, go);
    }

    public void AddWallFill(GameObject wallFillPrefab, TileWallOrientation orientation) {
        GameObject go = Instantiate(wallFillPrefab, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
        addedGameObject.transform.position += new Vector3(0, transform.localScale.y, 0);
        AddedWallsDictionary.Add(orientation, go);
    }

    public void ShowWallJointPreview(GameObject wallJointPrefab, TileWallOrientation orientation) {
        GameObject go = Instantiate(wallJointPrefab, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
        addedGameObject.transform.position += new Vector3(0, transform.localScale.y, 0);
        PreviewWallsDictionary.Add(orientation, go);
    }

    public void HideWallJointPreview(GameObject wallJointPrefab, TileWallOrientation orientation) {

    }

    public void ShowWallFillPreview(GameObject wallFillPrefab, TileWallOrientation orientation) {
        GameObject go = Instantiate(wallFillPrefab, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
        addedGameObject.transform.position += new Vector3(0, transform.localScale.y, 0);
        PreviewWallsDictionary.Add(orientation, go);
    }

    public void HideWallFillPreview(GameObject wallFillPrefab, TileWallOrientation orientation) {

    }


    #endregion

    #region Adding/Removing objects

    public void AddObjectToTile(GameObject obj) {
        InstantiateAddedObject(obj);
    }

    public void RemoveObjectFromTile() {
        Destroy(addedGameObject);
        addedGameObject = null;
    }

    void InstantiateAddedObject(GameObject obj) {
        if (addedGameObject != null) {
            Destroy(addedGameObject);
        }
        // must not be a child of the tile because of the scale shenanigans
        // I am adding 0.5f because the pivot is in the center of the model, if the pivot would be at the bottom of the model there wouldnt be need to make it go up
        addedGameObject = Instantiate(obj, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
        addedGameObject.transform.position += new Vector3(0, transform.localScale.y, 0);
    }

    #endregion

    #region IClickable interface

    public void OnClick() {
        TileManager.Instance.TileClickHandle(this);
    }

    public void OnHoverEnter() {
        TileManager.Instance.TileHoverEnterHandle(this);
    }

    public void OnHoverExit() {
        TileManager.Instance.TileHoverExitHandle(this);
    }

    #endregion

    #region SetUp related

    public void SetTileSize(float size) {
        tileSize = size;
        transform.localScale = new Vector3(tileSize * sizeMultiplicator, transform.localScale.y, tileSize * sizeMultiplicator);
    }

    public void SetGridPosition(Vector2Int pos) {
        gridPosition = pos;
    }

    public Vector2Int GetGridPosition() {
        return gridPosition;
    }

    public bool isTileOccupied() {
        return addedGameObject != null;
    }

    #endregion

    #region Different collider management

    void SwitchToWallsColliders() {
        classicCollider.enabled = false;
        wallColliders.SetActive(true);
    }

    void SwitchToClassicCollider() {
        classicCollider.enabled = true;
        wallColliders.SetActive(false);
    }

    #endregion

    #region EventListener (editor state)

    void HandleStateChange(EditorState newState) {
        switch (newState) {
            case EditorState.PlacingObjects:
                SwitchToClassicCollider();
                outline.OutlineColor = Color.black;
                break;
            case EditorState.RemovingObjects:
                outline.OutlineColor = Color.red;
                SwitchToClassicCollider();
                break;
            case EditorState.PlacingWalls:
                outline.OutlineColor = Color.black;
                SwitchToWallsColliders();
                break;

            default:
                outline.OutlineColor = Color.black;
                break;
        }
    }
    void OnEnable() {
        LevelEditorManager.Instance.OnStateChanged.AddListener(HandleStateChange);
    }

    void OnDisable() {
        LevelEditorManager.Instance.OnStateChanged.RemoveListener(HandleStateChange);
    }

    #endregion
}
// Class for being able to assign Orientation positions

[Serializable]
public class TileOrientationPosition {
    public TileWallOrientation state;
    public Vector3 position;
}