using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyScriptable", menuName = "Enemy", order = 1)]
public class EnemyScriptable : ScriptableObject
{
	[SerializeField] private int maxHP = 10;
	[SerializeField] private int hpIncreasePerStage = 2;
	[SerializeField] private int pointsGiven = 1000;
	[SerializeField] private ShootingPattern shootingPattern = ShootingPattern.Straight;

	// Getter
	public int MaxHP
    {
      get { return maxHP; }
    }

    public int PointsGiven
    {
        get {return pointsGiven;}
    }
  
  public int GetMaxHPForStage(int stage)
  {
      // Calcula o HP para o estágio especificado
      int increasedHP = maxHP + (hpIncreasePerStage * (stage - 1));
      return increasedHP;
  }
  
  public ShootingPattern Pattern
  {
        get { return shootingPattern; }
  }
}

public enum ShootingPattern
{
    NoPattern,         // Quando o inimigo não atira
    Straight,          // Tiro direto
    Spread,            // Tiro espalhado
    Burst,             // Disparo em rajada
    StearringBehavior, // Disparo do sniper
    Circular,          // Disparo em círculo
    Random,            // Tiro aleatório
    Tank,
    Helicopter,    
}