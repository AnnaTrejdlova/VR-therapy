using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CategoryManager : Singleton<CategoryManager>
{
    ScriptableObject[] editorObjects;

    protected override void Awake() {
        base.Awake();
    }

    private void Start()
    {
        LoadEditorObjects();
    }

    private void OnEnable()
    {
        TabController.OnCategoryChanged += HandleCategoryChanged;
    }

    private void OnDisable()
    {
        TabController.OnCategoryChanged -= HandleCategoryChanged;
    }

    private void HandleCategoryChanged(Category newCategory)
    {
        Debug.Log($"Category changed to: {newCategory.EditorObjectCategory}, {newCategory.Subcategory}");
        // Update UI or do something else here

    }

    private async void LoadEditorObjects()
    {
        // Call the singleton instance to load resources.
        await AsyncResourceLoader.Instance.LoadEditorObjectsAsync();

        // Access the loaded objects.
        editorObjects = await AsyncResourceLoader.Instance.GetLoadedObjects();
    }
}
