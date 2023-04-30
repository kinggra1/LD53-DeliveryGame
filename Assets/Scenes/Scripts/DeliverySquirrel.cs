using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeliverySquirrel : MonoBehaviour
{
    public TMP_Text heldFoodText;

    private PathPiece currentPath;
    private float timeOnPath = 0f;

    private float travelSpeed = 6f;

    private int maxFood = 3;
    private int heldFood = 3;

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
        UpdateUI();
        return finalAmount;
    }

    public void PickUpFood(int amount) {
        heldFood += amount;
        UpdateUI();
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
    }
}
