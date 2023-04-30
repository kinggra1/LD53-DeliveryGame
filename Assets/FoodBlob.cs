using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodBlob : MonoBehaviour
{
    private static readonly float TWEEN_RATE = 3f;
    private Vector2 targetPos;

    public void SetTargetPos(Vector2 targetPos) {
        this.targetPos = targetPos;
    }

    private void Update() {
        this.transform.position = Vector2.Lerp(this.transform.position, targetPos, TWEEN_RATE * Time.deltaTime);
    }
}
