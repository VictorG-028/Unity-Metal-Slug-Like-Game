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
    public EnemyScriptable enemyScriptable;
    public string enemyName = "";
    public int currentHP = 999999;
    
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
        if(currentHP > enemyScriptable.MaxHP)
        {
            currentHP = enemyScriptable.MaxHP;
        }
        currentHP -= damage;
        
        Debug.Log($"O jogador atacou {enemyName}! ({currentHP}/{enemyScriptable.MaxHP})HP");

        if (currentHP <= 0)
        {
            Destroy(gameObject);
            playerProps.ReceivingPoints(enemyScriptable.PointsGiven);
        }
    }
}
