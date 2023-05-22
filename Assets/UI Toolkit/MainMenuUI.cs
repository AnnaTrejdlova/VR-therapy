using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.XR.Management;

public class MainMenuUI : MonoBehaviour
{
    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        Button startButton = root.Q<Button>("StartButton");
        startButton.clicked += StartButton;

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
                    difficultyGroup.value = item.i;
                    ApplicationModel.difficulty = (Difficulty)difficultyGroup.value;
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
                    dominantHandGroup.value = item.i;
                    ApplicationModel.dominantHand = (DominantHand)dominantHandGroup.value;
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
                    positionGroup.value = item.i;
                    ApplicationModel.position = (Position)positionGroup.value;
                }
            });
        }
    }

    private void StartButton()
    {
        SceneManager.LoadScene("Room");
    }

    private void ExitButton()
    {
        Application.Quit();
    }


}
