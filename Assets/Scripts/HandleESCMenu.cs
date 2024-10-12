using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class HandleESCMenu : MonoBehaviour
{
    GameObject canvas;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetSceneByName("ESCMenu").name == null)
            {
                SceneManager.LoadScene("ESCMenu", LoadSceneMode.Additive);
            }
            else
            {
                if (!canvas)
                {
                    canvas = FindObjectOfType<UIDocument>(true).gameObject;
                }

                if (!canvas.activeInHierarchy)
                {
                    ApplicationModel.PauseGame();
                    canvas.SetActive(true);
                }
                else
                {
                    ApplicationModel.UnPauseGame();
                    canvas.SetActive(false);
                    if (ApplicationModel.isVR)
                    {
                        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                        UnityEngine.Cursor.visible = false;
                    }
                }
            }
        }
    }
}
