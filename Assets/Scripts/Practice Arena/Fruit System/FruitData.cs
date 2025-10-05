using UnityEngine;

[CreateAssetMenu(menuName = "Game/FruitData")]
public class FruitData : ScriptableObject
{
    public string fruitName;
    public int points = 10;
    public float baseForce = 10f;
    public float mass = 1f;
    public Color color;
}