using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ESCMenuUI : MonoBehaviour
{
    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        Button continueButton = root.Q<Button>("ContinueButton");
        continueButton.clicked += ContinueButton;

        Button exitButton = root.Q<Button>("ExitButton");
        exitButton.clicked += ExitToMainMenuButton;
    }

    public void ContinueButton()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
    public void ExitToMainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
    }

    public void ExitToDesktopButton()
    {
        Application.Quit();
    }
}
