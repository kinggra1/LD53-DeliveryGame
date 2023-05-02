using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeliverySquirrel : MonoBehaviour, IPathFollower
{
    private static readonly float FOOD_BLOB_FLOAT_DIST = 1.4f;

    public TMP_Text heldFoodText;
    public GameObject foodBlobPrefab;
    public Transform foodBlobParent;

    public GameObject visualSprite;

    private PathPiece currentPath;
    private PathConnectable currentNode;
    private float timeOnPath = 0f;

    private float travelSpeed = 2.8f;

    private int maxRedFood = 3;
    private int maxBlueFood = 3;
    private int maxYellowFood = 3;
    private int maxTotalFood = 5;
    private int heldRedFood = 0;
    private int heldBlueFood = 0;
    private int heldYellowFood = 0;

    private Vector3 startingScale;

    private List<FoodBlob> foodBlobs = new List<FoodBlob>();

    // Start is called before the first frame update
    void Start()
    {
        startingScale = visualSprite.transform.localScale;
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
            if (currentPath.KillsPlayer(CurrentSpeed(), timeOnPath)) {
                GameManager.Instance.GameOver("Burnt up in a fire!");
            }

            if (currentPath.IsLeftPath()) {
                Vector3 facingScale = startingScale;
                facingScale.x = startingScale.x * -1f;
                visualSprite.transform.localScale = facingScale;
            } else {
                visualSprite.transform.localScale = startingScale;
            }

            // Move along the current path
            this.transform.position = currentPath.LerpAtSpeed(CurrentSpeed(), timeOnPath);

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

    public int TryGetBlueFood(int amount) {
        int finalAmount = Mathf.Min(heldBlueFood, amount);
        heldBlueFood -= finalAmount;
        int numProcessed = 0;
        // Iterate in reverse so we can remove as we go.
        for (int i = foodBlobs.Count - 1; i >= 0; i--) {
            if (numProcessed == finalAmount) {
                break;
            }

            FoodBlob blob = foodBlobs[i];
            if (blob.GetColor() == FoodBlob.FoodColor.BLUE) {
                GameObject targetBlob = blob.gameObject;
                LeanTween.move(targetBlob, this.transform.position + Vector3.up, 0.2f)
                    .setEaseInCubic()
                    .setOnComplete(
                        () => { Destroy(targetBlob); }
                    );
                foodBlobs.RemoveAt(i);
                numProcessed++;
            }
        }
        return finalAmount;
    }

    public int TryGetRedFood(int amount) {
        int finalAmount = Mathf.Min(heldRedFood, amount);
        heldRedFood -= finalAmount;
        int numProcessed = 0;
        // Iterate in reverse so we can remove as we go.
        for (int i = foodBlobs.Count - 1; i >= 0; i--) {
            if (numProcessed == finalAmount) {
                break;
            }

            FoodBlob blob = foodBlobs[i];
            if (blob.GetColor() == FoodBlob.FoodColor.RED) {
                GameObject targetBlob = blob.gameObject;
                LeanTween.move(targetBlob, this.transform.position + Vector3.up, 0.2f)
                    .setEaseInCubic()
                    .setOnComplete(
                        () => { Destroy(targetBlob); }
                    );
                foodBlobs.RemoveAt(i);
                numProcessed++;
            }
        }
        return finalAmount;
    }

    public int TryGetYellowFood(int amount) {
        int finalAmount = Mathf.Min(heldYellowFood, amount);
        heldYellowFood -= finalAmount;
        int numProcessed = 0;
        // Iterate in reverse so we can remove as we go.
        for (int i = foodBlobs.Count - 1; i >= 0; i--) {
            if (numProcessed == finalAmount) {
                break;
            }

            FoodBlob blob = foodBlobs[i];
            if (blob.GetColor() == FoodBlob.FoodColor.YELLOW) {
                GameObject targetBlob = blob.gameObject;
                LeanTween.move(targetBlob, this.transform.position + Vector3.up, 0.2f)
                    .setEaseInCubic()
                    .setOnComplete(
                        () => { Destroy(targetBlob); }
                    );
                foodBlobs.RemoveAt(i);
                numProcessed++;
            }
        }
        return finalAmount;
    }

    public void PickUpFood(int amount, FoodBlob.FoodColor foodColor) {
        switch (foodColor) {
            case FoodBlob.FoodColor.RED:
                heldRedFood += amount;
                break;
            case FoodBlob.FoodColor.BLUE:
                heldBlueFood += amount;
                break;
            case FoodBlob.FoodColor.YELLOW:
                heldYellowFood += amount;
                break;
        }

        for (int i = 0; i < amount; i++) {
            GameObject foodObject = Instantiate(foodBlobPrefab, foodBlobParent);
            FoodBlob foodBlob = foodObject.GetComponent<FoodBlob>();
            foodBlob.SetColor(foodColor);
            foodBlobs.Add(foodBlob);
        }
    }

    public int HeldFoodAmount(FoodBlob.FoodColor desiredColor) {
        switch (desiredColor) {
            case FoodBlob.FoodColor.RED:
                return heldRedFood;
            case FoodBlob.FoodColor.BLUE:
                return heldBlueFood;
            case FoodBlob.FoodColor.YELLOW:
                return heldYellowFood;
        }

        return 0;
    }

    public int MaxFoodAmount(FoodBlob.FoodColor desiredColor) {
        switch (desiredColor) {
            case FoodBlob.FoodColor.RED:
                return maxRedFood;
            case FoodBlob.FoodColor.BLUE:
                return maxBlueFood;
            case FoodBlob.FoodColor.YELLOW:
                return maxYellowFood;
        }

        return 0;
    }

    public int MaxTotalFoodAmount() {
        return maxTotalFood;
    }

    public int TotalFoodAmount() {
        return heldRedFood + heldBlueFood + heldYellowFood;
    }

    private void UpdateUI() {
        // heldFoodText.text = "I have: " + heldFood;

        for (int i = 0; i < foodBlobs.Count; i++) {
            float radialProgress = Mathf.PI * 2 * ((float)i / foodBlobs.Count) + Time.time * 2f;
            Vector2 targetPosition = (Vector2)foodBlobParent.position + new Vector2(Mathf.Cos(radialProgress), Mathf.Sin(radialProgress)) * FOOD_BLOB_FLOAT_DIST;
            foodBlobs[i].SetTargetPos(targetPosition);
        }
    }

    public float CurrentSpeed() {
        return travelSpeed + 2f * GameManager.Instance.ExpertStatus();
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
