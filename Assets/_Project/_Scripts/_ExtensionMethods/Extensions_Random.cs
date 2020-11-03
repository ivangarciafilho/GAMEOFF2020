using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;


public static partial class Extensions_Random
{
    public static float RandomInRange(this Vector2 floatRange)
    {
        return Random.Range(floatRange.x, floatRange.y);
    }

    public static int RandomInRange(this Vector2Int intRange)
    {
        return Random.Range(intRange.x, intRange.y);
    }

    public static T RandomItem<T>(this IList<T> list)
    {
        if (list.Count == 0) throw new System.IndexOutOfRangeException("Cannot select a random item from an empty list");
        var shuffledList = list.Shuffle().ToArray();
        return shuffledList[Random.Range(0, shuffledList.Length)];
    }

    public static T RemoveRandom<T>(this IList<T> list)
    {
        if (list.Count == 0) throw new System.IndexOutOfRangeException("Cannot remove a random item from an empty list");
        var randomIndex = Random.Range(0, list.Count);
        T item = list[randomIndex];
        list.RemoveAt(randomIndex);
        return item;
    }

    public static Vector3 RandomXYZ(this Vector3 vector, Vector3 min , Vector3 max)
    {
        vector.x = Random.Range(min.x, max.x);
        vector.y = Random.Range(min.y, max.y);
        vector.z = Random.Range(min.z, max.z);
        return vector;
    }

    public static Vector3 RandomXYZ(this Vector3 vector, Vector2 range)
    {
        vector.x = Random.Range(range.x, range.y);
        vector.y = Random.Range(range.x, range.y);
        vector.z = Random.Range(range.x, range.y);
        return vector;
    }

    public static Vector3 RandomXYZ(this Vector3 vector, float min, float max)
    {
        vector.x = Random.Range(min, max);
        vector.y = Random.Range(min, max);
        vector.z = Random.Range(min, max);
        return vector;
    }

    public static Vector3 RandomX(this Vector3 vector, float min, float max)
    {
        vector.x = Random.Range(min, max);
        return vector;
    }

    public static Vector3 RandomY(this Vector3 vector, float min, float max)
    {
        vector.y = Random.Range(min, max);
        return vector;
    }

    public static Vector3 RandomZ(this Vector3 vector, float min, float max)
    {
        vector.z = Random.Range(min, max);
        return vector;
    }

    public static Vector3 RandomX(this Vector3 vector, Vector2 range)
    {
        vector.x = Random.Range(range.x, range.y);
        return vector;
    }

    public static Vector3 RandomY(this Vector3 vector, Vector2 range)
    {
        vector.y = Random.Range(range.x, range.y);
        return vector;
    }

    public static Vector3 RandomZ(this Vector3 vector, Vector2 range)
    {
        vector.z = Random.Range(range.x, range.y);
        return vector;
    }

    public static Vector3 RandomUniformScale(this Vector3 vector, Vector2 range)
    {
        var randomScale = Random.Range(range.x,range.y);
        vector.x = randomScale;
        vector.y = randomScale;
        vector.z = randomScale;

        return vector;
    }

    public static Vector3 RandomUniformScale(this Vector3 vector, float min, float max)
    {
        var randomScale = Random.Range(min,max);
        vector.x = randomScale;
        vector.y = randomScale;
        vector.z = randomScale;

        return vector;
    }

    public static Vector3 ScaleRandomly(this Vector3 vector, float min, float max)
    {
        var randomScale = Random.Range(min,max);
        vector.x *= randomScale;
        vector.y *= randomScale;
        vector.z *= randomScale;

        return vector;
    }

    public static Vector3 ScaleRandomly(this Vector3 vector, Vector2 range)
    {
        var randomScale = Random.Range(range.x, range.y);
        vector.x *= randomScale;
        vector.y *= randomScale;
        vector.z *= randomScale;
        return vector;
    }
}