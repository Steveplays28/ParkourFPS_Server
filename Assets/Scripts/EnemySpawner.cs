using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner settings")]
    public float spawnDelay = 5;
    public int amountToSpawn = 1;
    public float minSpawnPositionOffset = 0;
    public float maxSpawnPositionOffset = 5;

    [Header("Enemy prefabs")]
    public Entity[] enemyPrefabs;

    private System.Random random = new System.Random();

    private void Start()
    {
        if (enemyPrefabs.Length == 0)
        {
            Debug.LogError("No enemy prefabs set in the inspector!");
            return;
        }

        StartCoroutine(SpawnTimer());
    }

    private IEnumerator SpawnTimer()
    {
        yield return new WaitForSeconds(spawnDelay);

        if (Enemy.enemies.Count < Enemy.maxEnemies)
        {
            for (int i = 0; i < amountToSpawn; i++)
            {
                SpawnEnemy();
            }
        }

        StartCoroutine(SpawnTimer());
    }

    private void SpawnEnemy()
    {
        int randomEnemyNumber = random.Next(0, enemyPrefabs.Length - 1);
        Entity randomEnemyPrefab = enemyPrefabs[randomEnemyNumber];

        Vector3 spawnPosition = new Vector3(Random.Range(minSpawnPositionOffset, maxSpawnPositionOffset), transform.position.y, Random.Range(minSpawnPositionOffset, maxSpawnPositionOffset));
        Vector3 SpawnRotation = new Vector3(Random.Range(minSpawnPositionOffset, maxSpawnPositionOffset), transform.position.y, Random.Range(minSpawnPositionOffset, maxSpawnPositionOffset));

        Entity spawnedEnemy = Instantiate(randomEnemyPrefab, spawnPosition, Quaternion.identity);
        spawnedEnemy.Initialize(Server.clients.Count, "AI enemy " + randomEnemyNumber);
    }
}
