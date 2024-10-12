using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationModel : MonoBehaviour
{
    static public Difficulty difficulty = Difficulty.Easy;
    static public DominantHand dominantHand = DominantHand.Left;
    static public Position position = Position.Sitting;
    static public bool isVR = false;
    static public bool isPaused = false;

    static public void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;
    }
    static public void UnPauseGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
    }
}

public enum Difficulty
{
    Easy,
    Medium,
    Hard
}

public enum DominantHand
{
    Left,
    Right
}

public enum Position
{
    Sitting,
    Standing
}