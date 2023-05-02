using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormSpawnManager : Singleton<WormSpawnManager>
{
    private static readonly float MINIMUM_SPACING = 4f;
    private static readonly float MAX_SPAWN_DELAY = 60f;
    private static readonly float MIN_SPAWN_DELAY = 25f;
    private static readonly int SPAWN_LOCATION_CANDIDATES = 10;

    private static readonly float TIME_UNTIL_MULTIPLE_FOODS = 120f;
    private bool secondFoodSpawned = false;

    private float maxSpawnXDist = 11f;
    private float maxSpawnYDist = 5f;

    public GameObject wormPrefab;
    public GameObject blueFoodPilePrefab;
    public GameObject redFoodPilePrefab;
    public GameObject yellowFoodPilePrefab;

    private float spawnNextAfter;
    private float spawnTimer = 0f;

    private float foodPileTimer = 0f;


    private static readonly float MIN_TIME_TO_HUNGER = 10f;
    private static readonly float MAX_TIME_TO_HUNGER = 20f;
    private static readonly float EXPERT_MODIFIER = 10f;

    private float hungryWormTimer;
    private float timeUntilNextHunger;
    
    private List<PathConnectable> allConnectableEntities = new List<PathConnectable>();
    private List<DeliveryTarget> allWorms = new List<DeliveryTarget>();
    private Vector2[] spawnLocationCandidates = new Vector2[SPAWN_LOCATION_CANDIDATES];

    // Start is called before the first frame update
    void Start()
    {
        GameObject blueFoodPile = Instantiate(blueFoodPilePrefab);
        blueFoodPile.transform.position = Vector3.left * 5f;
        allConnectableEntities.Add(blueFoodPile.GetComponent<PathConnectable>());

        CreateWormAt(new Vector2(6f, 0f), false);
        CreateWormAt(new Vector2(2f, 4f), false);
        CreateWormAt(new Vector2(2f, -4f), false);
        SetNextSpawnDelay();
    }

    // Update is called once per frame
    void Update()
    {
        foodPileTimer += Time.deltaTime;
        if (!secondFoodSpawned && foodPileTimer > TIME_UNTIL_MULTIPLE_FOODS) {
            SpawnRedFoodPile();
            secondFoodSpawned = true;
        }

        spawnTimer += Time.deltaTime;
        if (spawnTimer > spawnNextAfter) {
            SpawnWorm();
            SetNextSpawnDelay();
        }

        hungryWormTimer += Time.deltaTime;
        if (hungryWormTimer > timeUntilNextHunger) {
            hungryWormTimer = 0f;

            float randomSum = (Random.Range(-1f, 1f) + Random.Range(-1f, 1f)) / 2f;
            float mirroredSum = Mathf.Abs(randomSum);
            timeUntilNextHunger = MIN_TIME_TO_HUNGER + (1f - mirroredSum) * (MAX_TIME_TO_HUNGER - MIN_TIME_TO_HUNGER);
            timeUntilNextHunger -= EXPERT_MODIFIER * GameManager.Instance.ExpertStatus();
            // timeUntilNextHunger = Random.Range(MIN_TIME_TO_HUNGER, MAX_TIME_TO_HUNGER);

            Shuffle(allWorms);
            foreach (DeliveryTarget worm in allWorms) {
                if (worm.CanBecomeHungry()) {
                    worm.BecomeHungry();
                    break;
                }
            }
        }
    }

    public static void Shuffle<T>(IList<T> list) {
        for (int i = list.Count - 1; i > 1; i--) {
            int rnd = Random.Range(0, i + 1);

            T value = list[rnd];
            list[rnd] = list[i];
            list[i] = value;
        }
    }

    public bool SecondFoodReady() {
        return secondFoodSpawned;
    }

    private void SpawnWorm() {
        Vector2 candidateLocation = TryFindSpawnLocation();
        if (candidateLocation != Vector2.zero) {
            CreateWormAt(candidateLocation);
        }
    }

    private void SpawnRedFoodPile() {
        Vector2 candidateLocation = TryFindSpawnLocation();
        if (candidateLocation != Vector2.zero) {
            GameObject redFoodPile = Instantiate(redFoodPilePrefab);
            redFoodPile.transform.position = candidateLocation;
            allConnectableEntities.Add(redFoodPile.GetComponent<PathConnectable>());
        }
    }

    private Vector2 TryFindSpawnLocation() {
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
            foreach (PathConnectable pathNode in allConnectableEntities) {
                float distToWorm = Vector2.Distance(candidate, pathNode.gameObject.transform.position);
                if (distToWorm < closestWormDist) {
                    closestWormDist = distToWorm;
                }
            }

            // Skip assigning a best candidate if we already have a better one (or we are not within constraints).
            if (closestWormDist > bestCandidateSpacingDist && closestWormDist > MINIMUM_SPACING) {
                bestCandidateSpacingDist = closestWormDist;
                bestCandidate = candidate;
                anyCandidateFound = true;
            }
        }

        // We have found our best placement.
        if (anyCandidateFound) {
            return bestCandidate;
        } else {
            Debug.Log("No candidate found");
            return Vector2.zero;
        }
    }

    private void CreateWormAt(Vector2 pos) {
        CreateWormAt(pos, true);
    }

    private void CreateWormAt(Vector2 pos, bool makeSound) {
        GameObject wormObj = Instantiate(wormPrefab);
        wormObj.transform.position = pos;
        DeliveryTarget worm = wormObj.GetComponent<DeliveryTarget>();
        allConnectableEntities.Add(worm);
        allWorms.Add(worm);

        if (makeSound)
            AudioManager.Instance.PlayWormAppearSound();
    }

    private void SetNextSpawnDelay() {
        spawnNextAfter = Random.Range(MIN_SPAWN_DELAY, MAX_SPAWN_DELAY);
        spawnTimer = 0f;
    }
}
