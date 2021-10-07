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
}
