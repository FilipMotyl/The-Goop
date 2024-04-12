using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Object Pool", menuName = "SO/Object Pool")]
public class ObjectPoolSO : ScriptableObject
{
    [SerializeField] private GameObject objectPrefab;
    [SerializeField] private int initialPoolSize = 10;

    private Queue<GameObject> pool = new Queue<GameObject>();
    private Transform parent;

    public void InitializePool(Transform parent)
    {
        this.parent = parent;
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject pooledObject = Instantiate(objectPrefab, parent);
            pooledObject.SetActive(false);
            pool.Enqueue(pooledObject);
        }
    }

    public GameObject GetObject()
    {
        if (pool.Count == 0)
        {
            GameObject newPooledObject = Instantiate(objectPrefab, parent);
            return newPooledObject;
        }
        else
        {
            GameObject pooledObject = pool.Dequeue();
            pooledObject.SetActive(true);
            return pooledObject;
        }
    }

    public GameObject GetObjectAtPosition(Vector3 position)
    {
        if (pool.Count == 0)
        {
            GameObject newPooledObject = Instantiate(objectPrefab, position, Quaternion.identity, parent);
            return newPooledObject;
        }
        else
        {
            GameObject pooledObject = pool.Dequeue();
            pooledObject.transform.position = position;
            pooledObject.SetActive(true);
            return pooledObject;
        }
    }
    public void ReturnObject(GameObject pooledObject)
    {
        pooledObject.SetActive(false);
        pool.Enqueue(pooledObject);
    }
}