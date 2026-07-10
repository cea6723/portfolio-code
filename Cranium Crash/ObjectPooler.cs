using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    [System.Serializable]
    public class Pool
    {
        [SerializeField] private string tag;
        public string Tag { get { return tag; } }

        [SerializeField] private GameObject prefab;
        public GameObject Prefab { get { return prefab; } }

        [SerializeField] private int size;
        public int Size { get { return size; } }
    }

    [SerializeField] private List<Pool> pools;

    public Dictionary<string, List<GameObject>> poolDictionary;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        poolDictionary = new Dictionary<string, List<GameObject>>();

        foreach (Pool pool in pools)
        {
            List<GameObject> objectPool = new List<GameObject>();

            for (int i = 0; i < pool.Size; i++)
            {
                GameObject obj = Instantiate(pool.Prefab);
                obj.SetActive(false);
                objectPool.Add(obj);
            }

            poolDictionary.Add(pool.Tag, objectPool);
        }
    }

    public GameObject GetPooledObject(string tag)
    {
        for (int i = 0; i < poolDictionary[tag].Count; i++)
        {
            if (!poolDictionary[tag][i].activeInHierarchy)
            {
                return poolDictionary[tag][i];
            }
        }
        return null;
    }
}
