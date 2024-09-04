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
                                .Select(x => x.GetComponent<Image>())
                                .ToArray();
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
        for (int i = 0; i < playerProps.maxHP; i++)
        {
            if (i < playerProps.HP)
            {
                containers[i].sprite = fullBar;
            }
            else
            {
                containers[i].sprite = emptyBar;
            }
            
        }
    }
}
