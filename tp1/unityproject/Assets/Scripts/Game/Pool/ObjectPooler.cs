using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPoolItem {
    public GameObject objectToPool;
    public int amountToPool;
}

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler SharedInstance;
    public List<GameObject> pooledObjects;
    public List<ObjectPoolItem> itemsToPool;
    
    void Awake()
    {
        SharedInstance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        pooledObjects = new List<GameObject>();
        foreach (ObjectPoolItem item in itemsToPool) {
            for (int i = 0; i < item.amountToPool; i++) {
                GameObject go = Instantiate(item.objectToPool);
                go.SetActive(false);
                pooledObjects.Add(go);
            } 
        }
    }

    public GameObject GetPooledObject(string tag)
    {
        for (int i = 0; i < pooledObjects.Count; i++) {
            if (!pooledObjects[i].activeInHierarchy &&
                pooledObjects[i].tag == tag) {
                return pooledObjects[i];
            }
        }
        return null;
    }

    public void ActivatePooledObject(string tag, Vector3 position, Quaternion rotation) {
        GameObject go = ObjectPooler.SharedInstance.GetPooledObject(tag);
        if (go != null) {
            go.transform.position = position;
            go.transform.rotation = rotation;
            go.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
