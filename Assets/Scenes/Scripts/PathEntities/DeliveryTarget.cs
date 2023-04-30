using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeliveryTarget : PathConnectable {

    public TMP_Text hungerText;

    private int desiredFoodAmount;
    private bool isHungry = false;

    private float stateTimer = 0f;
    private float timeToHunger = 10f;

    // Start is called before the first frame update
    void Start()
    {
        SatisfyHunger();
    }

    // Update is called once per frame
    void Update()
    {
        stateTimer += Time.deltaTime;
        if (!isHungry && stateTimer > timeToHunger) {
            BecomeHungry();
        }
    }

    private void BecomeHungry() {
        isHungry = true;
        desiredFoodAmount = 2;
        stateTimer = 0f;
        UpdateUI();
        hungerText.enabled = true;
    }

    private void SatisfyHunger() {
        isHungry = false;
        stateTimer = 0f;
        UpdateUI();
        hungerText.enabled = false;
    }

    public override void SquirrelVisit(DeliverySquirrel squirrel) {

        if (isHungry) {
            int deliveredFoodAmount = squirrel.TryGetFood(desiredFoodAmount);
            desiredFoodAmount -= deliveredFoodAmount;
            UpdateUI();

            if (desiredFoodAmount == 0) {
                SatisfyHunger();
            }
        }
    }

    private void UpdateUI() {
        hungerText.text = "Hungry for: " + desiredFoodAmount;
    }
}
