using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeliverySquirrel : MonoBehaviour, IPathFollower
{
    private static readonly float FOOD_BLOB_FLOAT_DIST = 1.7f;

    public TMP_Text heldFoodText;
    public GameObject foodBlobPrefab;
    public Transform foodBlobParent;

    private PathPiece currentPath;
    private PathConnectable currentNode;
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

        timeOnPath += Time.deltaTime;

        if (currentPath != null) {
            if (currentPath.KillsPlayer(travelSpeed, timeOnPath)) {
                GameManager.Instance.GameOver();
            }

            // Move along the current path
            this.transform.position = currentPath.LerpAtSpeed(travelSpeed, timeOnPath);

            if (currentPath.AtEndOfPath(this.transform.position)) {
                currentPath.entityOnPath = null;
                currentNode = currentPath.EndNode();
                currentNode.SquirrelArrives(this);
            }
        }

        // If the current node has an outgoing path at any point, start following it.
        if (currentNode != null) {
            PathPiece nextSegment = currentNode.outgoingPath;
            if (nextSegment) {
                currentNode.SquirrelLeaves(this);
                ((IPathFollower)this).StartOnPath(nextSegment);
                currentNode = null;
            }
        }
    }

    public int TryGetFood(int amount) {
        int finalAmount = Mathf.Min(heldFood, amount);
        heldFood -= finalAmount;
        for (int i = 0; i < finalAmount; i++) {
            GameObject targetBlob = foodBlobs[i].gameObject;
            LeanTween.move(targetBlob, this.transform.position + Vector3.up, 0.2f)
                .setEaseInCubic()
                .setOnComplete(
                    () => { Destroy(targetBlob); }
                );
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

    private void UpdateUI() {
        heldFoodText.text = "I have: " + heldFood;

        for (int i = 0; i < foodBlobs.Count; i++) {
            float radialProgress = Mathf.PI * 2 * ((float)i / foodBlobs.Count) + Time.time * 2f;
            Vector2 targetPosition = (Vector2)foodBlobParent.position + new Vector2(Mathf.Cos(radialProgress), Mathf.Sin(radialProgress)) * FOOD_BLOB_FLOAT_DIST;
            foodBlobs[i].SetTargetPos(targetPosition);
        }
    }

    public float CurrentSpeed() {
        return travelSpeed;
    }

    public float TimeOnPathSegment() {
        return timeOnPath;
    }

    public void StartOnPath(PathPiece pathPiece, float timeOnPathSoFar) {
        timeOnPath = timeOnPathSoFar;
        this.transform.position = pathPiece.StartNode().transform.position;
        pathPiece.entityOnPath = this;
        currentPath = pathPiece;
    }

    public void SetCurrentNode(PathConnectable node) {
        this.currentNode = node;
    }

    public PathPiece GetCurrentPath() {
        return currentPath;
    }
}
