using UnityEngine;

// Повесить на пустой объект-триггер внутри комнаты (Box Collider, Is Trigger = true).
// Объект игрока должен иметь Tag = "Player".
// spawnPoints - до 3 пустых объектов (точек появления) внутри комнаты.
public class RoomTrigger : MonoBehaviour
{
    public GameObject[] enemyPrefabs; // префабы Zombie (с разным detectionType)
    public Transform[] spawnPoints;   // точки спавна, максимум 3
    public int maxEnemies = 3;

    private bool hasSpawned = false;

    void OnTriggerEnter(Collider other)
    {
        if (hasSpawned) return;
        if (!other.CompareTag("Player")) return;

        hasSpawned = true;
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        if (enemyPrefabs.Length == 0 || spawnPoints.Length == 0) return;

        int count = Mathf.Min(maxEnemies, spawnPoints.Length);

        for (int i = 0; i < count; i++)
        {
            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Instantiate(prefab, spawnPoints[i].position, spawnPoints[i].rotation);
        }
    }
}