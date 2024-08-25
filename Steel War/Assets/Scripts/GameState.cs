using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{
    [SerializeField] GameObject player = null;
    [SerializeField] PlayerProperties playerProps = null;
    [SerializeField] Vector3 startPosition = Vector3.zero;
    [SerializeField] TextMeshProUGUI pauseText = null;
 

    private void OnValidate()
    {
        if (!player) { player = GameObject.Find("Player"); }
        if (!playerProps) { playerProps = player.GetComponent<PlayerProperties>(); }
        if (startPosition == Vector3.zero) { startPosition = player.transform.position; }
        if (!pauseText) { pauseText = GameObject.Find("Pause Text TMP").GetComponent<TextMeshProUGUI>(); }
    }

    private void Start()
    {
        //acessa variavel static para manipulacao das fases e atualiza fase atual
        GameStateStatic.currentLevel = "stage_1";
    }

    void Update()
    {
        PauseAction();
    }

    private void PauseAction()
    {
        if (playerProps.canPause && Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = Time.timeScale == 0 ? 1 : 0;
            pauseText.text = pauseText.text.Equals("Pause") ? "" : "Pause";
        }
    }

    public void Restart()
    {
        // Go To Death Screen Scene
        //SceneManager.LoadScene(2); // TODO: fazer cena de morte com botão de restart
    }
}
