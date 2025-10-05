using UnityEngine;

[CreateAssetMenu(fileName = "FruitData", menuName = "ScriptableObjects/FruitData")]
public class FruitData : ScriptableObject
{
    [Header("Identification")]
    public string fruitKey; // same as addressable key ("BigFruit", etc.)

    [Header("Gameplay Settings")]
    public int points = 10;
    public float baseForce = 10f;
    public float randomForceVariation = 0.1f; // ± percentage
    public float torqueRange = 2f;

    [Header("Spawn Behaviour")]
    public float spawnWeight = 1f; // used for rarity probability
    public bool favorLeftSide = false; // for RareFruit

    public float GetRandomizedForce()
    {
        float multiplier = Random.Range(1f - randomForceVariation, 1f + randomForceVariation);
        return baseForce * multiplier;
    }

    public float GetRandomTorque()
    {
        return Random.Range(-torqueRange, torqueRange);
    }
}