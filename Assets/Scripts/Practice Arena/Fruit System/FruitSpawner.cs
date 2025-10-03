using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    

    [Header("Spawn Settings")]
    public float spawnInterval = 2.5f;
    public int minFruitsPerWave = 1;
    public int maxFruitsPerWave = 3;

    [Header("Spawn Zone")]
    public BoxCollider2D spawnZone; // assign in inspector

    [Header("Fruit Forces")]
    public float bigFruitForce = 8f;
    public float smallFruitForce = 12f;

    private float timer;


    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnWave();
            timer = 0;
        }
    }

    void SpawnWave()
    {
        int fruitCount = Random.Range(minFruitsPerWave, maxFruitsPerWave + 1);

        for (int i = 0; i < fruitCount; i++)
        {
            SpawnFruit();
        }
    }

    void SpawnFruit()
    {
        string fruitType = Random.value > 0.5f ? "BigFruit" : "SmallFruit";
        GameObject fruit = FruitPooler.Instance.GetFruit(fruitType);
        if (fruit == null) return;

        Bounds bounds = spawnZone.bounds;

        //  Spawn at bottom inside zone
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = bounds.min.y;

        fruit.transform.position = new Vector2(x, y);
        fruit.transform.rotation = Quaternion.identity;

        Rigidbody2D rb = fruit.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // Force based on type
        float upwardForce = (fruitType == "BigFruit") ? bigFruitForce : smallFruitForce;

        //  Calculate horizontal direction so it stays within zone
        // 
        // 
        float zoneWidth = bounds.max.x - bounds.min.x;
        float normalizedX = (x - bounds.center.x) / (zoneWidth * 0.5f);

        
        float horizontalBias = -normalizedX * 0.3f; // push inward

        Vector2 launchDir = new Vector2(horizontalBias, 1f).normalized;

        rb.AddForce(launchDir * upwardForce, ForceMode2D.Impulse);

        
        rb.AddTorque(Random.Range(-2f, 2f), ForceMode2D.Impulse);
    }
}
