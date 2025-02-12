using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
/// <summary>
/// You add this to your scene if you need utilities loaded for the thing you are testing
/// </summary>
public class SceneDebugger : MonoBehaviour {
    public SceneField Utility;

    void Awake() {
        // If Utilities not loaded, load them
        if (!SceneManager.GetSceneByName(Utility.SceneName).IsValid()) {
            StartCoroutine(LoadUtilitiesAndInitializeScene());
        } else {
            // Should I run initializer?
        }
    }

    IEnumerator LoadUtilitiesAndInitializeScene() {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(Utility.SceneName, LoadSceneMode.Additive);
        while (!asyncLoad.isDone) {
            yield return null;
        }
        Iinitializer initializer = FindInitializerInScene(SceneManager.GetActiveScene().name);
        initializer?.Initialize();
        initializer?.StartRunning();
    }

    Iinitializer FindInitializerInScene(string scene) {
        GameObject[] rootObjects = SceneManager.GetSceneByName(scene).GetRootGameObjects();
        foreach (GameObject obj in rootObjects) {
            Iinitializer initializer = obj.GetComponentInChildren<Iinitializer>();
            if (initializer != null) {
                return initializer;
            }
        }
        return null;
    }

}
#endif
