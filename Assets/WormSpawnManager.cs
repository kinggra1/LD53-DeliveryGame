using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormSpawnManager : Singleton<WormSpawnManager>
{
    private static readonly float MINIMUM_SPACING = 4f;
    private static readonly float MAX_SPAWN_DELAY = 15f;
    private static readonly float MIN_SPAWN_DELAY = 5f;
    private static readonly int SPAWN_LOCATION_CANDIDATES = 10;

    private float maxSpawnXDist = 10f;
    private float maxSpawnYDist = 5f;

    public GameObject wormPrefab;

    private float spawnNextAfter;
    private float spawnTimer = 0f;
    
    private List<DeliveryTarget> worms = new List<DeliveryTarget>();
    private Vector2[] spawnLocationCandidates = new Vector2[SPAWN_LOCATION_CANDIDATES];

    // Start is called before the first frame update
    void Start()
    {
        SetNextSpawnDelay();
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer > spawnNextAfter) {
            SpawnWorm();
            SetNextSpawnDelay();
        }
    }

    private void SpawnWorm() {
        for (int i = 0; i < SPAWN_LOCATION_CANDIDATES; i++) {
            Vector2 locationCandidate = new Vector2(
                Random.Range(-maxSpawnXDist, maxSpawnXDist),
                Random.Range(-maxSpawnYDist, maxSpawnYDist));
            spawnLocationCandidates[i] = locationCandidate;
        }

        Vector2 bestCandidate = Vector2.positiveInfinity;
        float bestCandidateSpacingDist = 0f;
        bool anyCandidateFound = false;
        foreach (Vector2 candidate in spawnLocationCandidates) {
            float closestWormDist = Mathf.Infinity;
            foreach (DeliveryTarget worm in worms) {
                float distToWorm = Vector2.Distance(candidate, worm.gameObject.transform.position);
                if (distToWorm < closestWormDist) {
                    Debug.Log("MinDistToWorm: " + distToWorm);
                    closestWormDist = distToWorm;
                }
            }

            // Skip assigning a best candidate if we already have a better one (or we are not within constraints).
            if (closestWormDist > bestCandidateSpacingDist && closestWormDist > MINIMUM_SPACING) {
                Debug.Log("New best (valid) candidate dist: " + closestWormDist);
                bestCandidateSpacingDist = closestWormDist;
                bestCandidate = candidate;
                anyCandidateFound = true;
            }
        }

        // We have found our best placement.
        if (anyCandidateFound) {
            Debug.Log("Spawning at: " + bestCandidate);
            Instantiate(wormPrefab);
            wormPrefab.transform.position = bestCandidate;
            DeliveryTarget worm = wormPrefab.GetComponent<DeliveryTarget>();
            worms.Add(worm);
        } else {
            Debug.Log("No candidate found");
        }
    }

    private void SetNextSpawnDelay() {
        spawnNextAfter = Random.Range(MIN_SPAWN_DELAY, MAX_SPAWN_DELAY);
        spawnTimer = 0f;
    }
}
