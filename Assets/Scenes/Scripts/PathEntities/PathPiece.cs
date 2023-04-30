using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPiece : MonoBehaviour
{
    [SerializeField]
    private GameObject pathSprite;
    private PathConnectable startNode;
    private PathConnectable endNode;

    public void SetEnds(PathConnectable startNode, PathConnectable endNode) {
        this.startNode = startNode;
        this.endNode = endNode;
        pathSprite.SetActive(true);

        Vector2 startToEnd = endNode.transform.position - startNode.transform.position;

        float distance = startToEnd.magnitude;
        pathSprite.transform.position = ((Vector2)startNode.transform.position + startToEnd.normalized * distance / 2f);
        pathSprite.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Rad2Deg * Mathf.Atan2(startToEnd.y, startToEnd.x));

        pathSprite.transform.localScale = new Vector3(distance, 1f, 1f);
    }

    public PathConnectable StartNode() {
        return this.startNode;
    }

    public PathConnectable EndNode() {
        return this.endNode;
    }

    public bool AtEndOfPath(Vector2 evaluatedPosition) {
        return Vector2.Distance(evaluatedPosition, endNode.transform.position) < 0.01f;
    }

    public Vector2 LerpAtSpeed(float speed, float time) {
        float totalTravelTime = Vector2.Distance(endNode.transform.position, startNode.transform.position);
        float percent = speed * time / totalTravelTime;
        return Vector2.Lerp(startNode.transform.position, endNode.transform.position, percent);
    }

    private void Update() {
        Debug.DrawLine(startNode.transform.position, endNode.transform.position, Color.red);
    }
}
