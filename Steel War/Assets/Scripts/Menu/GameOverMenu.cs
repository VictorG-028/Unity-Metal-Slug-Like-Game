using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{

    public void CarregarScene()
    {
       if(!String.IsNullOrEmpty(GameStateStatic.currentLevel))
        {
            SceneManager.LoadScene(GameStateStatic.currentLevel);
        }
        
    }

    public void VoltarMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void SairJogo()
    {
        Application.Quit();
    }
}
