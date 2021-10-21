using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper
{
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
            gen = Random.Range(low, high + 1);
            if (!mem.ContainsKey(gen)) {
                res[curr] = gen;
                curr++;
                mem[gen] = true;
            }
        }
        return res;
    }
}
