using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public static partial class Extensions
{
    public static float RandomRange(this Vector2 range)
    {
        return Random.Range(range.x, range.y);
    }

    public static int RandomRange(this Vector2Int range)
    {
        return Random.Range(range.x, range.y);
    }

    public static Vector2 RandomMagnitude(this Vector2 range)
    {
        return new Vector2(Random.Range(-range.x, range.x), Random.Range(-range.y, range.y));
    }

    public static Vector2Int RandomMagnitude(this Vector2Int range)
    {
        return new Vector2Int(Random.Range(-range.x, range.x), Random.Range(-range.y, range.y));
    }

    public static Vector3 RandomMagnitude(this Vector3 range)
    {
        return new Vector3(Random.Range(-range.x, range.x), Random.Range(-range.y, range.y), Random.Range(-range.z, range.z));
    }

    public static Vector3Int RandomMagnitude(this Vector3Int range)
    {
        return new Vector3Int(Random.Range(-range.x, range.x), Random.Range(-range.y, range.y), Random.Range(-range.z, range.z));
    }

    public static float Random01(this float floatValue)
    {
        return floatValue * Random.Range(0f, 1f);
    }

    public static int Random01(this int floatValue)
    {
        return Mathf.RoundToInt(floatValue * Random.Range(0f, 1f));
    }

    private static System.Random randomValue = new System.Random();
    
    public static T RandomItem<T>(this T[] items)
    {
        return items[randomValue.Next(0, items.Length)];
    }

    public static T RandomItem<T>(this List<T> items)
    {
        return items[randomValue.Next(0, items.Count)];
    }
}

