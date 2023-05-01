using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : Singleton<ClickManager> {
    public Camera mainCamera;

    public GameObject pathPiecePrefab;

    private PathConnectable firstClickedNode;
    private PathPiece firstClickedPath;

    // Fake path pieces used to draw a placeholder path piece while making connections.
    private PathPiece tempPathVisualToMouse;
    private PathPiece tempPathVisualFromMouse;
    private PathConnectable invisibleMousePlaceholder;

    void Awake() {
        GameObject fakePathVisualHolderToMouse = Instantiate(pathPiecePrefab, this.transform);
        fakePathVisualHolderToMouse.name = "FakePathVisualToMouse";
        tempPathVisualToMouse = fakePathVisualHolderToMouse.GetComponent<PathPiece>();
        // Disable colliders so we cannot click on these fake placeholder paths.
        fakePathVisualHolderToMouse.GetComponentInChildren<Collider2D>(/* includeInactive= */ true).enabled = false;

        GameObject fakePathVisualHolderFromMouse = Instantiate(pathPiecePrefab, this.transform);
        fakePathVisualHolderFromMouse.name = "FakePathVisualFromMouse";
        tempPathVisualFromMouse = fakePathVisualHolderFromMouse.GetComponent<PathPiece>();
        fakePathVisualHolderFromMouse.GetComponentInChildren<Collider2D>(/* includeInactive= */ true).enabled = false;

        // Doesn't matter what object this is on.
        invisibleMousePlaceholder = fakePathVisualHolderToMouse.AddComponent<PathConnectable>();

        // Hide path placing visuals.
        tempPathVisualToMouse.gameObject.SetActive(false);
        tempPathVisualFromMouse.gameObject.SetActive(false);
    }

    void Update() {

        if (!GameManager.Instance.IsPlaying()) {
            return;
        }

        bool click = Input.GetMouseButtonDown(0);

        if (click) {
            bool clickHandled = false;
            Ray cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(cameraRay);
            foreach (RaycastHit2D hit in hits) {
                PathConnectable clickedPathNode = hit.collider.GetComponent<PathConnectable>();
                if (clickedPathNode != null) {

                    // Kind of janky, but this indicates that we are splitting a path and locking it onto a new node.
                    if (tempPathVisualFromMouse.isActiveAndEnabled) {
                        AudioManager.Instance.PlaySoftBoop();
                        GameObject pathPieceToNodeObj = Instantiate(pathPiecePrefab);
                        PathPiece pathPieceToNode = pathPieceToNodeObj.GetComponent<PathPiece>();
                        pathPieceToNode.SetEnds(tempPathVisualToMouse.StartNode(), clickedPathNode);
                        tempPathVisualToMouse.StartNode().ConnectOutgoing(pathPieceToNode);
                        clickedPathNode.ConnectIncoming(pathPieceToNode);

                        GameObject pathPieceFromNodeObj = Instantiate(pathPiecePrefab);
                        PathPiece pathPieceFromNode = pathPieceFromNodeObj.GetComponent<PathPiece>();
                        pathPieceFromNode.SetEnds(clickedPathNode, tempPathVisualFromMouse.EndNode());
                        tempPathVisualFromMouse.EndNode().ConnectIncoming(pathPieceFromNode);
                        clickedPathNode.ConnectOutgoing(pathPieceFromNode);

                        // BLOCK DISABLED
                        // Need to adjust player along the new path segments.
                        IPathFollower pathFollower = firstClickedPath.entityOnPath;
                        if (pathFollower != null && false) {
                            float speed = pathFollower.CurrentSpeed();
                            float timeOnPath = pathFollower.TimeOnPathSegment();
                            float firstSegmentTime = pathPieceToNode.TotalPathTimeAtSpeed(speed);
                            if (timeOnPath < firstSegmentTime) {
                                // Move follower to first segment
                                pathFollower.StartOnPath(pathPieceToNode, timeOnPath);
                            } else {
                                // Move follower to second segment.
                                float timeOnSecondSegment = timeOnPath - firstSegmentTime;
                                pathFollower.StartOnPath(pathPieceFromNode, timeOnSecondSegment);
                            }
                        }
                        
                        firstClickedNode = null;
                        tempPathVisualFromMouse.gameObject.SetActive(false);
                        tempPathVisualToMouse.gameObject.SetActive(false);
                        clickHandled = true;
                        break;
                    }

                    // Select the first piece if nothing is selected
                    else if (firstClickedNode == null) {
                        AudioManager.Instance.PlaySoftBoop();
                        clickedPathNode.OnClick();
                        firstClickedNode = clickedPathNode;
                        tempPathVisualToMouse.gameObject.SetActive(true);
                        tempPathVisualToMouse.SetEnds(firstClickedNode, invisibleMousePlaceholder);
                        clickHandled = true;
                        break;
                    // Connect second piece if first piece is already selected.
                    } else {
                        // Don't connect back to ourself.
                        if (clickedPathNode == firstClickedNode) {
                            continue;
                        }
                        AudioManager.Instance.PlaySoftBoop();
                        GameObject pathPieceObj = Instantiate(pathPiecePrefab);
                        PathPiece newPathPiece = pathPieceObj.GetComponent<PathPiece>();
                        newPathPiece.SetEnds(firstClickedNode, clickedPathNode);

                        // BLOCK DISABLED
                        // Need to adjust follower along the new path if there was already a follower on the outgoign path.
                        if (firstClickedNode.outgoingPath != null && firstClickedNode.outgoingPath.entityOnPath != null && false) {
                            IPathFollower pathFollower = firstClickedNode.outgoingPath.entityOnPath;
                            float timeOnPath = pathFollower.TimeOnPathSegment();
                            pathFollower.StartOnPath(newPathPiece, timeOnPath);
                        }

                        firstClickedNode.ConnectOutgoing(newPathPiece);
                        clickedPathNode.ConnectIncoming(newPathPiece);
                        firstClickedNode = null;
                        tempPathVisualToMouse.gameObject.SetActive(false);
                        clickHandled = true;
                        break;
                    }
                }
            }

            // Try checking through again to see if there are paths we clicked
            if (!clickHandled) {
                // Check the Raycast hits a second time to see if there are any paths we clicked
                foreach (RaycastHit2D hit in hits) {
                    PathPiece clickedPathPiece = hit.collider.GetComponentInParent<PathPiece>();
                    // Split a path into two segments
                    if (clickedPathPiece && firstClickedNode == null) {
                        AudioManager.Instance.PlayHarshCutBoop();
                        tempPathVisualToMouse.gameObject.SetActive(true);
                        tempPathVisualFromMouse.gameObject.SetActive(true);

                        tempPathVisualToMouse.SetEnds(clickedPathPiece.StartNode(), invisibleMousePlaceholder);
                        tempPathVisualFromMouse.SetEnds(invisibleMousePlaceholder, clickedPathPiece.EndNode());
                        clickHandled = true;
                        firstClickedPath = clickedPathPiece;
                        break;
                    }
                }
            }

            // If we went through all objects without clicking anything, reset to neutral state
            if (!clickHandled) {
                firstClickedNode = null;
                tempPathVisualFromMouse.gameObject.SetActive(false);
                tempPathVisualToMouse.gameObject.SetActive(false);
            }
        }

        // Visually adjust any mouse-based path segments
        if (firstClickedNode != null || tempPathVisualFromMouse.isActiveAndEnabled) {
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            invisibleMousePlaceholder.transform.position = mousePos;
            if (tempPathVisualToMouse.isActiveAndEnabled)
                tempPathVisualToMouse.RescalePathToEnds();
            if (tempPathVisualFromMouse.isActiveAndEnabled)
                tempPathVisualFromMouse.RescalePathToEnds();
        }
    }
}
