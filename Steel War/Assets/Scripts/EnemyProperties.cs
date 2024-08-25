using UnityEngine;

public class EnemyProperties : MonoBehaviour
{
    [SerializeField] PlayerProperties playerProps = null;

    private void OnValidate()
    {
        if (!playerProps) { playerProps = GameObject.Find("Player").GetComponent<PlayerProperties>(); }
    }

    // Actions
    public bool canJump = true;
    public bool canMove = true;
    public bool canAttack = true;

    // Stats
    public string enemyName = "";
    public int maxHP = 10;
    public int currentHP = 2;
    public int pointsGiven = 5;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"Collider {other} entrou no colider do {enemyName}");
        if (other.CompareTag("Attack"))
        {
            TakeDamage(1);
        }
    }


    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        Debug.Log($"O jogador atacou {enemyName}! ({currentHP}/{maxHP})HP");

        if (currentHP <= 0)
        {
            Destroy(gameObject);
            playerProps.points += pointsGiven;
        }
    }
}
