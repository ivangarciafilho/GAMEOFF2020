using UnityEngine;
using UnityEngine.SceneManagement;

public static partial class Extensions
{

    /// <summary>
    /// Extension method to check if a layer is in a layermask
    /// </summary>
    /// <param name="mask"></param>
    /// <param name="layer"></param>
    /// <returns></returns>
    public static bool Contains(this LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }

    #region LAYER BY INT ////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Convert int value to equivalent Layer string Name
    /// </summary>
    /// <param name="intValue"></param>
    /// <returns></returns>
    public static string ToLayer(this int intValue)
    {
        return LayerMask.LayerToName(intValue);
    }

    /// <summary>
    /// Switch layer of the transform's gameobject
    /// </summary>
    /// <param name="ownTransform"></param>
    /// <param name="layer"></param>
    /// <param name="sendChildObjectsToSameLayer"></param>
    public static void SendToLayer(this Transform ownTransform, int layer, bool sendChildObjectsToSameLayer = false)
    {
        ownTransform.gameObject.SendToLayer(layer, sendChildObjectsToSameLayer);
    }

    /// <summary>
    /// Switch layer of the gameobject
    /// </summary>
    /// <param name="ownGameObject"></param>
    /// <param name="layer"></param>
    /// <param name="sendChildObjectsToSameLayer"></param>
    public static void SendToLayer(this GameObject ownGameObject, int layer, bool sendChildObjectsToSameLayer = false)
    {
        ownGameObject.layer = layer;

        if (sendChildObjectsToSameLayer)
        {
            var childGameObjects = ownGameObject.GetComponentsInChildren<GameObject>();
            var childCount = childGameObjects.Length;

            for (int i = 0; i < childCount; i++)
                childGameObjects[i].layer = layer;
        }
    }

    /// <summary>
    /// Switch layer of the transforms gameObjects
    /// </summary>
    /// <param name="ownGameObject"></param>
    /// <param name="layer"></param>
    /// <param name="sendChildObjectsToSameLayer"></param>
    public static void SendToLayer(this Transform[] transforms, int layer, bool sendChildObjectsToSameLayer = false)
    {
        var amountOfObjects = transforms.Length;
        for (int i = 0; i < amountOfObjects; i++)
            transforms[i].SendToLayer(layer, sendChildObjectsToSameLayer);
    }

    /// <summary>
    /// Switch layer of the gameobjects
    /// </summary>
    /// <param name="ownGameObject"></param>
    /// <param name="layer"></param>
    /// <param name="sendChildObjectsToSameLayer"></param>
    public static void SendToLayer(this GameObject[] gameObjects, int layer, bool sendChildObjectsToSameLayer = false)
    {
        var amountOfObjects = gameObjects.Length;
        for (int i = 0; i < amountOfObjects; i++)
            gameObjects[i].SendToLayer(layer, sendChildObjectsToSameLayer);
    }
    #endregion

    #region LAYER BY STRING ////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Convert string value to equivalent Layer int Value
    /// </summary>
    /// <param name="stringValue"></param>
    /// <returns></returns>
    public static int ToLayer(this string stringValue)
    {
        return LayerMask.NameToLayer(stringValue);
    }

    /// <summary>
    /// Switch layer of the transform's gameobject
    /// </summary>
    /// <param name="ownTransform"></param>
    /// <param name="layer"></param>
    /// <param name="sendChildObjectsToSameLayer"></param>
    public static void SendToLayer(this Transform ownTransform, string layer, bool sendChildObjectsToSameLayer = false)
    {
        if (sendChildObjectsToSameLayer)
        {
            var layerBitmask = layer.ToLayer();
            var transFormGameObject = ownTransform.gameObject;
            transFormGameObject.layer = layerBitmask;

            var childGameObjects = transFormGameObject.GetComponentsInChildren<GameObject>();
            var childCount = childGameObjects.Length;

            for (int i = 0; i < childCount; i++)
                childGameObjects[i].layer = layerBitmask;
        }
        else
        {
            ownTransform.gameObject.layer = layer.ToLayer();
        }
    }

    /// <summary>
    /// Switch layer of the gameobject
    /// </summary>
    /// <param name="ownGameObject"></param>
    /// <param name="layer"></param>
    /// <param name="sendChildObjectsToSameLayer"></param>
    public static void SendToLayer(this GameObject ownGameObject, string layer, bool sendChildObjectsToSameLayer = false)
    {
        if (sendChildObjectsToSameLayer)
        {
            var layerBitmask = layer.ToLayer();
            ownGameObject.layer = layerBitmask;

            var childGameObjects = ownGameObject.GetComponentsInChildren<GameObject>();
            var childCount = childGameObjects.Length;

            for (int i = 0; i < childCount; i++)
                childGameObjects[i].layer = layerBitmask;
        }
        else
        {
            ownGameObject.layer = layer.ToLayer();
        }
    }

    /// <summary>
    /// Switch layer of the transforms gameObjects
    /// </summary>
    /// <param name="ownGameObject"></param>
    /// <param name="layer"></param>
    /// <param name="sendChildObjectsToSameLayer"></param>
    public static void SendToLayer(this Transform[] transforms, string layer, bool sendChildObjectsToSameLayer = false)
    {
        var layerBitmask = layer.ToLayer();
        var amountOfTransforms = transforms.Length;
        for (int i = 0; i < amountOfTransforms; i++)
            transforms[i].SendToLayer(layerBitmask, sendChildObjectsToSameLayer);
    }

    /// <summary>
    /// Switch layer of the gameobjects
    /// </summary>
    /// <param name="ownGameObject"></param>
    /// <param name="layer"></param>
    /// <param name="sendChildObjectsToSameLayer"></param>
    public static void SendToLayer(this GameObject[] gameObjects, string layer, bool sendChildObjectsToSameLayer = false)
    {
        var layerBitmask = layer.ToLayer();
        var amountOfgameObjects = gameObjects.Length;
        for (int i = 0; i < amountOfgameObjects; i++)
            gameObjects[i].SendToLayer(layerBitmask, sendChildObjectsToSameLayer);
    }
    #endregion
}