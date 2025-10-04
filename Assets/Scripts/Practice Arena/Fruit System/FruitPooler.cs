using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class FruitPooler : MonoBehaviour
{
    //public static FruitPooler Instance;

    //[System.Serializable]
    //public class FruitType
    //{
    //    public string name;          
    //    public GameObject prefab;    
    //    public int poolSize = 10;    
    //}

    //[Header("Fruit Types")]
    //public List<FruitType> fruitTypes;

    //private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

    //void Awake()
    //{
    //    Instance = this;
    //}

    //void Start()
    //{
    //    // Create a pool for each fruit type
    //    foreach (FruitType type in fruitTypes)
    //    {
    //        Queue<GameObject> fruitQueue = new Queue<GameObject>();

    //        for (int i = 0; i < type.poolSize; i++)
    //        {
    //            GameObject obj = Instantiate(type.prefab);
    //            obj.SetActive(false);
    //            fruitQueue.Enqueue(obj);
    //        }

    //        poolDictionary.Add(type.name, fruitQueue);
    //    }
    //}

    //public GameObject GetFruit(string fruitName)
    //{
    //    if (!poolDictionary.ContainsKey(fruitName)) return null;

    //    GameObject fruit = poolDictionary[fruitName].Dequeue();
    //    fruit.SetActive(true);
    //    poolDictionary[fruitName].Enqueue(fruit); // recycle
    //    return fruit;
    //} 

    public static FruitPooler Instance;

    [System.Serializable]
    public class FruitType
    {
        public string key;            // Addressable key (e.g. "BigFruit", "SmallFruit")
        public int poolSize = 10;     // Preload amount
    }

    [Header("Fruit Types")]
    public List<FruitType> fruitTypes;

    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();
    private Dictionary<string, GameObject> prefabCache = new Dictionary<string, GameObject>();

    private bool isReady = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(PreloadFruits());
    }

    IEnumerator PreloadFruits()
    {
        foreach (FruitType type in fruitTypes)
        {
            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(type.key);
            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject prefab = handle.Result;
                prefabCache[type.key] = prefab;

                Queue<GameObject> fruitQueue = new Queue<GameObject>();

                for (int i = 0; i < type.poolSize; i++)
                {
                    GameObject obj = Instantiate(prefab);
                    obj.SetActive(false);
                    fruitQueue.Enqueue(obj);
                }

                poolDictionary.Add(type.key, fruitQueue);
            }
            else
            {
                Debug.LogError($" Failed to load fruit prefab: {type.key}");
            }
        }

        isReady = true;
    }

    public GameObject GetFruit(string key)
    {
        if (!isReady)
        {
            Debug.LogWarning(" Fruits not ready yet!");
            return null;
        }

        if (!poolDictionary.ContainsKey(key))
        {
            Debug.LogError($" Fruit type {key} not found!");
            return null;
        }

        Queue<GameObject> fruitQueue = poolDictionary[key];

        if (fruitQueue.Count > 0)
        {
            GameObject fruit = fruitQueue.Dequeue();
            fruit.SetActive(true);
            return fruit;
        }
        else
        {
            // Expand pool using cached prefab
            if (prefabCache.ContainsKey(key))
            {
                GameObject fruit = Instantiate(prefabCache[key]);
                fruit.SetActive(true);
                return fruit;
            }

            Debug.LogError($" No prefab cached for {key}");
            return null;
        }
    }

    public void ReturnFruit(string key, GameObject fruit)
    {
        fruit.SetActive(false);

        if (poolDictionary.ContainsKey(key))
        {
            poolDictionary[key].Enqueue(fruit);
        }
        else
        {
            Destroy(fruit); // fallback safety
        }
    }
}
