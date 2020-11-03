using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameUtils
{
    public static bool LayerMaskContains(int layer, LayerMask lm)
    {
        return (lm == (lm | (1 << layer)));
    }

    public static bool LayerMaskContains(Collider2D collider, LayerMask lm)
    {
        return LayerMaskContains(collider.gameObject.layer, lm);
    }

    public static bool LayerMaskContains(Collision2D collision, LayerMask lm)
    {
        return LayerMaskContains(collision.gameObject.layer, lm);
    }

    public static bool LayerMaskContains(Collider collider, LayerMask lm)
    {
        return LayerMaskContains(collider.gameObject.layer, lm);
    }

    public static bool LayerMaskContains(Collision collision, LayerMask lm)
    {
        return LayerMaskContains(collision.gameObject.layer, lm);
    }

    public static void SetEulerAngles(Transform t, Vector3 euler)
    {
        Quaternion q = t.rotation;
        q.eulerAngles = euler;
        t.rotation = q;
    }

    public static float Map(float value, float fromSource, float toSource, float fromTarget, float toTarget)
    {
        return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
    }

    public static Vector3 MidPoint(Vector3 a, Vector3 b)
    {
        return new Vector3((a.x + b.x) / 2, (a.y + b.y) / 2, (a.z + b.z) / 2);
    }

    public static float ClosestTo(IEnumerable<float> collection, float target)
    {
        return collection.OrderBy(x => Mathf.Abs(target - x)).First();
    }
}
