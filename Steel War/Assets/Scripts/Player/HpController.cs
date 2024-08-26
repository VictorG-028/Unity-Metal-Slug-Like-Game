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

        if (containers == null || containers.Length == 0)
        {
            containers = GameObject.FindGameObjectsWithTag("HealthBar")
                                .Select(x => x.GetComponent<Image>()).ToArray();
        }

        if (!emptyBar) { emptyBar = Resources.Load<Sprite>("Assets/Sprites/EmptyBar.png"); }
        if (!fullBar) { fullBar = Resources.Load<Sprite>("Assets/Sprites/FullBar.png"); } 
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
