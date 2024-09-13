using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//classe para lidar com variaveis static, que devem ser atualizadas e compartilhadas
// entre classes
public class GameStateStatic : MonoBehaviour
{
    static public string currentLevel;
    static public Dictionary<int, bool> completedLevels = new();


    public static void CompleteLevel(int levelIndex)
    {
        if (!completedLevels.ContainsKey(levelIndex))
        {
            completedLevels.Add(levelIndex, true);
        }
    }

    public static bool IsLevelCompleted(int levelIndex)
    {
        return completedLevels.ContainsKey(levelIndex) && completedLevels[levelIndex];
    }
}
