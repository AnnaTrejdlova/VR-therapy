using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class TabController: MonoBehaviour {
    private Button furnitureTab;
    private Button buildingTab;
    private VisualElement contentPanel;

    private VisualElement root;

    private List<Button> tabButtons;
    private List<Button> categoryButtons;

    TabCategory selectedTabCategory = 0;
    FurnitureCategory selectedFurnitureCategory = 0;
    BuildingCategory selectedBuildingCategory = 0;

    private void OnEnable() {
        root = GetComponent<UIDocument>().rootVisualElement;

        #region Category group

        RadioButtonGroup furnitureGroup = root.Q<RadioButtonGroup>("furniture-picker");
        RadioButtonGroup buildingGroup = root.Q<RadioButtonGroup>("building-picker");

        RegisterRadioButtonListeners(furnitureGroup);
        RegisterRadioButtonListeners(buildingGroup);

        furnitureGroup.RegisterValueChangedCallback((evt) => {
            selectedFurnitureCategory = (FurnitureCategory)evt.newValue;
            Debug.Log("Radio Changed! (FurnitureCategory): " + selectedFurnitureCategory);

            ChangeCategory();
        });
        buildingGroup.RegisterValueChangedCallback((evt) => {
            selectedBuildingCategory = (BuildingCategory)evt.newValue;
            Debug.Log("Radio Changed! (BuildingCategory): " + selectedBuildingCategory);

            ChangeCategory();
        });

        #endregion

        #region Tabs group

        RadioButtonGroup tabGroup = root.Q<RadioButtonGroup>("tab-picker");

        RegisterRadioButtonListeners(tabGroup);

        tabGroup.RegisterValueChangedCallback((evt) => {
            selectedTabCategory = (TabCategory)evt.newValue;
            Debug.Log("Radio Changed! (tab-picker): " + selectedTabCategory);

            switch (selectedTabCategory) {
                case TabCategory.Furniture:
                    furnitureGroup.style.display = DisplayStyle.Flex;
                    buildingGroup.style.display = DisplayStyle.None;
                    break;
                case TabCategory.Building:
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
        ChangeCategory();
    }

    /// <summary>
    ///     Register button listeners for category radio buttons
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

    private void ChangeCategory() {
        contentPanel.Q<Label>().text = selectedTabCategory.ToString() + " -> " + ((selectedTabCategory == TabCategory.Furniture) ? selectedFurnitureCategory.ToString() : selectedBuildingCategory.ToString());
        switch (selectedTabCategory) {
            case TabCategory.Furniture:
                switch (selectedFurnitureCategory) {
                    case FurnitureCategory.LivingRoom:

                        break;
                    case FurnitureCategory.Bathroom:
                        break;
                    case FurnitureCategory.Kitchen:
                        break;
                    default:
                        break;
                }
                break;
            case TabCategory.Building:
                switch (selectedBuildingCategory) {
                    case BuildingCategory.Walls:
                        break;
                    case BuildingCategory.Floor:
                        break;
                    case BuildingCategory.Doors:
                        break;
                    case BuildingCategory.Windows:
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }

    struct Category {
        TabCategory _tabCategory;
        FurnitureCategory _furnitureCategory;
        BuildingCategory _buildingCategory;
    }

    enum TabCategory {
        Furniture,
        Building
    }

    enum FurnitureCategory {
        LivingRoom,
        Bathroom,
        Kitchen
    }

    enum BuildingCategory {
        Walls,
        Floor,
        Doors,
        Windows
    }
}
