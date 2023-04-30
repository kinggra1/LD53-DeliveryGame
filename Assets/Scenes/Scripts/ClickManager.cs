using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : Singleton<ClickManager> {
    public Camera mainCamera;

    public GameObject pathPiecePrefab;

    private PathConnectable selectedPiece;

    // Fake path pieces used to draw a placeholder path piece while making connections.
    private PathPiece tempPathVisual;
    private PathConnectable invisibleMousePlaceholder;

    private void Awake() {
        GameObject fakePathVisualHolder = Instantiate(pathPiecePrefab, this.transform);
        fakePathVisualHolder.name = "FakePathVisualObject";
        tempPathVisual = fakePathVisualHolder.GetComponent<PathPiece>();
        invisibleMousePlaceholder = fakePathVisualHolder.AddComponent<PathConnectable>();

        // Hide path placing visual.
        tempPathVisual.gameObject.SetActive(false);
    }

    void Update() {

        bool click = Input.GetMouseButtonDown(0);

        if (selectedPiece != null) {
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            invisibleMousePlaceholder.transform.position = mousePos;
            tempPathVisual.SetEnds(selectedPiece, invisibleMousePlaceholder);
        }

        if (click) {
            Ray cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(cameraRay);
            foreach (RaycastHit2D hit in hits) {
                PathConnectable pathConnectable = hit.collider.GetComponent<PathConnectable>();
                if (pathConnectable != null) {

                    // Select the first piece
                    if (selectedPiece == null) {
                        pathConnectable.OnClick();
                        selectedPiece = pathConnectable;
                        tempPathVisual.gameObject.SetActive(true);
                        tempPathVisual.SetEnds(selectedPiece, invisibleMousePlaceholder);
                        break;
                    // Connect second piece if first piece is already selected.
                    } else {
                        GameObject pathPieceObj = Instantiate(pathPiecePrefab);
                        PathPiece pathPiece = pathPieceObj.GetComponent<PathPiece>();
                        pathPiece.SetEnds(selectedPiece, pathConnectable);

                        selectedPiece.ConnectOutgoing(pathPiece);
                        pathConnectable.ConnectIncoming(pathPiece);
                        selectedPiece = null;
                        tempPathVisual.gameObject.SetActive(false);
                        break;
                    }
                }
            }
        }
    }
}
