using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

    private static T _instance;

    public static T Instance {
        get {
            if (_instance == null) {
                //print($"The singleton you are trying to access isnt instantialized! (This causes issues only if not on the first frame calls)");
            }
            return _instance;
        }
    }

    protected virtual void Awake() {
        if (_instance == null) {
            _instance = this as T;
            //      DontDestroyOnLoad(gameObject);
        } else if (_instance != this) {
            Destroy(gameObject);
        }
    }


    // THIS FUCKING SCRIPT SPAWNS IT FROM THE FUCKING DEAD EVEN IF ITS NOT IN THE SCENE WAAAAAAAAAA

    /*
    private static T _instance;

    public static T Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<T>();

                if (_instance == null) {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<T>();
                    singletonObject.name = typeof(T).ToString() + " (Singleton)";

             //       DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake() {
        if (_instance == null) {
            _instance = this as T;
      //      DontDestroyOnLoad(gameObject);
        } else if (_instance != this) {
            Destroy(gameObject);
        }
    }
    */
}