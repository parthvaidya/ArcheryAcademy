using UnityEngine;
using System.Collections.Generic;

public class FruitSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public BoxCollider2D spawnZone;
    public float spawnInterval = 2.5f;
    public float minSpawnInterval = 0.8f;
    public int fruitsBeforeSpeedUp = 10;
    public float spawnSpeedIncrease = 0.9f;

    [Header("Fruit Data (SO)")]
    public List<FruitData> allFruitData;

    [Header("Spawn Limits")]
    [Tooltip("Maximum fruits allowed active in the scene at once")]
    public int maxActiveFruits = 4;

    private float timer;
    private int fruitSpawnedCount = 0;
    private float currentRareChance = 10f;
    public float rareFruitIncreaseRate = 5f;
    public float maxRareChance = 30f;

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
        int activeCount = FruitPooler.Instance.GetActiveFruitCount();
        if (activeCount >= maxActiveFruits)
            return;

        FruitData chosenData = ChooseFruitData();
        if (chosenData == null) return;

        GameObject fruitObj = FruitPooler.Instance.GetFruit(chosenData.fruitKey);
        if (fruitObj == null) return;

        Fruit fruit = fruitObj.GetComponent<Fruit>();
        fruit.data = chosenData;

        Bounds bounds = spawnZone.bounds;

        float x;
        if (chosenData.favorLeftSide)
        {
            float leftBound = Mathf.Lerp(bounds.min.x, bounds.center.x, 0.3f);
            x = Random.Range(bounds.min.x, leftBound);
        }
        else
        {
            x = Random.Range(bounds.min.x, bounds.max.x);
        }

        float y = bounds.min.y;
        fruitObj.transform.position = new Vector2(x, y);
        fruitObj.transform.rotation = Quaternion.identity;

        // Calculate direction bias
        float zoneWidth = bounds.max.x - bounds.min.x;
        float normalizedX = (x - bounds.center.x) / (zoneWidth * 0.5f);
        float horizontalBias = -normalizedX * Random.Range(0.25f, 0.4f);
        horizontalBias += Random.Range(-0.1f, 0.1f);
        Vector2 launchDir = new Vector2(horizontalBias, 1f).normalized;

        fruit.Launch(launchDir);

        fruitSpawnedCount++;
        HandleDifficultyScaling();


    }

    //FruitData ChooseFruitData()
    //{
    //    // Weighted probability
    //    float totalWeight = 0;
    //    foreach (var data in allFruitData) totalWeight += data.spawnWeight;

    //    float roll = Random.Range(0, totalWeight);
    //    float cumulative = 0;

    //    foreach (var data in allFruitData)
    //    {
    //        cumulative += data.spawnWeight;
    //        if (roll <= cumulative)
    //            return data;
    //    }

    //    return allFruitData[0];
    //}

    FruitData ChooseFruitData()
    {
        // Define a 5-step pattern cycle
        string[] pattern = { "BigFruit", "SmallFruit", "BigFruit", "SmallFruit", "RareFruit" };

        // Get pattern slot
        int index = fruitSpawnedCount % pattern.Length;
        string expected = pattern[index];

        // Get FruitData matching expected type
        FruitData chosen = allFruitData.Find(f => f.fruitKey == expected);

        // Add slight randomness — 15% chance to break pattern
        float chance = Random.Range(0f, 100f);
        if (chance < 15f)
        {
            // pick any random fruit for surprise
            chosen = allFruitData[Random.Range(0, allFruitData.Count)];
        }

        return chosen;
    }

    void HandleDifficultyScaling()
    {
        if (fruitSpawnedCount % fruitsBeforeSpeedUp == 0)
        {
            spawnInterval = Mathf.Max(minSpawnInterval, spawnInterval * spawnSpeedIncrease);
            currentRareChance = Mathf.Min(currentRareChance + rareFruitIncreaseRate, maxRareChance);
            Debug.Log($"Speed | Interval: {spawnInterval:F2}s | RareChance: {currentRareChance}%");
        }
    }
}