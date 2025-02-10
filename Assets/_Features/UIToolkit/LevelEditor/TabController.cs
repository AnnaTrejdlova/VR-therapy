using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class TabController: Singleton<TabController> {
    public static event Action<Category> OnCategoryChanged;

    [SerializeField]
    private VisualTreeAsset tileTemplateAsset;

    private VisualElement contentPanel;
    private UQueryState<TemplateContainer> TileQuery;

    private VisualElement root;

    public EditorObjectCategory SelectedTabCategory { get; private set; } = 0;
    public Subcategory<FurnitureSubcategory, BuildingSubcategory> SelectedSubcategory { get; private set; } = (FurnitureSubcategory)0;

    private List<EditorObjectScriptable> inCategoryObjects = new();
    public EditorObjectScriptable SelectedObject { get; private set; }

    private void OnEnable() {
        root = GetComponent<UIDocument>().rootVisualElement;

        #region Category group

        RadioButtonGroup furnitureGroup = root.Q<RadioButtonGroup>("furniture-picker");
        RadioButtonGroup buildingGroup = root.Q<RadioButtonGroup>("building-picker");

        RegisterRadioButtonListeners(furnitureGroup);
        RegisterRadioButtonListeners(buildingGroup);

        // Update the subcategory when the value changes
        furnitureGroup.RegisterValueChangedCallback((evt) => {
            SelectedSubcategory = (FurnitureSubcategory)evt.newValue;
            LevelEditorManager.Instance.ChangeState(EditorState.None);
            ChangeCategory();
        });
        // Update the subcategory when the value changes
        buildingGroup.RegisterValueChangedCallback((evt) => {
            SelectedSubcategory = (BuildingSubcategory)evt.newValue;
            LevelEditorManager.Instance.ChangeState(EditorState.None);
            ChangeCategory();
        });

        #endregion

        #region Toolbox group
        Button deleteModeButton = root.Q<Button>("delete-mode-btn");
        Button wallDeleteModeButton = root.Q<Button>("wall-delete-mode-btn");
        Button clearToolButton = root.Q<Button>("clear-tool-btn");

        deleteModeButton.clicked += OnDeleteModeClick;
        wallDeleteModeButton.clicked += OnWallDeleteModeClick;
        clearToolButton.clicked += OnClearModeClick;

        furnitureGroup.style.display = DisplayStyle.Flex;
        buildingGroup.style.display = DisplayStyle.None;
        #endregion

        #region Tabs group

        RadioButtonGroup tabGroup = root.Q<RadioButtonGroup>("tab-picker");

        RegisterRadioButtonListeners(tabGroup);

        // Update the tab category when the value changes
        tabGroup.RegisterValueChangedCallback((evt) => {
            SelectedTabCategory = (EditorObjectCategory)evt.newValue;
            LevelEditorManager.Instance.ChangeState(EditorState.None);

            switch (SelectedTabCategory) {
                case EditorObjectCategory.Furniture:
                    SelectedSubcategory = (FurnitureSubcategory)furnitureGroup.value;
                    furnitureGroup.style.display = DisplayStyle.Flex;
                    buildingGroup.style.display = DisplayStyle.None;

                    wallDeleteModeButton.parent.style.display = DisplayStyle.None;
                    deleteModeButton.parent.style.display = DisplayStyle.Flex;
                    break;
                case EditorObjectCategory.Building:
                    SelectedSubcategory = (BuildingSubcategory)buildingGroup.value;
                    furnitureGroup.style.display = DisplayStyle.None;
                    buildingGroup.style.display = DisplayStyle.Flex;

                    wallDeleteModeButton.parent.style.display = DisplayStyle.Flex;
                    deleteModeButton.parent.style.display = DisplayStyle.None;
                    break;
                default:
                    break;
            }

            ChangeCategory();
        });

        #endregion

        #region Item-tile group
        contentPanel = root.Q<VisualElement>("content-panel");

        TileQuery = new UQueryBuilder<VisualElement>(root)
            .Descendents<VisualElement>("content-panel")
            .Descendents<ScrollView>("item-list")
            .Descendents<VisualElement>("content")
            .Descendents<TemplateContainer>("Tile")
            .Build();

        RadioButtonGroup tileGroup = root.Q<RadioButtonGroup>("item-picker");

        RegisterRadioButtonListeners(tileGroup);

        // React to selected object (placing mode)
        tileGroup.RegisterValueChangedCallback(OnTileSelected);
        #endregion

        #region Setup Tooltips
        var buttonsWithTooltip = (IEnumerable<VisualElement>)root.Query<Button>().Build()
            .Where(btn => !string.IsNullOrEmpty(btn.tooltip));
        var radioButtonsWithTooltip = (IEnumerable<VisualElement>)root.Query<RadioButton>().Build()
            .Where(btn => !string.IsNullOrEmpty(btn.tooltip));

        IEnumerable<VisualElement> combinedQuery = buttonsWithTooltip.Concat(radioButtonsWithTooltip);

        var tooltipHandler = GetComponent<TooltipHandler>();

        foreach (var element in combinedQuery) {
            element.RegisterCallback<MouseOverEvent>(evt => MyButtonMouseOver(element));
            element.RegisterCallback<MouseOutEvent>(evt => MyButtonMouseOut());
        }

        void MyButtonMouseOver(VisualElement element) {
            // For dynamic tooltips
            StringObject tooltipString = new StringObject(element.tooltip);
            //tooltipString.text = "Some String or other variable that you want to display in a tooltip.";
            tooltipHandler.OnElementMouseOver(tooltipString);
        }

        void MyButtonMouseOut() {
            tooltipHandler.OnElementMouseLeave();
        }
        #endregion

        ChangeCategory();
    }
    public void OnDeleteModeClick() {
        ResetSelectedObjectUI();
        LevelEditorManager.Instance.ChangeState(EditorState.RemovingObjects);
    }

    public void OnWallModeClick() {
        ResetSelectedObjectUI();
        LevelEditorManager.Instance.ChangeState(EditorState.PlacingWalls);
    }

    public void OnWallDeleteModeClick() {
        ResetSelectedObjectUI();
        LevelEditorManager.Instance.ChangeState(EditorState.RemovingWalls);
    }

    public void OnClearModeClick() {
        ResetSelectedObjectUI();
        LevelEditorManager.Instance.ChangeState(EditorState.None);
        EditorObjectManager.Instance.SelectObject(null); // clear model
    }

    void OnTileSelected(ChangeEvent<int> evt) {
        if (evt.newValue == -1) return;

        SelectedObject = inCategoryObjects[evt.newValue];
        Debug.Log(SelectedObject.name);

        if (SelectedObject.EditorObjectType == EditorObjectCategory.Furniture) { // Is Furniture -> PlacingObjects
            LevelEditorManager.Instance.ChangeState(EditorState.PlacingObjects);
            EditorObjectManager.Instance.SelectObject(SelectedObject.Model);
        } else if (SelectedObject.EditorObjectType == EditorObjectCategory.Building) { // Is building object -> PlacingWalls
            if (((BuildingSubcategory)SelectedObject.Subcategory) == BuildingSubcategory.Wall) {
                //EditorHUDui.OnWallModeClick();
                LevelEditorManager.Instance.ChangeState(EditorState.PlacingWalls);
            }
        }
    }

    /// <summary>
    ///     Register button listeners for subcategory radio buttons
    /// </summary>
    /// <param name="radioButtonGroup"></param>
    private void RegisterRadioButtonListeners(RadioButtonGroup radioButtonGroup) {
        List<RadioButton> radioButtonList = radioButtonGroup.Query<RadioButton>().Build().ToList();

        foreach (var item in radioButtonList.Select((value, i) => new { i, value })) {
            radioButtonList[item.i].RegisterCallback<ChangeEvent<bool>>((evt) => {
                if (evt.newValue) {
                    (evt.target as RadioButton).parent.AddToClassList("RadioButton--selected");
                    radioButtonGroup.value = item.i;
                } else {
                    (evt.target as RadioButton).parent.RemoveFromClassList("RadioButton--selected");
                }
            });
        }
    }

    private async void ChangeCategory() {
        ResetSelectedObjectUI();

        contentPanel.Q<Label>().text = SelectedTabCategory.ToString() + " -> " + SelectedSubcategory.ToString();

        // Fire the event, notifying listeners of the change
        OnCategoryChanged?.Invoke(new Category(SelectedTabCategory, SelectedSubcategory));

        var editorObjects = await AsyncResourceLoader.Instance.GetLoadedObjects();
        inCategoryObjects = editorObjects
            .Where(obj =>
                (SelectedTabCategory == EditorObjectCategory.Furniture &&
                 obj.EditorObjectType == EditorObjectCategory.Furniture &&
                 (FurnitureSubcategory)obj.Subcategory == (FurnitureSubcategory)SelectedSubcategory.Category) ||
                (SelectedTabCategory == EditorObjectCategory.Building &&
                 obj.EditorObjectType == EditorObjectCategory.Building &&
                 (BuildingSubcategory)obj.Subcategory == (BuildingSubcategory)SelectedSubcategory.Category))
            .ToList();
        FillObjectPanelWithContent();
    }

    /// <summary>
    ///     Reset all active button and variables associated with it
    /// </summary>
    private void ResetSelectedObjectUI() {
        // Reset tile RadioButtons
        for (int i = 0; i < Math.Max(inCategoryObjects.Count, TileQuery.Count()); i++) {
            var _tile = TileQuery.AtIndex(i);
            _tile.Q<VisualElement>("tile").RemoveFromClassList("RadioButton--selected");
            _tile.Q<VisualElement>("tile").Q<RadioButton>().value = false;
        }

        // Reset tile RadioButtonGroup
        RadioButtonGroup tileGroup = root.Q<RadioButtonGroup>("item-picker");
        tileGroup.value = -1;
        SelectedObject = null;
    }

    private void FillObjectPanelWithContent() {
        // Populate tiles with new content
        for (int i = 0; i < Math.Max(inCategoryObjects.Count, TileQuery.Count()); i++) {
            // All objects are rendered, hide the rest
            if (i >= inCategoryObjects.Count) {
                var _tile = TileQuery.AtIndex(i);
                _tile.visible = false;
                continue;
            }

            // Reuse a tile to set the model UI texture
            if (i < TileQuery.Count()) {
                var _tile = TileQuery.AtIndex(i);
                _tile.Q<VisualElement>("tile").RemoveFromClassList("RadioButton--selected");
                _tile.Q<VisualElement>("tile").style.backgroundImage = inCategoryObjects[i].UITexture;
                _tile.Q<VisualElement>("tile").Q<RadioButton>().value = false;
                _tile.style.height = _tile.style.width;
                _tile.visible = true;
            } else { // Not enough tiles in a pool
                var newTile = tileTemplateAsset.CloneTree();
                newTile.Q<VisualElement>("tile").style.backgroundImage = inCategoryObjects[i].UITexture;
                newTile.style.height = newTile.style.width;
                TileQuery.AtIndex(0).parent.Add(newTile); // Assumes at least 1 Tile is present
            }

            // Generate a debug label for the Tile
            var tile = TileQuery.AtIndex(i);
            var label = tile.Q<Label>();
            if (label == null) {
                label = new Label(inCategoryObjects[i].name);
                tile.Add(label);
            } else {
                label.text = inCategoryObjects[i].name;
            }
        }
    }
}
