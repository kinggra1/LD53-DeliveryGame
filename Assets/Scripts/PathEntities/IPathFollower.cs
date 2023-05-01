using System.Collections;
using UnityEngine;

public interface IPathFollower {

    public void StartOnPath(PathPiece pathPiece) {
        StartOnPath(pathPiece, 0f);
    }

    public abstract void StartOnPath(PathPiece pathPiece, float timeOnPathSoFar);

    public abstract void SetCurrentNode(PathConnectable node);

    public abstract float CurrentSpeed();
    public abstract float TimeOnPathSegment();
}