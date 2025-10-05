using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("Initial time (seconds) between each fruit spawn")]
    public float spawnInterval = 2.5f;

    [Tooltip("Area inside which fruits spawn (bottom to top arc)")]
    public BoxCollider2D spawnZone;

    [Tooltip("After how many fruits the speed increases")]
    public int fruitsBeforeSpeedUp = 10;

    [Tooltip("Each speed-up reduces interval by this multiplier (e.g. 0.9 = 10% faster)")]
    public float spawnSpeedIncrease = 0.9f;

    [Tooltip("The minimum time between spawns (speed cap)")]
    public float minSpawnInterval = 0.8f;

    [Header("Fruit Forces")]
    public float bigFruitForce = 8f;
    public float smallFruitForce = 12f;
    public float rareFruitForce = 10f;

    [Header("Difficulty Settings")]
    [Tooltip("How much rare fruit chance increases every 10 fruits (in %)")]
    public float rareFruitIncreaseRate = 5f;

    [Tooltip("Maximum chance (%) of spawning rare fruit at high difficulty")]
    public float maxRareChance = 30f;

    private float timer;
    private int fruitSpawnedCount = 0;
    private int fruitPatternIndex = 0;
    private float currentRareChance = 10f;

    private string[] fruitPattern = new string[]
    {
        "BigFruit", "SmallFruit", "BigFruit", "SmallFruit", "RareFruit"
    };

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnFruit();
            timer = 0f;
        }
    }

    void SpawnFruit()
    {
        // Decide fruit type
        string fruitType = GetNextFruitType();

        GameObject fruit = FruitPooler.Instance.GetFruit(fruitType);
        if (fruit == null) return;

        Bounds bounds = spawnZone.bounds;
        float x, y;

        //  RareFruit spawns closer to left (archer’s side)
        if (fruitType == "RareFruit")
        {
            // 70% chance to appear in left quarter of spawn zone
            float leftBound = Mathf.Lerp(bounds.min.x, bounds.center.x, 0.3f);
            x = Random.Range(bounds.min.x, leftBound);
        }
        else
        {
            // normal random spawn across full width
            x = Random.Range(bounds.min.x, bounds.max.x);
        }

        y = bounds.min.y;

        fruit.transform.position = new Vector2(x, y);
        fruit.transform.rotation = Quaternion.identity;

        Rigidbody2D rb = fruit.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        float upwardForce = GetForceForFruit(fruitType);

        // inward arc
        float zoneWidth = bounds.max.x - bounds.min.x;
        float normalizedX = (x - bounds.center.x) / (zoneWidth * 0.5f);
        float horizontalBias = -normalizedX * Random.Range(0.25f, 0.4f); // slight randomness for more variety

        Vector2 launchDir = new Vector2(horizontalBias, 1f).normalized;

        rb.AddForce(launchDir * upwardForce, ForceMode2D.Impulse);
        rb.AddTorque(Random.Range(-2f, 2f), ForceMode2D.Impulse);

        fruitSpawnedCount++;
        HandleDifficultyScaling();
    }

    string GetNextFruitType()
    {
        // every 5th fruit is rare
        if ((fruitSpawnedCount + 1) % 5 == 0)
            return "RareFruit";

        // weighted chance
        float chance = Random.Range(0f, 100f);
        if (chance < currentRareChance)
            return "RareFruit";

        fruitPatternIndex = (fruitPatternIndex + 1) % 4;
        return fruitPattern[fruitPatternIndex];
    }

    float GetForceForFruit(string fruitType)
    {
        float baseForce = 10f;
        float randomMultiplier = 1f;

        switch (fruitType)
        {
            case "BigFruit":
                baseForce = bigFruitForce;
                randomMultiplier = Random.Range(0.9f, 1.15f); // small variation
                break;

            case "SmallFruit":
                baseForce = smallFruitForce;
                randomMultiplier = Random.Range(0.8f, 1.3f); // more variation
                break;

            case "RareFruit":
                baseForce = rareFruitForce;
                randomMultiplier = Random.Range(0.9f, 1.2f); // moderate variation
                break;
        }

        return baseForce * randomMultiplier;
    }

    void HandleDifficultyScaling()
    {
        if (fruitSpawnedCount % fruitsBeforeSpeedUp == 0)
        {
            spawnInterval *= spawnSpeedIncrease;
            spawnInterval = Mathf.Clamp(spawnInterval, minSpawnInterval, 5f);

            currentRareChance = Mathf.Min(currentRareChance + rareFruitIncreaseRate, maxRareChance);

            Debug.Log($"Speed | New interval: {spawnInterval:F2}s | Rare chance: {currentRareChance}%");
        }
    }
}