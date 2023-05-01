using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodBlob : MonoBehaviour
{
    public enum FoodColor { RED, BLUE, YELLOW }
    public FoodColor foodColor = FoodColor.BLUE;

    public GameObject redSprite;
    public GameObject blueSprite;
    public GameObject yellowSprite;

    private static readonly float TWEEN_RATE = 3f;
    private Vector2 targetPos;

    private void Start() {
        SetColor(foodColor);
    }

    public void SetColor(FoodColor color) {
        this.foodColor = color;

        redSprite.SetActive(false);
        blueSprite.SetActive(false);
        yellowSprite.SetActive(false);

        switch (foodColor) {
            case FoodColor.RED:
                redSprite.SetActive(true);
                break;
            case FoodColor.BLUE:
                blueSprite.SetActive(true);
                break;
            case FoodColor.YELLOW:
                yellowSprite.SetActive(true);
                break;
        }
    }

    public FoodColor GetColor() {
        return foodColor;
    }

    public void SetTargetPos(Vector2 targetPos) {
        this.targetPos = targetPos;
    }

    private void Update() {
        this.transform.position = Vector2.Lerp(this.transform.position, targetPos, TWEEN_RATE * Time.deltaTime);
    }
}
