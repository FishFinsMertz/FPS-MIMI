using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemySpawner : NetworkBehaviour
{
    [Header("Spawned Object prefab")]
    [SerializeField] GameObject spawnedObjectPrefab;

    [Header("Spawning Details")]
    [SerializeField] private float radius = 1f;
    [SerializeField] private int poolSize = 10;
    private List<GameObject> enemyPool = new List<GameObject>();
    private Queue<GameObject> availableEnemies = new Queue<GameObject>();


    // Event Bindings
    private EventBinding<LocalPlayerSpawned> playerSpawnEventBinding;

    private void Start()
    {
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log("Populating spawner");
        PopulateSpawner();
    }

    private void PopulateSpawner()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject enemy = Instantiate(spawnedObjectPrefab);
            enemy.SetActive(false);
            enemyPool.Add(enemy);
            availableEnemies.Enqueue(enemy);

            if (enemy.TryGetComponent<NetworkObject>(out var netObj))
            {
                netObj.Spawn(true); // Spawn and keep it disabled
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Spawning enemy");
            SpawnFromPool();
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            // Recycle the first enemy in the pool (if active) // TEMPORARY TO TEST
            foreach (GameObject enemy in enemyPool)
            {
                if (enemy.activeInHierarchy)
                {
                    RecycleEnemy(enemy);
                    break;
                }
            }
        }
    }

    private void SpawnFromPool()
    {
        if (availableEnemies.Count > 0)
        {
            GameObject enemy = availableEnemies.Dequeue();
            enemy.transform.position = GetSpawnPosition(); // Implement this afterwards by making a function that calculates the spawn location
            enemy.SetActive(true);
        }
    }

    private void RecycleEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
        availableEnemies.Enqueue(enemy);
    }

    private Vector3 GetSpawnPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle * radius;
        Vector3 offset = new Vector3(randomCircle.x, 0f, randomCircle.y);
        Vector3 spawnPos = transform.position + offset;
        return spawnPos;
    }
}
