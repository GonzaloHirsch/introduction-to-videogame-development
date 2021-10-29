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
        rayOrigin += transform.forward * (enemyCollider.radius + 0.65f);
        // return enemy ray origin
        return rayOrigin;  
    }

    public static Vector3 GetEnemyRaycastDirection(
        Transform enemy, 
        Transform target,
        float axisAccuracyProbability = 0f,
        float axisAccuracyDelta = 0f
    ) {
        int[] multipliers = {-1, 1};
        // Direction to the target
        Vector3 direction = (target.position - enemy.position).normalized;
        // Inaccuracy to be added to the raycast
        Vector3 accuracyDelta = new Vector3(0f, 0f, 0f);
        // Add inaccuracy to each axis
        for (int i = 0; i < 3; i++)
        {
            if (axisAccuracyProbability >= Random.Range(0f, 1f)) 
            {
                // Debug.Log("HERE");
                // Add degrees of inaccuracy
                accuracyDelta[i] = Random.Range(0.5f, axisAccuracyDelta);
                // Define if it will be positive or negative degree
                accuracyDelta[i] *= multipliers[Random.Range(0, 2)];
            }
        }
        Debug.Log("Inaccuracy = " + accuracyDelta);
        // Return the direct direction with the added inaccuracy
        return direction + accuracyDelta;
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
