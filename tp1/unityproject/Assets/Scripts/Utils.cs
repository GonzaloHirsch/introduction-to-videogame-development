using UnityEngine;
using Enum = System.Enum;
public static class Utils
{
    public static T getRandomEnumValue<T>() {
        T[] values = (T[])Enum.GetValues(typeof(T));
        return values[Random.Range(0,values.Length)];
    } 
    public static float getRandomNumInRange(float lower, float upper) {
        return Random.Range(lower, upper);
    }
}
