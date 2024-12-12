using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.XR.Management;

public class MainMenuUI : MonoBehaviour
{
    public SceneType sceneToLoad;
    public SceneType sceneToUnload;

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        Button startButton = root.Q<Button>("StartButton");
        startButton.clicked += StartButton;

        Button startNonVRButton = root.Q<Button>("StartNonVR");
        startNonVRButton.clicked += StartNonVRButton;

        Button exitButton = root.Q<Button>("ExitButton");
        exitButton.clicked += ExitButton;
    }

    private void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        List<RadioButton> radioButtonList;

        // Difficulty group
        RadioButtonGroup difficultyGroup = root.Q<RadioButtonGroup>("DifficultyGroup");
        //Debug.Log(difficulty.value);

        //difficultyGroup.RegisterValueChangedCallback((evt) =>
        //{
        //    Debug.Log("Radio Changed! " + evt.newValue);
        //});

        radioButtonList = difficultyGroup.Query<RadioButton>().Build().ToList();
        foreach (var item in radioButtonList.Select((value, i) => new { i, value }))
        {
            radioButtonList[item.i].RegisterCallback<ChangeEvent<bool>>((evt) =>
            {
                if (evt.newValue)
                {
                    (evt.target as RadioButton).AddToClassList("RadioButton--selected");
                    difficultyGroup.value = item.i;
                    ApplicationModel.difficulty = (Difficulty)difficultyGroup.value;
                }
                else
                {
                    (evt.target as RadioButton).RemoveFromClassList("RadioButton--selected");
                }
            });
        }

        // Dominant hand group
        RadioButtonGroup dominantHandGroup = root.Q<RadioButtonGroup>("DominantHandGroup");
        radioButtonList = dominantHandGroup.Query<RadioButton>().Build().ToList();
        foreach (var item in radioButtonList.Select((value, i) => new { i, value }))
        {
            radioButtonList[item.i].RegisterCallback<ChangeEvent<bool>>((evt) =>
            {
                if (evt.newValue)
                {
                    (evt.target as RadioButton).AddToClassList("RadioButton--selected");
                    dominantHandGroup.value = item.i;
                    ApplicationModel.dominantHand = (DominantHand)dominantHandGroup.value;
                }
                else
                {
                    (evt.target as RadioButton).RemoveFromClassList("RadioButton--selected");
                }
            });
        }

        // Position group
        RadioButtonGroup positionGroup = root.Q<RadioButtonGroup>("PositionGroup");
        radioButtonList = positionGroup.Query<RadioButton>().Build().ToList();
        foreach (var item in radioButtonList.Select((value, i) => new { i, value }))
        {
            radioButtonList[item.i].RegisterCallback<ChangeEvent<bool>>((evt) =>
            {
                if (evt.newValue)
                {
                    (evt.target as RadioButton).AddToClassList("RadioButton--selected");
                    positionGroup.value = item.i;
                    ApplicationModel.position = (Position)positionGroup.value;
                }
                else
                {
                    (evt.target as RadioButton).RemoveFromClassList("RadioButton--selected");
                }
            });
        }
    }

    private void StartButton()
    {
        ApplicationModel.isVR = true;
        LoadGame();
    }

    private void StartNonVRButton()
    {
        ApplicationModel.isVR = false;
        LoadGame();
    }

    private void LoadGame()
    {
        StartCoroutine(InitializeGameC());
    }

    private void ExitButton()
    {
        Application.Quit();
    }

    private IEnumerator InitializeGameC()
    {
        // Wait until the scene loading task is complete
        var loadTask = SceneLoadingManager.Instance.LoadSceneAsync(sceneToLoad, 0f);
        yield return new WaitUntil(() => loadTask.IsCompleted);
        if (loadTask.Result)
        {
            // after its loaded do something
            SceneLoadingManager.Instance.SetActiveScene(sceneToLoad);
            var unLoadTask = SceneLoadingManager.Instance.UnLoadSceneAsync(sceneToUnload);
        }
        else
        {
            Debug.LogError("Failed to load scene.");
        }
    }
}
