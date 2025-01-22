using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class TabController: MonoBehaviour {
    public static event Action<Category> OnCategoryChanged;

    [SerializeField]
    private VisualTreeAsset tileTemplateAsset;

    private Button furnitureTab;
    private Button buildingTab;
    private VisualElement contentPanel;
    private UQueryState<TemplateContainer> TileQuery;

    private VisualElement root;

    private List<Button> tabButtons;
    private List<Button> categoryButtons;

    public EditorObjectCategory SelectedTabCategory { get; private set; } = 0;
    public Subcategory<FurnitureSubcategory, BuildingSubcategory> SelectedSubcategory { get; private set; } = (FurnitureSubcategory)0;

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
            ChangeCategory();
        });
        // Update the subcategory when the value changes
        buildingGroup.RegisterValueChangedCallback((evt) => {
            SelectedSubcategory = (BuildingSubcategory)evt.newValue;
            ChangeCategory();
        });

        #endregion

        #region Tabs group

        RadioButtonGroup tabGroup = root.Q<RadioButtonGroup>("tab-picker");

        RegisterRadioButtonListeners(tabGroup);

        // Update the tab category when the value changes
        tabGroup.RegisterValueChangedCallback((evt) => {
            SelectedTabCategory = (EditorObjectCategory)evt.newValue;

            switch (SelectedTabCategory) {
                case EditorObjectCategory.Furniture:
                    SelectedSubcategory = (FurnitureSubcategory)0;
                    furnitureGroup.style.display = DisplayStyle.Flex;
                    buildingGroup.style.display = DisplayStyle.None;
                    break;
                case EditorObjectCategory.Building:
                    SelectedSubcategory = (BuildingSubcategory)0;
                    furnitureGroup.style.display = DisplayStyle.None;
                    buildingGroup.style.display = DisplayStyle.Flex;
                    break;
                default:
                    break;
            }

            ChangeCategory();
        });

        #endregion

        contentPanel = root.Q<VisualElement>("content-panel");

        TileQuery = new UQueryBuilder<VisualElement>(root)
            .Descendents<VisualElement>("content-panel")
            .Descendents<ScrollView>("item-list")
            .Descendents<VisualElement>("content")
            .Descendents<TemplateContainer>("Tile")
            .Build();

        ChangeCategory();
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
                    (evt.target as RadioButton).AddToClassList("RadioButton--selected");
                    radioButtonGroup.value = item.i;
                } else {
                    (evt.target as RadioButton).RemoveFromClassList("RadioButton--selected");
                }
            });
        }
    }

    private async void ChangeCategory() {
        contentPanel.Q<Label>().text = SelectedTabCategory.ToString() + " -> " + SelectedSubcategory.ToString();

        // Fire the event, notifying listeners of the change
        OnCategoryChanged?.Invoke(new Category(SelectedTabCategory, SelectedSubcategory));

        var editorObjects = await AsyncResourceLoader.Instance.GetLoadedObjects();
        FillObjectPanelWithContent(editorObjects);
    }

    private void FillObjectPanelWithContent(EditorObjectScriptable[] editorObjects)
    {
        var inCategoryObjects = editorObjects;
        //var inCategoryObjects = editorObjects.Where(obj =>
        //    (SelectedTabCategory == EditorObjectCategory.Furniture && obj.EditorObjectCategory.Category is FurnitureSubcategory furnitureSubcategory && furnitureSubcategory == (FurnitureSubcategory)SelectedSubcategory.Category) ||
        //    (SelectedTabCategory == EditorObjectCategory.Building && obj.EditorObjectCategory.Category is BuildingSubcategory buildingSubcategory && buildingSubcategory == (BuildingSubcategory)SelectedSubcategory.Category)
        //).ToArray();

        // Populate tiles with new content
        for (int i = 0; i < Math.Max(inCategoryObjects.Length, TileQuery.Count()); i++)
        {
            // All objects are rendered, hide the rest
            if (i >= inCategoryObjects.Length)
            {
                var _tile = TileQuery.AtIndex(i);
                _tile.visible = false;
                continue;
            }

            // Set the model UI texture to the tile background image
            if (i < TileQuery.Count())
            {
                var _tile = TileQuery.AtIndex(i);
                _tile.style.backgroundImage = inCategoryObjects[i].UITexture;
                _tile.visible = true;
            }
            else
            { // Not enough tiles in a pool
                var newTile = tileTemplateAsset.CloneTree();
                newTile.style.backgroundImage = inCategoryObjects[i].UITexture;
                TileQuery.AtIndex(0).parent.Add(newTile); // Assumes at least 1 Tile is present
            }

            // Generate a debug label for the Tile
            var tile = TileQuery.AtIndex(i);
            var label = tile.Q<Label>();
            if (label == null)
            {
                label = new Label(inCategoryObjects[i].name);
                tile.Add(label);
            }
            else
            {
                label.text = inCategoryObjects[i].name;
            }
        }
    }
}
