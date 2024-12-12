using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneDebugger : MonoBehaviour {
    /// <summary>
    /// You add this to your scene if you need utilities loaded for the thing you are testing
    /// </summary>


    public SceneField Utility;

    void Awake() {
        StartCoroutine(LoadUtilitiesAndInitializeScene());
    }

    IEnumerator LoadUtilitiesAndInitializeScene() {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Utilities", LoadSceneMode.Additive);
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
