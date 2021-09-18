using UnityEngine;
using Enum = System.Enum;
public static class Utils
{
    public static T GetRandomEnumValue<T>() {
        T[] values = (T[])Enum.GetValues(typeof(T));
        return values[Random.Range(0,values.Length)];
    } 
    public static float GetRandomNumInRange(float lower, float upper) {
        return Random.Range(lower, upper);
    }
}
