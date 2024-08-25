using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HpController : MonoBehaviour
{
    [SerializeField] PlayerProperties playerProps = null;
    [SerializeField] Image[] containers = null;
    [SerializeField] Sprite emptyBar = null;
    [SerializeField] Sprite fullBar = null;

    private void OnValidate()
    {
        if (!playerProps) { playerProps = GameObject.Find("Player").GetComponent<PlayerProperties>(); }
        // TODO: Criar os Health UI Container (GameObject) na cena contendo as barras de vida como GameObject filhos
        if (containers == null || containers.Length == 0)
        {
            containers = GameObject.FindGameObjectsWithTag("Health UI Container")
                                   .Select(x => x.GetComponent<Image>()).ToArray();
        }
        // TODO: fazer essas linhas carregarem automaticamente as sprites
        if (!emptyBar) { emptyBar = Resources.Load<Sprite>("TODO"); }
        if (!fullBar) { fullBar = Resources.Load<Sprite>("TODO"); }
    }

    void Start()
    {
        UpdatePlayerUI();
    }

    public void UpdatePlayerUI()
    {
        int fullContainersAmmount = playerProps.HP / 2; // implicit convert float to int removes the 0.5 (floor operation)
        int halfFullContainersAmmount = playerProps.HP % 2;

        for (int i = 0; i < fullContainersAmmount; i++)
        { containers[i].sprite = fullBar; }

        for (int i = fullContainersAmmount + halfFullContainersAmmount; i < containers.Length; i++)
        { containers[i].sprite = emptyBar; }
    }
}
