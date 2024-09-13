using UnityEngine;
using UnityEngine.UIElements;

public class PickablePoints : IPickable
{
    [SerializeField] private int pointsGiven = 50;

    protected override bool OnPickUp()
    {
        playerProps.ReceivingPoints(pointsGiven);
        return false;
    }
}
