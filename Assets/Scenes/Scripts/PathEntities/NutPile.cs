using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NutPile : PathConnectable
{
    public DeliverySquirrel deliverySquirrel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ConnectOutgoing(PathPiece outgoing) {
        base.ConnectOutgoing(outgoing);
        deliverySquirrel.StartOnPath(outgoing);
    }

    public override void SquirrelVisit(DeliverySquirrel squirrel) {
        int pickupAmount = Mathf.Max(squirrel.MaxFoodAmount() - squirrel.HeldFoodAmount(), 0);
        squirrel.PickUpFood(pickupAmount);
    }
    
}
