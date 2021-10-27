using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper
{
    public static Vector3 GetEnemyRaycastOrigin(Transform transform, CapsuleCollider enemyCollider)
    {
        Vector3 rayOrigin = transform.position;
        // Add the capsule height
        rayOrigin += new Vector3(0, enemyCollider.height - 1, 0);
        // Add the capsule radius in the forward direction
        rayOrigin += transform.forward * enemyCollider.radius;
        // return enemy ray origin
        return rayOrigin;  
    }
 
    public static GameObject FindChildGameObjectWithTag(GameObject obj, string tag)
    {
        Transform t = obj.transform;
        foreach (Transform tr in t)
        {
            if (tr.tag == tag)
            {
                return tr.gameObject;
            }
        }
        return null;
    }

    public static int[] GetUniqueRandomNumbersBetween(int low, int high, int count) {
        Dictionary<int, bool> mem = new Dictionary<int, bool>();
        int[] res = new int[count];
        int curr = 0;
        int gen;
        while (curr < count) {
            gen = Random.Range(low, high);
            if (!mem.ContainsKey(gen)) {
                res[curr] = gen;
                curr++;
                mem[gen] = true;
            }
        }
        return res;
    }
}
