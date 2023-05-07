using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenMenuScript : MonoBehaviour
{
    GameObject canvas;

    private void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetSceneByName("ESCMenu").name == null)
            {
                SceneManager.LoadScene("ESCMenu", LoadSceneMode.Additive);
                Time.timeScale = 0f;
            }
            else
            {
                if (!canvas)
                {
                    canvas = FindObjectsOfType<Canvas>(true).Where(obj => obj.name == "ESCMenuCanvas").ToArray()[0].gameObject;

                }

                if (!canvas.activeInHierarchy)
                {
                    canvas.SetActive(true);
                    Time.timeScale = 0f;
                } else
                {
                    canvas.SetActive(false);
                    Time.timeScale = 1f;
                }
            }
        }
    }

    public void ContinueButton()
    {
        if (!canvas)
        {
            canvas = GameObject.Find("ESCMenuCanvas");
        }

        canvas.SetActive(false);
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
