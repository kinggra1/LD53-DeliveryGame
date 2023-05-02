using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeliveryTarget : PathConnectable {

    public RectTransform hungerVisual;
    public RectTransform hungerVisualParentTransformRed;
    public RectTransform hungerVisualParentTransformBlue;
    public RectTransform hungerVisualParentTransformYellow;
    public GameObject foodBlobPrefabRed;
    public GameObject foodBlobPrefabBlue;
    public GameObject foodBlobPrefabYellow;

    public SpriteRenderer thoughtBubbleSpriteRenderer;

    public GameObject deadWormVisual;
    public GameObject livingWormVisual;

    private int desiredRedFood;
    private int desiredBlueFood;
    private int desiredYellowFood;

    private bool isHungry = false;
    private bool isDead = false;

    private float stateTimer = 0f;
    private float timeToHunger = 10f;

    private float timeToStarving = 20f;
    private float timeToDead = 40f;

    // Start is called before the first frame update
    void Start()
    {
        SatisfyHunger();

        livingWormVisual.SetActive(true);
        deadWormVisual.SetActive(false);

        livingWormVisual.transform.localScale = Vector3.zero;
        LeanTween.scale(livingWormVisual, Vector3.one * 0.6f, 0.5f).setEaseOutBack();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) {
            return;
        }

        stateTimer += Time.deltaTime;
        if (!isHungry && stateTimer > timeToHunger) {
            // BecomeHungry();
        }

        if (isHungry) {
            if (stateTimer > timeToDead) {
                // Starved to death
                isDead = true;
                hungerVisual.gameObject.SetActive(false);
                this.GetComponentInChildren<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.05f);
                this.GetComponentInChildren<Animator>().enabled = false;

                livingWormVisual.SetActive(false);
                deadWormVisual.SetActive(true);

                GameManager.Instance.AddDeadWorm();

            } else if (stateTimer > timeToStarving) {
                // Very Hungry
                this.GetComponentInChildren<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 1f);
                this.GetComponentInChildren<Animator>().speed = 0.5f;
                thoughtBubbleSpriteRenderer.color = new Color(1f, 0f, 0f, 1f);
            } else {
                // Standard Hungry
            }
        }
    }

    public bool CanBecomeHungry() {
        return !isHungry && !isDead;
    }
    public void BecomeHungry() {
        isHungry = true;
        desiredBlueFood = Random.Range(1, 3);
        // Add in other food after a certain amount of time
        if (WormSpawnManager.Instance.SecondFoodReady()) {
            desiredRedFood = Random.Range(0, 3);
        }
        stateTimer = 0f;
        UpdateUI();
        hungerVisual.gameObject.SetActive(true);
        for (int i = 0; i < desiredRedFood; i++) {
            hungerVisualParentTransformRed.gameObject.SetActive(true);
            GameObject foodBit = Instantiate(foodBlobPrefabRed, hungerVisualParentTransformRed);
            foodBit.transform.localScale = Vector3.one * 0.5f;
        }
        for (int i = 0; i < desiredBlueFood; i++) {
            hungerVisualParentTransformBlue.gameObject.SetActive(true);
            GameObject foodBit = Instantiate(foodBlobPrefabBlue, hungerVisualParentTransformBlue);
            foodBit.transform.localScale = Vector3.one * 0.5f;
        }
        for (int i = 0; i < desiredYellowFood; i++) {
            hungerVisualParentTransformYellow.gameObject.SetActive(true);
            GameObject foodBit = Instantiate(foodBlobPrefabYellow, hungerVisualParentTransformYellow);
            foodBit.transform.localScale = Vector3.one * 0.5f;
        }
    }

    private void SatisfyHunger() {
        isHungry = false;
        stateTimer = 0f;
        UpdateUI();
        hungerVisual.gameObject.SetActive(false);

        this.GetComponentInChildren<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        thoughtBubbleSpriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        this.GetComponentInChildren<Animator>().speed = 1f;

        hungerVisualParentTransformRed.gameObject.SetActive(false);
        hungerVisualParentTransformBlue.gameObject.SetActive(false);
        hungerVisualParentTransformYellow.gameObject.SetActive(false);
    }

    public override void SquirrelArrives(DeliverySquirrel squirrel) {

        if (isDead) {
            return;
        }

        if (isHungry) {
            int deliveredBlueFoodAmount = squirrel.TryGetBlueFood(desiredBlueFood);
            int deliveredRedFoodAmount = squirrel.TryGetRedFood(desiredRedFood);
            int deliveredYellowFoodAmount = squirrel.TryGetYellowFood(desiredYellowFood);

            bool ateAnything = false;
            if (deliveredBlueFoodAmount > 0) {
                desiredBlueFood -= deliveredBlueFoodAmount;
                for (int i = 0; i < deliveredBlueFoodAmount; i++) {
                    Destroy(hungerVisualParentTransformBlue.GetChild(i).gameObject);
                }
                GameManager.Instance.AddToScore(deliveredBlueFoodAmount);
                ateAnything = true;
            }

            if (deliveredRedFoodAmount > 0) {
                desiredRedFood -= deliveredRedFoodAmount;
                for (int i = 0; i < deliveredRedFoodAmount; i++) {
                    Destroy(hungerVisualParentTransformRed.GetChild(i).gameObject);
                }
                GameManager.Instance.AddToScore(deliveredRedFoodAmount);
                ateAnything = true;
            }

            if (deliveredYellowFoodAmount > 0) {
                desiredYellowFood -= deliveredYellowFoodAmount;
                for (int i = 0; i < deliveredYellowFoodAmount; i++) {
                    Destroy(hungerVisualParentTransformYellow.GetChild(i).gameObject);
                }
                GameManager.Instance.AddToScore(deliveredYellowFoodAmount);
                ateAnything = true;
            }

            if (ateAnything) {
                AudioManager.Instance.PlaySlurp();
            }

            if (desiredBlueFood == 0 && desiredRedFood == 0 && desiredYellowFood == 0) {
                SatisfyHunger();
            }
        }

        UpdateUI();
    }

    private void UpdateUI() {

    }
}
