using System.Collections.Generic;
using UnityEngine;

public class FruitPooler : MonoBehaviour
{
    public static FruitPooler Instance;

    [System.Serializable]
    public class FruitType
    {
        public string name;          // e.g. "BigFruit", "SmallFruit"
        public GameObject prefab;    // prefab reference
        public int poolSize = 10;    // how many to pre-instantiate
    }

    [Header("Fruit Types")]
    public List<FruitType> fruitTypes;

    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Create a pool for each fruit type
        foreach (FruitType type in fruitTypes)
        {
            Queue<GameObject> fruitQueue = new Queue<GameObject>();

            for (int i = 0; i < type.poolSize; i++)
            {
                GameObject obj = Instantiate(type.prefab);
                obj.SetActive(false);
                fruitQueue.Enqueue(obj);
            }

            poolDictionary.Add(type.name, fruitQueue);
        }
    }

    public GameObject GetFruit(string fruitName)
    {
        if (!poolDictionary.ContainsKey(fruitName)) return null;

        GameObject fruit = poolDictionary[fruitName].Dequeue();
        fruit.SetActive(true);
        poolDictionary[fruitName].Enqueue(fruit); // recycle
        return fruit;
    }
}
