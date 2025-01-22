using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AsyncResourceLoader : Singleton<AsyncResourceLoader>
{
    public LoadingState State { get; private set; } = LoadingState.NotStarted;

    private EditorObjectScriptable[] _loadedEditorObjects;

    protected override void Awake()
    {
        base.Awake();
    }

    // Helper method: Converts an AsyncOperation to a Task
    private async Task<T> LoadResourceAsync<T>(string path) where T : Object {
        /* What chat says
         * Unity recommends avoiding heavy use of the Resources folder for production projects
         * due to performance and memory overhead. Consider asset bundles or Addressables for larger projects.
         * If you use Addressables, they have better built-in async support.
        */
        ResourceRequest resourceRequest = Resources.LoadAsync<T>(path);

        while (!resourceRequest.isDone) {
            await Task.Yield(); // Wait for the next frame
        }

        return resourceRequest.asset as T;
    }

    public async Task<EditorObjectScriptable[]> LoadEditorObjectsAsync()
    {
        State = LoadingState.Loading;
        List<Task<EditorObjectScriptable>> loadTasks = new List<Task<EditorObjectScriptable>>();

        // Load all resource names
        Object[] resourceObjects = Resources.LoadAll("EditorObjects", typeof(EditorObjectScriptable));

        foreach (Object obj in resourceObjects)
        {
            string path = $"EditorObjects/{obj.name}";
            loadTasks.Add(LoadResourceAsync<EditorObjectScriptable>(path));
        }
        _loadedEditorObjects = await Task.WhenAll(loadTasks);

        State = LoadingState.Completed;
        return _loadedEditorObjects;
    }

    public async Task<EditorObjectScriptable[]> GetLoadedObjects()
    {
        if (State == LoadingState.NotStarted)
        {
            return await LoadEditorObjectsAsync();
        }
        else if (State == LoadingState.Loading)
        {
            while (State == LoadingState.Loading)
            {
                await Task.Yield(); // Wait for the loading to complete
            }
        }

        // When State == LoadingState.Completed
        return _loadedEditorObjects ?? new EditorObjectScriptable[0];
    }

    public enum LoadingState
    {
        NotStarted,
        Loading,
        Completed
    }
}
