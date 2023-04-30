using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathConnectable : MonoBehaviour
{
    public PathPiece incomingPath;
    public PathPiece outgoingPath;

    public void OnClick() {
        Debug.Log("Clicky clicky");
        // Remove outgoing path, or maybe set it to be partially transparent
    }

    public void ConnectIncoming(PathPiece incoming) {
        if (incomingPath) {
            Destroy(incomingPath.gameObject);
        }
        incomingPath = incoming;
    }

    public virtual void ConnectOutgoing(PathPiece outgoing) {
        if (outgoingPath) {
            Destroy(outgoingPath.gameObject);
        }
        outgoingPath = outgoing;
    }

    public virtual void SquirrelVisit(DeliverySquirrel squirrel) {

    }

    public void SquirrelArrives() {

    }

    public void SquirrelLeaves() {

    }
}
