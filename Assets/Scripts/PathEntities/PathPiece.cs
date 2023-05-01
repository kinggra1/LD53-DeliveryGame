using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPiece : MonoBehaviour
{
    [SerializeField]
    private GameObject pathSprite;
    [SerializeField]
    private SpriteMask spriteMask;

    [SerializeField]
    private bool flammable;
    private bool onFire;

    public GameObject firePrefab;
    public IPathFollower entityOnPath;

    private PathConnectable startNode;
    private PathConnectable endNode;

    public void SetEnds(PathConnectable startNode, PathConnectable endNode) {
        this.startNode = startNode;
        this.endNode = endNode;
        pathSprite.SetActive(true);
        RescalePathToEnds();
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

    // If the path is on fire and we're over halfway across, game over.
    public bool KillsPlayer(float speed, float time) {
        return onFire && (time / TotalPathTimeAtSpeed(speed) > 0.5f); 
    }

    public float TotalPathTimeAtSpeed(float speed) {
        return Vector2.Distance(endNode.transform.position, startNode.transform.position) / speed;
    }

    public Vector2 LerpAtSpeed(float speed, float time) {
        float totalTravelTime = TotalPathTimeAtSpeed(speed);
        float percent = time / totalTravelTime;
        return Vector2.Lerp(startNode.transform.position, endNode.transform.position, percent);
    }

    public bool IsOnFire() {
        return onFire;
    }

    public bool IsFlammable() {
        return flammable;
    }

    public void SetOnFire() {
        GameObject fire = Instantiate(firePrefab, this.transform);
        fire.transform.position = GetCenteredPosition();
        fire.transform.localScale = Vector3.zero;
        LeanTween.scale(fire, Vector3.one * 2f, 0.2f).setEaseInCubic()
            .setOnComplete(() => {
                LeanTween.scale(fire, Vector3.one * 1.3f, 0.5f);
            });

        onFire = true;
    }

    private void Update() {
        Debug.DrawLine(startNode.transform.position, endNode.transform.position, Color.red);
    }

    private Vector2 GetCenteredPosition() {
        Vector2 startToEnd = endNode.transform.position - startNode.transform.position;
        float distance = startToEnd.magnitude;
        return ((Vector2)startNode.transform.position + startToEnd.normalized * distance / 2f);
    }

    public void RescalePathToEnds() {
        Vector2 startToEnd = endNode.transform.position - startNode.transform.position;

        float distance = startToEnd.magnitude;
        Vector2 centeredPosition = GetCenteredPosition();
        Quaternion xAlignedRotation = Quaternion.Euler(0f, 0f, Mathf.Rad2Deg * Mathf.Atan2(startToEnd.y, startToEnd.x));

        pathSprite.transform.position = centeredPosition;
        pathSprite.transform.rotation = xAlignedRotation;

        // pathSprite.transform.localScale = new Vector3(distance / 10f, 1f, 1f);
        // spriteMask.transform.position = centeredPosition;
        // spriteMask.transform.rotation = xAlignedRotation;
        spriteMask.transform.localScale = new Vector3(0.6f, distance / 2f, 1f);
    }
}
