using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationModel : MonoBehaviour
{
    static public Difficulty difficulty = Difficulty.Easy;
    static public DominantHand dominantHand = DominantHand.Right;
    static public Position position = Position.Sitting;
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