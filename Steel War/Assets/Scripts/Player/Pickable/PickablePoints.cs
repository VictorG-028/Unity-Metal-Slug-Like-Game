using UnityEngine;
using UnityEngine.UIElements;

public class PickablePoints : IPickable
{
    [SerializeField] private int pointsGiven = 50;

    protected override void OnPickUp()
    {
        playerProps.ReceivingPoints(pointsGiven);
    }
}
