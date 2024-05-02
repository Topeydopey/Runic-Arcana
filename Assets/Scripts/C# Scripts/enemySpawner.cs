using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // The enemy prefab
    public float spawnRate = 2f;   // How often to spawn enemies
    private float nextSpawnTime;

    public Vector2 spawnAreaMin; // Minimum spawn position
    public Vector2 spawnAreaMax; // Maximum spawn position

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + 1f / spawnRate;
        }
    }

    void SpawnEnemy()
    {
        // Generate a random position within the defined area
        float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float randomY = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
        Vector3 spawnPosition = new Vector3(randomX, randomY, 0); // Assuming a 2D game

        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}
