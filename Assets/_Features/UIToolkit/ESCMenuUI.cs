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

        if (!ApplicationModel.isVR)
        {
            ApplicationModel.PauseGame();
        }
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
    }

    public void ContinueButton()
    {
        gameObject.SetActive(false);
        ApplicationModel.UnPauseGame();
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }
    public void ExitToMainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
        ApplicationModel.UnPauseGame();
    }

    public void ExitToDesktopButton()
    {
        Application.Quit();
    }
}
