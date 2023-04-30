using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeliverySquirrel : MonoBehaviour
{
    private static readonly float FOOD_BLOB_FLOAT_DIST = 1.7f;

    public TMP_Text heldFoodText;
    public GameObject foodBlobPrefab;
    public Transform foodBlobParent;

    private PathPiece currentPath;
    private float timeOnPath = 0f;

    private float travelSpeed = 6f;

    private int maxFood = 8;
    private int heldFood = 0;

    private List<FoodBlob> foodBlobs = new List<FoodBlob>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsPlaying()) {
            return;
        }

        UpdateUI();

        if (currentPath == null) {
            return;
        }

        timeOnPath += Time.deltaTime;
        this.transform.position = currentPath.LerpAtSpeed(travelSpeed, timeOnPath);

        if (currentPath.AtEndOfPath(this.transform.position)) {
            PathPiece nextSegment = currentPath.EndNode().outgoingPath;
            if (nextSegment) {
                currentPath.EndNode().SquirrelVisit(this);
                StartOnPath(nextSegment);
            }
        }
    }

    public int TryGetFood(int amount) {
        int finalAmount = Mathf.Min(heldFood, amount);
        heldFood -= finalAmount;
        for (int i = 0; i < finalAmount; i++) {
            Destroy(foodBlobs[i].gameObject);
        }
        foodBlobs.RemoveRange(0, finalAmount);
        return finalAmount;
    }

    public void PickUpFood(int amount) {
        heldFood += amount;
        for (int i = 0; i < amount; i++) {
            GameObject foodObject = Instantiate(foodBlobPrefab, foodBlobParent);
            FoodBlob foodBlob = foodObject.GetComponent<FoodBlob>();
            foodBlobs.Add(foodBlob);
        }
    }

    public int HeldFoodAmount() {
        return heldFood;
    }

    public int MaxFoodAmount() {
        return maxFood;
    }

    public void StartOnPath(PathPiece pathPiece) {
        timeOnPath = 0f;
        this.transform.position = pathPiece.StartNode().transform.position;
        currentPath = pathPiece;
    }

    private void UpdateUI() {
        heldFoodText.text = "I have: " + heldFood;

        for (int i = 0; i < foodBlobs.Count; i++) {
            float radialProgress = Mathf.PI * 2 * ((float)i / foodBlobs.Count) + Time.time * 2f;
            Vector2 targetPosition = (Vector2)foodBlobParent.position + new Vector2(Mathf.Cos(radialProgress), Mathf.Sin(radialProgress)) * FOOD_BLOB_FLOAT_DIST;
            foodBlobs[i].SetTargetPos(targetPosition);
        }
    }
}
