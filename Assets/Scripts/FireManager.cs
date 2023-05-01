using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireManager : MonoBehaviour
{
    private static readonly float MIN_TIME_BETWEEN_FIRES = 10f;
    private static readonly float MAX_TIME_BETWEEN_FIRES = 20f;

    public DeliverySquirrel mainPathFollower;

    private float fireTimer = 0f;
    private float nextFireTime;

    // Start is called before the first frame update
    void Start()
    {
        SetNextFireTime();
    }

    // Update is called once per frame
    void Update()
    {
        fireTimer += Time.deltaTime;
        if (fireTimer > nextFireTime) {
            TryStartFire();
            SetNextFireTime();
        }
    }

    private void TryStartFire() {
        if (mainPathFollower.GetCurrentPath() == null) {
            return;
        }

        // travel backwarrds to the path segment prior to the current segment.
        PathPiece candidatePiece = mainPathFollower.GetCurrentPath().StartNode().incomingPath;
        candidatePiece?.SetOnFire();
    }

    private void SetNextFireTime() {
        nextFireTime = Random.Range(MIN_TIME_BETWEEN_FIRES, MAX_TIME_BETWEEN_FIRES);
        fireTimer = 0f;
    }
}
