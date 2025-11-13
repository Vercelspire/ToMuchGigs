using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CandySpawner : MonoBehaviour
{
    public GameObject candyPrefab;

    // player
    public Transform player;

    // respawn time
    public float respawnTime = 10f;

    // spawn count
    public int spawnCount = 10;
    public float spawnRadius = 150f; // 150 studs

    // list of game objects, so this is star
    private List<GameObject> spawnedCandies = new List<GameObject>();


    public LayerMask whatIsGround;
    public float groundOffset = 5f;
    public int maxAttempts = 30;


    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnBatch();
            yield return new WaitForSeconds(respawnTime);
            ClearBatch();
        }
    }

    //spawn
    void SpawnBatch()
    {
        int spawned = 0;

        for (int i = 0; i < spawnCount; i++)
        {
            bool foundGround = false;
            for (int attempt = 0; attempt < maxAttempts && !foundGround; attempt++)
            {
                Vector3 randomOffset = new Vector3(
                    Random.Range(-spawnRadius, spawnRadius),
                    0, 
                    Random.Range(-spawnRadius, spawnRadius)
                );

                Vector3 horizontalPos = player.position + randomOffset;

                // raycast downward to find ground
                Vector3 rayStart = new Vector3(
                    horizontalPos.x,
                    player.position.y + 300f,
                    horizontalPos.z
                );

                RaycastHit hit;

                if (Physics.Raycast(rayStart, Vector3.down, out hit, 1000f, whatIsGround))

                {

                    // finds ground to spawn candy
                    Vector3 finalPos = hit.point + Vector3.up * groundOffset;
                    GameObject candy = Instantiate(candyPrefab, finalPos, Quaternion.identity);
                    spawnedCandies.Add(candy);
                    spawned++;
                    foundGround = true;
                }
            }
        }
    }


    // clears after 10 seconds
    void ClearBatch()
    {
        int cleared = 0;
        foreach (GameObject c in spawnedCandies)
        {
            if (c != null)
            {
                Destroy(c);
                cleared++;
            }
        }
        spawnedCandies.Clear();
        Debug.Log($"Cleared {cleared} candies");
    }
}
