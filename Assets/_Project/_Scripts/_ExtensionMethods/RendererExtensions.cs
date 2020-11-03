using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static partial class Extensions
{
    public static void Enabled( this IList<Renderer> items, bool enabled)
    {
        var itemsCount = items.Count;

        for (int i = 0; i < itemsCount; i++)
            items[i].enabled = enabled;
    }

    public static void EnableAll(this IList<Renderer> items)
    {
        var itemsCount = items.Count;

        for (int i = 0; i < itemsCount; i++)
            items[i].enabled = true;
    }

    public static void DisableAll(this IList<Renderer> items)
    {
        var itemsCount = items.Count;

        for (int i = 0; i < itemsCount; i++)
            items[i].enabled = false;
    }
}