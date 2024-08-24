using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CarregarScene()
    {
       if(!String.IsNullOrEmpty(GameStateStatic.currentLevel))
        {
            SceneManager.LoadScene(GameStateStatic.currentLevel);
        }
        
    }

    public void SairJogo()
    {
        Application.Quit();
    }
}
