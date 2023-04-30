using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : Singleton<ClickManager> {
    public Camera mainCamera;

    public GameObject pathPiecePrefab;

    private PathConnectable selectedNode;

    // Fake path pieces used to draw a placeholder path piece while making connections.
    private PathPiece tempPathVisualToMouse;
    private PathPiece tempPathVisualFromMouse;
    private PathConnectable invisibleMousePlaceholder;

    void Awake() {
        GameObject fakePathVisualHolderToMouse = Instantiate(pathPiecePrefab, this.transform);
        fakePathVisualHolderToMouse.name = "FakePathVisualToMouse";
        tempPathVisualToMouse = fakePathVisualHolderToMouse.GetComponent<PathPiece>();
        // Disable colliders so we cannot click on these fake placeholder paths.
        fakePathVisualHolderToMouse.GetComponentInChildren<BoxCollider2D>(/* includeInactive= */ true).enabled = false;

        GameObject fakePathVisualHolderFromMouse = Instantiate(pathPiecePrefab, this.transform);
        fakePathVisualHolderFromMouse.name = "FakePathVisualFromMouse";
        tempPathVisualFromMouse = fakePathVisualHolderFromMouse.GetComponent<PathPiece>();
        fakePathVisualHolderFromMouse.GetComponentInChildren<BoxCollider2D>(/* includeInactive= */ true).enabled = false;

        // Doesn't matter what object this is on.
        invisibleMousePlaceholder = fakePathVisualHolderToMouse.AddComponent<PathConnectable>();

        // Hide path placing visuals.
        tempPathVisualToMouse.gameObject.SetActive(false);
        tempPathVisualFromMouse.gameObject.SetActive(false);
    }

    void Update() {

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
                        
                        selectedNode = null;
                        tempPathVisualFromMouse.gameObject.SetActive(false);
                        tempPathVisualToMouse.gameObject.SetActive(false);
                        clickHandled = true;
                        break;
                    }

                    // Select the first piece if nothing is selected
                    else if (selectedNode == null) {
                        clickedPathNode.OnClick();
                        selectedNode = clickedPathNode;
                        tempPathVisualToMouse.gameObject.SetActive(true);
                        tempPathVisualToMouse.SetEnds(selectedNode, invisibleMousePlaceholder);
                        clickHandled = true;
                        break;
                    // Connect second piece if first piece is already selected.
                    } else {
                        GameObject pathPieceObj = Instantiate(pathPiecePrefab);
                        PathPiece newPathPiece = pathPieceObj.GetComponent<PathPiece>();
                        newPathPiece.SetEnds(selectedNode, clickedPathNode);

                        selectedNode.ConnectOutgoing(newPathPiece);
                        clickedPathNode.ConnectIncoming(newPathPiece);
                        selectedNode = null;
                        tempPathVisualToMouse.gameObject.SetActive(false);
                        clickHandled = true;
                        break;
                    }
                }

                PathPiece clickedPathPiece = hit.collider.GetComponentInParent<PathPiece>();
                if (clickedPathPiece && selectedNode == null) {
                    tempPathVisualToMouse.gameObject.SetActive(true);
                    tempPathVisualFromMouse.gameObject.SetActive(true);

                    tempPathVisualToMouse.SetEnds(clickedPathPiece.StartNode(), invisibleMousePlaceholder);
                    tempPathVisualFromMouse.SetEnds(invisibleMousePlaceholder, clickedPathPiece.EndNode());
                    clickHandled = true;
                    return;
                }
            }

            // If we went through all objects without clicking anything, reset to neutral state
            if (!clickHandled) {
                selectedNode = null;
                tempPathVisualFromMouse.gameObject.SetActive(false);
                tempPathVisualToMouse.gameObject.SetActive(false);
            }
        }

        // Visually adjust any mouse-based path segments
        if (selectedNode != null || tempPathVisualFromMouse.isActiveAndEnabled) {
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            invisibleMousePlaceholder.transform.position = mousePos;
            if (tempPathVisualToMouse.isActiveAndEnabled)
                tempPathVisualToMouse.RescalePathToEnds();
            if (tempPathVisualFromMouse.isActiveAndEnabled)
                tempPathVisualFromMouse.RescalePathToEnds();
        }
    }
}
