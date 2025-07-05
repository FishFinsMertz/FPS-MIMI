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
        //Debug.Log("Populating spawner");
        if (!IsServer) return; 
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
        }
    }

    private void Update()
    {
        if (!IsServer) return;
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
        if (availableEnemies.Count == 0) return;

        GameObject enemy = availableEnemies.Dequeue();
        enemy.transform.position = GetSpawnPosition();

        // Enable before spawning so clients see it immediately
        enemy.SetActive(true);

        if (enemy.TryGetComponent<NetworkObject>(out var netObj))
        {
            // Spawn the enemy on the network
            if (!netObj.IsSpawned)
                netObj.Spawn();

            // Set targets list after spawning
            if (enemy.TryGetComponent<EnemyControllerBase>(out var controller))
            {
                var allPlayers = new List<GameObject>();
                foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
                {
                    if (client.PlayerObject != null)
                        allPlayers.Add(client.PlayerObject.gameObject);
                }
                controller.targets = allPlayers;
                controller.spawner = this;
            }
        }
    }

    public void RecycleEnemy(GameObject enemy)
    {
        if (enemy.TryGetComponent<NetworkObject>(out var netObj))
        {
            if (netObj.IsSpawned)
                netObj.Despawn(false);  
        }

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
