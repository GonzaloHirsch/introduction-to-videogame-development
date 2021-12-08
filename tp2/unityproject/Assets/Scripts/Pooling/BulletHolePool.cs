using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Taken from https://learn.unity.com/tutorial/introduction-to-object-pooling#5ff8d015edbc2a002063971d
public class BulletHolePool : MonoBehaviour
{
    public static BulletHolePool SharedInstance;
    public GameObject objectToPool;
    public int amountToPool;
    
    private List<GameObject> pooledObjects = new List<GameObject>();
    private int nextObjectToGive = 0;
    private GameObject tmp;

    void Awake()
    {
        SharedInstance = this;
    }

    void Start() {
        // Generate all initial instances
        for (int i = 0; i < this.amountToPool; i++) {
            this.tmp = Instantiate(this.objectToPool);
            this.tmp.SetActive(false);
            this.pooledObjects.Add(this.tmp);
        }
    }

    public GameObject GetPooledObject() {
        return this.pooledObjects[this.nextObjectToGive++ % this.amountToPool];
    }

    public void ActivatePooledObject(Vector3 position, Quaternion rotation) {
        this.tmp = this.GetPooledObject();
        this.tmp.transform.position = position;
        this.tmp.transform.rotation = rotation;
        this.tmp.SetActive(true);
    }
}