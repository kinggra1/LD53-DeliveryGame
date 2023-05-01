using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NutPile : PathConnectable
{
    public DeliverySquirrel deliverySquirrel;

    // Eventually turn this into a map I guess?
    private int foodCount = 20;

    // Start is called before the first frame update
    void Start()
    {
        deliverySquirrel.transform.position = this.transform.position;
        deliverySquirrel.SetCurrentNode(this);


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddFood(int amount) {

    }

    public override void SquirrelArrives(DeliverySquirrel squirrel) {
        int pickupAmount = Mathf.Max(squirrel.MaxFoodAmount() - squirrel.HeldFoodAmount(), 0);
        squirrel.PickUpFood(pickupAmount);
    }
    
}
