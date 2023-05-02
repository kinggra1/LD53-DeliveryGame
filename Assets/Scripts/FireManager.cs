using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireManager : MonoBehaviour
{
    private static readonly float MIN_TIME_BETWEEN_FIRES = 20f;
    private static readonly float MAX_TIME_BETWEEN_FIRES = 40f;
    private static readonly float EXPERT_MODE_MODIFIER = 15f;

    private static bool showFireTooltip = true;

    public DeliverySquirrel mainPathFollower;

    private float fireTimer = 0f;
    private float nextFireTime = 15f;

    // Start is called before the first frame update
    void Start()
    {
        // SetNextFireTime();
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
        candidatePiece?.SetOnFire(showFireTooltip);

        AudioManager.Instance.PlayFireWoosh();

        showFireTooltip = false;
    }

    private void SetNextFireTime() {
        nextFireTime = Random.Range(MIN_TIME_BETWEEN_FIRES, MAX_TIME_BETWEEN_FIRES)
            - EXPERT_MODE_MODIFIER * GameManager.Instance.ExpertStatus();
        fireTimer = 0f;
    }
}
