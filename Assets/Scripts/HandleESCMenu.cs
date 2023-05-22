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
                    canvas.SetActive(true);
                    Time.timeScale = 0f;
                }
                else
                {
                    canvas.SetActive(false);
                    Time.timeScale = 1f;
                }
            }
        }
    }
}
