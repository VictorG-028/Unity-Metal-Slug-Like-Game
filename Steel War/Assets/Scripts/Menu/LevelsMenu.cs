using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelsMenu : MonoBehaviour
{
    public Button[] buttons;

    private void Awake()
    {
        int unlockedlevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        for (int i = 0; i < buttons.Length; i++)
        {
            //buttons[i].interactable = false;
            buttons[i].interactable = GameStateStatic.IsLevelCompleted(i+1);
        }
        for (int i = 0; i < unlockedlevel; i++)
        {
            //print($"{unlockedlevel} = unlockedlevel | IsLevelCompleted(1) = {GameStateStatic.IsLevelCompleted(1)}");
            buttons[i].interactable = true;
        }
    }
    
    public void OpenLevel(int levelId)
    {
        string levelName = "Stage_" + levelId;
        SceneManager.LoadScene(levelName);
    }
}
