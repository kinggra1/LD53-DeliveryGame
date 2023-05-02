using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NutPile : PathConnectable
{
    public DeliverySquirrel deliverySquirrel;

    public FoodBlob.FoodColor foodColor = FoodBlob.FoodColor.BLUE;

    // Start is called before the first frame update
    void Start()
    {
        if (Time.timeSinceLevelLoad < 10f) {
            deliverySquirrel = FindObjectOfType<DeliverySquirrel>();
            deliverySquirrel.transform.position = this.transform.position;
            deliverySquirrel.SetCurrentNode(this);
        }

        this.transform.localScale = Vector3.zero;
        LeanTween.scale(gameObject, Vector3.one * 0.6f, 2f).setEaseOutSine();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void SquirrelArrives(DeliverySquirrel squirrel) {
        int pickupAmount = Mathf.Max(squirrel.MaxFoodAmount(foodColor) - squirrel.HeldFoodAmount(foodColor), 0);
        squirrel.PickUpFood(pickupAmount, foodColor);
    }

    public override void SquirrelLeaves(DeliverySquirrel squirrel) {
        int pickupAmount = Mathf.Max(squirrel.MaxFoodAmount(foodColor) - squirrel.HeldFoodAmount(foodColor), 0);
        if (pickupAmount > 0) {
            squirrel.PickUpFood(pickupAmount, foodColor);
        }
    }

}
