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
            // If something is navigating the current path, let it finish before destroying
            if (incomingPath.entityOnPath != null) {
                IPathFollower pathFollower = incomingPath.entityOnPath;
                // Make path unclickable
                incomingPath.GetComponentInChildren<Collider2D>().enabled = false;
                // Make path to remove semi-transparent
                incomingPath.GetComponentInChildren<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.2f);
                float followerSpeed = pathFollower.CurrentSpeed();
                float timeToDeath = incomingPath.TotalPathTimeAtSpeed(followerSpeed) - pathFollower.TimeOnPathSegment();
                LeanTween.delayedCall(timeToDeath + 0.2f, () => {
                    Destroy(incomingPath.gameObject);
                    incomingPath = incoming;
                });
            } else {
                // Otherwise immediately destroy and re-set
                Destroy(incomingPath.gameObject);
                incomingPath = incoming;
            }
        } else {
            incomingPath = incoming;
        }
    }

    public virtual void ConnectOutgoing(PathPiece outgoing) {
        if (outgoingPath) {
            // If something is navigating the current path, let it finish before destroying
            if (outgoingPath.entityOnPath != null) {
                IPathFollower pathFollower = outgoingPath.entityOnPath;
                // Make path unclickable
                outgoingPath.GetComponentInChildren<Collider2D>().enabled = false;
                // Make path to remove semi-transparent
                outgoingPath.GetComponentInChildren<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.2f);
                float followerSpeed = pathFollower.CurrentSpeed();
                float timeToDeath = outgoingPath.TotalPathTimeAtSpeed(followerSpeed) - pathFollower.TimeOnPathSegment();
                LeanTween.delayedCall(timeToDeath + 0.2f, () => {
                    Destroy(outgoingPath.gameObject);
                    outgoingPath = outgoing;
                });
            } else {
                // Otherwise immediately destroy and re-set
                Destroy(outgoingPath.gameObject);
                outgoingPath = outgoing;
            }
        } else {
            outgoingPath = outgoing;
        }
    }

    public virtual void SquirrelVisit(DeliverySquirrel squirrel) {

    }

    public virtual void SquirrelArrives(DeliverySquirrel squirrel) {

    }

    public virtual void SquirrelLeaves(DeliverySquirrel squirrel) {

    }
}
